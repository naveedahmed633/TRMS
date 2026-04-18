# TRMS Security Architecture

**Document:** Security Architecture  
**Subject:** IAM, access control, audit logging, and encryption  
**Version:** 1.1  
**Date:** February 2026  

---

## 1. Overview

This document describes the security architecture of TRMS (Time & Resource Management System), including identity and access management (IAM), access control, audit logging, and encryption as implemented in the application.

---

## 2. Identity and Access Management (IAM)

### 2.1 Authentication

- **Mode:** ASP.NET **Forms Authentication** (Web.config: `authentication mode="Forms"`).
- **Login URL:** `~/Account/Login`; unauthenticated users are redirected to login.
- **Session:** Configurable timeout (e.g. 50 minutes) with sliding expiration; cookie-based authentication. See **§2.4 User Session Handling** and **§7** diagram for mechanisms.

**Identity Sources:**

| User Type | Storage / Validation |
|-----------|----------------------|
| **Regular users** | SQL Server (TRMS) — `employee` table; validated via **TimeTuneMembershipProvider**. |
| **SUPER_ADMIN** | File-based credential store (`ShadowFile.rsa`) — password hash and salt; validated by **TimeTuneMembershipProvider**. |
| **Sudo** | Hard-coded credential (e.g. `Sudo@TimeTune1`) for operational access; can be replaced with config or DB. |

### 2.2 Password Management

- **Hashing:** Passwords are not stored in plain text. The application uses **salted hashing** (DLL.Commons.Passwords):
  - 128-bit salt (secure PRNG).
  - Hash stored with salt in DB (`employee.password`, `employee.salt`) or in `ShadowFile.rsa` for Super Admin.
- **Validation:** `DLL.Commons.Passwords.validate(user, attemptedPassword)` for employees; equivalent logic for Super Admin.
- **Change password:** Supported via MembershipProvider `ChangePassword`; Super Admin password change updates `ShadowFile.rsa`.

### 2.3 Role-Based Identity (RBAC)

- **Provider:** **TimeTuneRoleProvider** (Web.config: default role provider).
- **Roles** (DLL.Commons.Roles):

| Role Constant | Purpose |
|---------------|--------|
| TimeTuneHR | Human Resources |
| TimeTuneEMP | Employee |
| TimeTuneLM | Line Manager |
| TimeTuneSLM | Super Line Manager |
| TimeTuneSU | Super User (SUPER_ADMIN) |
| TimeTuneADMIN | Admin |
| TimeTuneREPORT | Report-only access |
| TimeTuneSUDO | Sudo (operational) |

- **Resolution:** Roles are resolved from the database via **AccessGroup** — each employee is linked to an `access_group`; `access_group.name` is the role name returned by `GetRolesForUser`.
- **Special accounts:** SUPER_ADMIN returns `ROLE_SUPER_USER`; Sudo returns `ROLE_SUDO` without DB lookup.

### 2.4 User Session Handling

TRMS provides the following mechanisms for user sessions:

- **Forms Authentication cookie:** The primary session token. Cookie name, path, domain, and secure/HTTP-only flags are configurable in Web.config (`forms` element under `authentication`).
- **Timeout:** Configurable (e.g. 50 minutes) with **sliding expiration** — the ticket is renewed on activity within the timeout window.
- **Session state:** By default, ASP.NET uses **in-process session state**. For load-balanced deployments, **out-of-process** options are recommended:
  - **SQL Server session state** — store session in a dedicated SQL database; all web/app nodes share the same store.
  - **Redis (or similar)** — distributed cache for session; supports stateless scale-out.
- **Concurrent sessions:** The current implementation does not enforce a single-session-per-user limit; this can be added at application or identity-provider level if required by MOC policy.
- **Logout:** Sign-out invalidates the forms authentication cookie (e.g. `FormsAuthentication.SignOut()`); server-side session state may be abandoned depending on implementation.

These mechanisms should be aligned with MOC policy for session duration, concurrent logins, and secure cookie settings.

### 2.5 Integration with Internal MOC Identity Provider

- **Current state:** TRMS today uses **application-managed identity** (TimeTuneMembershipProvider with SQL Server and file-based stores for SUPER_ADMIN/Sudo). There is **no active integration with an internal MOC central identity provider** (e.g. SAML/OIDC/LDAP).
- **Planned direction:** Integration with the **internal MOC identity provider** is planned to align with enterprise IAM and MOC security standards. Options include:
  - **Federation:** TRMS as a relying party (RP) to MOC IdP (e.g. SAML 2.0 or OpenID Connect); users authenticate at MOC IdP and are issued tokens/assertions consumed by TRMS.
  - **LDAP/AD integration:** If MOC standard is directory-based, TRMS can validate users against MOC LDAP/Active Directory and map roles via group membership.
