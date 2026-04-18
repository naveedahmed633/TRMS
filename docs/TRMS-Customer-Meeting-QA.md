# TRMS — Customer Meeting Q&A

**Purpose:** Direct answers to customer questions from the TRMS meeting.  
**Reference:** Infrastructure, Application, and Security Architecture documents (docs folder).  
**Date:** February 2026  

---

## Infrastructure Architecture

### 1. The logical diagram provided doesn't show any application servers.

**Answer:** The architecture has been updated. The logical diagram now **explicitly shows an Application tier** with dedicated **App Server 1, App Server 2, … App Server N**, each hosting BLL + DLL (business logic). The Web tier (IIS + MVC) is separate; traffic flows Client → Load Balancer (Web) → Web servers (DMZ) → Load Balancer (App) → Application servers → Data tier. See **TRMS-Infrastructure-Architecture.md**, §7 Logical Diagram.

---

### 2. The application server should also be load balanced.

**Answer:** **Yes.** The application tier is **load balanced**. The diagram and text now show:
- A **Load Balancer (App)** in front of the application servers.
- Multiple application server nodes (App Server 1, 2, … N).
- HA summary: “Application servers are load balanced; redundant app server nodes; load balancer health checks.”

See **TRMS-Infrastructure-Architecture.md**, §3.3 Application Tier and §7.

---

### 3. As long as a file server is not required, where are the files stored?

**Answer:** TRMS **does not require a dedicated file server**. Files (uploads, CSV imports/exports, UNIS photos, TRMS real photos, etc.) are stored in **configurable paths** defined in **appSettings** (e.g. UNIS photos path, TRMS real photos path). These paths can be:
- **Local disks** on the application server(s), or  
- **A network share** (e.g. NAS or resilient share) if HA or central backup is required.

So: files live on the **application tier** (local or shared storage); no separate file server is mandated. See **TRMS-Infrastructure-Architecture.md**, §3.6 File Storage.

---

### 4. UAT environment specification and architecture are not provided.

**Answer:** UAT is now documented. **TRMS-Infrastructure-Architecture.md**, §5 **UAT Environment Specification and Architecture**, covers:
- **Architecture:** Same N-tier as production — Web tier (DMZ), Application tier, Data tier, Jobs/Integration; at least one (or load-balanced) node per tier.
- **Data tier:** Dedicated UAT SQL Server with TRMS_UAT and UNIS_UAT (or equivalent); no shared data with production.
- **Specification:** Same software/framework versions as production (IIS, .NET 4.8, SQL Server, etc.) per §4; hardware can be scaled down per policy.
- **Data:** Sanitized/anonymized production copy or synthetic data; refresh/retention per policy.
- **Access:** Restricted to testers/stakeholders; network segmentation and firewalls per MOC/security standards.

---

### 5. The specification table doesn't reflect all software/framework/libraries required (e.g. IIS, .NET Core 8, etc.).

**Answer:** A full **Software / Framework / Libraries Specification** table is now in **TRMS-Infrastructure-Architecture.md**, §4. It includes:

| Category        | Component              | Version / Notes                                      |
|----------------|------------------------|------------------------------------------------------|
| Web Server     | IIS                    | IIS 10 or later; Application Pool .NET CLR v4.0     |
| Runtime        | .NET Framework         | 4.8 (current); .NET 8+ per Application Architecture  |
| Application    | ASP.NET MVC            | 5.3                                                  |
| Application    | ASP.NET Web API        | 5.3                                                  |
| ORM            | Entity Framework       | 6.4.4                                                |
| Database       | SQL Server             | As per MOC (e.g. 2019+); TRMS + UNIS_*              |
| Auth           | Forms Authentication   | Custom Membership/Role providers                    |
| Libraries      | EPPlus, iTextSharp, Rotativa, BouncyCastle, gRPC, etc. | As in solution   |
| OS             | Windows Server         | Per MOC (e.g. 2019/2022)                            |
| Load Balancer  | Hardware or software   | Web tier and Application tier                       |
| Optional       | Redis / SQL Session    | Out-of-process session store for scale-out          |

**.NET Core 8 (or above):** The current production stack is .NET Framework 4.8. The **plan to move to .NET 8+** and how it applies to MOC is in the Application Architecture document (see Application Architecture section below).

---

### 6. The web servers should be inside DMZ.

**Answer:** **Yes.** The architecture now states that **web servers are deployed inside the DMZ** (demilitarized zone). The diagram shows the Web tier inside a “DMZ” band, with traffic from the load balancer going to Web Server 1, 2, … N in the DMZ. The Web tier handles presentation and SSL termination; the application tier (BLL) sits behind the DMZ. See **TRMS-Infrastructure-Architecture.md**, §3.2 Web Tier and §7.

---

### 7. "Application BLL should be deployed on dedicated load-balanced application servers" (not same IIS app pool as web).

**Answer:** **Agreed.** The target architecture is that the **BLL is deployed on dedicated, load-balanced application servers**, **not** in the same IIS application pool as the web tier. The web tier (IIS + MVC in DMZ) calls the application tier; for MOC-aligned and scale-out deployments, web and app are separate. Co-location on the same IIS pool is only noted as an option for smaller/legacy deployments. See **TRMS-Infrastructure-Architecture.md**, §3.3 Application Tier.