- **Impact:** Migration from Forms Auth + custom providers to MOC IdP will require changes to authentication middleware, role resolution, and possibly session handling; the Security and Application Architecture documents will be updated when the integration approach is finalized with MOC.

---

## 3. Access Control

### 3.1 Authorization (MVC)

- **Controller-level:** `[Authorize]` on controllers (e.g. AccountController) — only authenticated users can access.
- **Action-level:** `[AllowAnonymous]` used for login and public actions; other actions require authentication.
- **Role-based access:** Application logic and area routing enforce role-based access:
  - **Areas:** HR, LM, SLM, EMP, Report, Sudo, SuperAdmin — each area is intended for a subset of roles.
  - **Permission tables:** PermissionUser, PermissionHR, PermissionLM, PermissionSuperAdmin — used to restrict which users can access specific functions or reports.

### 3.2 Access Groups and Permissions

- **AccessGroup:** Defines the primary role (e.g. TimeTuneHR, TimeTuneLM) for an employee.
- **Permission*:** Fine-grained permissions (e.g. report access, admin functions) stored in permission tables and checked in business logic and controllers.
- **Super Admin code:** Configurable via appSettings (e.g. `SuperAdminCode`); used for super-admin–related checks.
- **Organization access:** `Organization_Access_Denied` (appSettings) used to deny access to certain organization codes.

### 3.3 Anti-Forgery and Request Security

- **Anti-forgery:** `HandleAntiForgeryError` filter handles `HttpAntiForgeryException` and redirects to login; validation tokens used where applicable to prevent CSRF.
- **Client validation:** Enabled (Web.config: `ClientValidationEnabled`, `UnobtrusiveJavaScriptEnabled`).

---

## 4. Audit Logging

### 4.1 Audit Trail (Application-Level)

- **Table:** `audit_trail` (DLL.Models.AuditTrail).
- **Provider:** `TimeTune.AuditTrail` in BLL (insert, update, delete methods).
- **Recorded fields (conceptually):**
  - **action_code:** insert | update | delete (DLL.Commons.actionCode).
  - **table:** Affected logical entity (e.g. "Employees", "ManualAttendance", "GeoPhencing").
  - **description:** JSON-like or text description of the change (e.g. ids, key fields).
  - **user_code:** Identity of the user who performed the action.
  - **create_date_adt:** Timestamp of the action.

- **Usage:** Called from business logic for sensitive operations (e.g. employee import/export, manual attendance, geo-fencing, leave/appraisal-related changes). Some CSV job audit calls are commented out but the pattern is in place.

### 4.2 Manual Attendance Log

- **Model:** ManualAttLog (e.g. WhoMark, timestamps) — tracks who marked manual attendance and when.
- **Purpose:** Accountability for manual attendance changes.

### 4.3 Viewing Audit Data

- **SuperAdmin area:** View "ViewAuditLog" (and related views) allow authorized users to view audit trail data (e.g. sorted by AuditTrailId, filtered by user/table/date as implemented in TimeTune).

### 4.4 HaTransit and Raw Data

- **HaTransit:** Raw attendance/transaction logs from devices; retained and processed by the Attendance Pipeline. Not the same as application audit trail but part of data lineage for attendance.
- **DeleteHaTransitLogs:** Utility to purge or manage HaTransit data per retention policy.

---

## 5. Encryption

### 5.1 Sensitive Data Encryption (Application)

- **Library:** CryptograpyED project (Crypto class).
- **Algorithm:** **AES** (symmetric) with key derived using **Rfc2898DeriveBytes** (PBKDF2) from a key and fixed salt.
- **Usage:** Encrypt/Decrypt methods used for protecting sensitive strings (e.g. credentials or configurable sensitive fields). Encryption key is currently in code; for production it should be moved to a secure store (e.g. Azure Key Vault, HSM, or protected config).

### 5.2 Cryptographic Libraries

- **System.Security.Cryptography:** Used for hashing (passwords), AES, and key derivation.
- **BouncyCastle:** Referenced in the solution for additional cryptographic support where needed.

### 5.3 Transport and Data at Rest (Recommendations)