---

## Application Architecture

### 8. Is there a plan to move to a newer version of .NET (.NET Core 8 or above)? How will that be applied to MOC?

**Answer:**  
- **Plan:** Yes. There is a **plan to evaluate and migrate** to a modern LTS runtime such as **.NET 8** (or .NET 9+) to align with Microsoft support and MOC standards. Scope: main web app (MvcApplication1), BLL, DLL, and supporting projects; some integrations (e.g. SSIS, devices) may need separate assessment.  
- **Applied to MOC:**  
  - **Approval:** MOC approval of target .NET version and timeline.  
  - **Environments:** Dev, UAT, and Production upgraded per MOC-approved rollout; infrastructure and UAT specs updated (e.g. .NET 8 runtime).  
  - **Security/compliance:** Auth (e.g. ASP.NET Core Identity or MOC IdP), configuration, and secrets per MOC security architecture.  
  - **Testing:** UAT and security testing in MOC-aligned environments before production.  
  - **Phasing:** Phased approach (e.g. new services first) can be used; phasing to be agreed with MOC.

See **TRMS-Application-Architecture.md**, §11 .NET Upgrade Plan (.NET Core 8+).

---

## Security Architecture

### 9. Diagram: Security Components — segregate Web/App layer into two layers.

**Answer:** The Security Architecture diagram now **segregates Web and App into two distinct layers**:
- **Web Layer (DMZ — IIS + MVC):** Cookie validation, anti-forgery, session (Forms Auth timeout, cookie-based), forwards auth context to App layer.  
- **App Layer (BLL + Auth Providers):** TimeTuneMembershipProvider, TimeTuneRoleProvider, Permission* checks, CryptograpyED (key to be in secure store).

Flow: Client → Web Layer → App Layer → Data (SQL Server). See **TRMS-Security-Architecture.md**, §7 Diagram.

---

### 10. Integration with internal MOC identity provider.

**Answer:**  
- **Current:** TRMS uses application-managed identity (TimeTuneMembershipProvider with SQL + file-based store for SUPER_ADMIN/Sudo). There is **no active integration** with an internal MOC central identity provider (e.g. SAML/OIDC/LDAP).  
- **Planned:** **Integration with the internal MOC identity provider is planned.** Options include: (1) **Federation** — TRMS as relying party to MOC IdP (SAML 2.0 or OpenID Connect); (2) **LDAP/AD** — validate users against MOC directory and map roles via groups.  
- **Impact:** Migration will require changes to authentication middleware, role resolution, and possibly session handling; docs will be updated when the approach is finalized with MOC.

See **TRMS-Security-Architecture.md**, §2.5 Integration with Internal MOC Identity Provider.

---

### 11. Is there any mechanism to handle user sessions?

**Answer:** **Yes.** TRMS has the following session mechanisms:
- **Forms Authentication cookie** — primary session token; name, path, domain, secure/HTTP-only configurable in Web.config.  
- **Timeout** — configurable (e.g. 50 minutes) with **sliding expiration** (ticket renewed on activity).  
- **Session state:** Default is in-process; for load-balanced deployments **SQL Server session state** or **Redis** (or similar) is recommended so all web/app nodes share the same store.  
- **Logout** — sign-out invalidates the forms auth cookie (and optionally server-side session).

These will be aligned with MOC policy for session duration, concurrent logins, and cookie settings. See **TRMS-Security-Architecture.md**, §2.4 User Session Handling.

---

### 12. High risk: Hard-coded credential (e.g. Sudo@TimeTune1) for operational access.

**Answer:** **Acknowledged as high risk.** The Sudo operational account password is currently hard-coded in the BLL (e.g. `UserAuthentication.validatePasswordForSudo`).  
**Remediation:** Remove the hard-coded password. Store Sudo credentials in **secure configuration** (e.g. encrypted appSettings, Azure Key Vault, or MOC-approved secrets store) and validate the password from config/secrets at runtime. See **TRMS-Security-Architecture.md**, §8 High-Risk Items and Remediation.

---

### 13. High risk: Super Admin uses a file-based credential store.

**Answer:** **Acknowledged as high risk.** SUPER_ADMIN uses a **file-based credential store** (`ShadowFile.rsa`) for password hash and salt.  
**Remediation:** Migrate Super Admin identity to **database or enterprise IdP** (e.g. same secure store as other privileged accounts, or MOC identity provider). Eliminate `ShadowFile.rsa` for production. See **TRMS-Security-Architecture.md**, §8.

---

### 14. High risk: Encryption key is currently in code.

**Answer:** **Acknowledged as high risk.** The AES encryption key used by CryptograpyED (e.g. in `Crypto.cs`) is **currently in source code** (static key and salt).  
**Remediation:** Move the encryption key to a **secure store** (e.g. Azure Key Vault, HSM, or protected configuration), load it at runtime, never commit keys to source control, and rotate per organizational/MOC policy. See **TRMS-Security-Architecture.md**, §8.

---

*For full detail, refer to TRMS-Infrastructure-Architecture.md, TRMS-Application-Architecture.md, and TRMS-Security-Architecture.md in the docs folder.*