- **Transport:** Use **HTTPS (TLS)** in production for all web traffic to protect authentication and data in transit.
- **Data at rest:** SQL Server TDE (Transparent Data Encryption) and encrypted backups align with MOC/enterprise policies; implement as per organizational standards.
- **Connection strings:** Store in secured configuration; avoid plain-text passwords in source control; use managed identities or restricted service accounts where applicable.

---

## 6. Security Architecture Summary

| Area | Implementation |
|------|----------------|
| **IAM** | Forms Authentication; TimeTuneMembershipProvider (DB + file for Super Admin); TimeTuneRoleProvider (roles from AccessGroup). **MOC IdP integration planned** (§2.5). |
| **Sessions** | Configurable timeout, sliding expiration, cookie-based; SQL Server or Redis session store for scale-out (§2.4). |
| **Passwords** | Salted hash (Commons.Passwords); no plain-text storage. |
| **Access control** | [Authorize], role-based areas (HR, LM, SLM, EMP, Report, Sudo, SuperAdmin), permission tables. |
| **Audit** | audit_trail (action, table, description, user_code, date); ManualAttLog for manual attendance. |
| **Encryption** | AES (CryptograpyED) for sensitive strings; **key must move to secure store** (§8); TLS for transport; TDE per policy. |
| **High risks** | Hard-coded Sudo credential; Super Admin file store; encryption key in code — see §8 for remediation. |

---

## 7. Diagram (Security Components) — Web and App Layers Segregated

The security components are shown with **Web layer** and **App layer** as two distinct tiers (aligned with Infrastructure Architecture: Web in DMZ, App on dedicated servers).

```
┌─────────────────────────────────────────────────────────────────┐
│  Client (Browser)                                                │
│  HTTPS (recommended) → Forms Auth Cookie                         │
└────────────────────────────┬────────────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────────┐
│  Web Layer (DMZ — IIS + MVC)                                     │
│  • [Authorize] / [AllowAnonymous]                                │
│  • Cookie validation, anti-forgery, client validation            │
│  • Session: Forms Auth timeout (configurable); cookie-based      │
│  • Forwards auth context to App layer                            │
└────────────────────────────┬────────────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────────┐
│  App Layer (BLL + Auth Providers)                                │
│  • TimeTuneMembershipProvider (ValidateUser, ChangePassword)     │
│  • TimeTuneRoleProvider (GetRolesForUser → AccessGroup)          │
│  • Permission* checks (PermissionUser, PermissionHR, …)           │
│  • CryptograpyED (AES) — key to be in secure store (see §8)      │
└────────────────────────────┬────────────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────────┐
│  Data (SQL Server)                                               │
│  • employee (password hash + salt), access_group                  │
│  • audit_trail (action_code, table, description, user_code, date)│
│  • Permission*, ManualAttLog                                     │
│  • TDE / backup encryption (recommended)                         │
└─────────────────────────────────────────────────────────────────┘
```

---

## 8. High-Risk Items and Remediation

The following items have been flagged as **high risk** in security reviews. Current state and recommended remediation are documented below.

| Risk | Current State | Remediation |
|------|----------------|-------------|
| **Hard-coded credential (e.g. Sudo@TimeTune1)** | The **Sudo** operational account password is hard-coded in the BLL (e.g. `UserAuthentication.validatePasswordForSudo`). | **Remediation:** Remove hard-coded password. Store Sudo credentials in **secure configuration** (e.g. appSettings with encrypted values, Azure Key Vault, or MOC-approved secrets store). Validate Sudo password from config/secrets at runtime. |
| **Super Admin file-based credential store** | **SUPER_ADMIN** uses a **file-based credential store** (`ShadowFile.rsa`) for password hash and salt. | **Remediation:** Migrate Super Admin identity to **database or enterprise IdP**. Prefer storing SUPER_ADMIN in the same secure store as other privileged accounts (e.g. SQL with strong access control, or MOC identity provider). Eliminate `ShadowFile.rsa` for production. |
| **Encryption key in code** | The **AES encryption key** used by CryptograpyED (e.g. for sensitive strings) is **currently in source code** (e.g. `Crypto.cs`: static key and salt). | **Remediation:** Move the encryption key to a **secure store** (e.g. Azure Key Vault, HSM, or protected configuration) and load it at runtime. Never commit keys to source control. Rotate keys per organizational/MOC policy. |

These remediations should be planned in line with the .NET upgrade and MOC identity provider integration (see Application and Security architecture sections).

---

*This document reflects the security controls implemented in the TRMS codebase. Section 8 documents high-risk items and remediation. Operational hardening (e.g. TLS, key management, DB encryption, backup security) should follow organizational and MOC security policies.*
