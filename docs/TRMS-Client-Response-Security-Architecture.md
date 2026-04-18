# TRMS — Response to Security & Architecture Review

**To:** Client  
**From:** [Vendor / Project Team]  
**Subject:** Responses to Security and Architecture Findings  
**Date:** February 2026  

---

We have reviewed the security and architecture findings and provide below our responses and committed remediation for each item.

---

## Our Position: No New Third-Party Integrations (Except SSO)

We will address all findings **without introducing any new third-party integrations**, other than **SSO with the client’s chosen Identity Provider (IdP)**. Our approach is:

- **Secrets, keys, and credentials:** Use **client-approved or in-house mechanisms** (e.g. secure configuration, client’s existing secret store or HSM, environment-based config). We will not mandate a specific commercial secrets service.
- **Identity (SSO only):** We will integrate **only with the client’s IdP for SSO**. No separate Azure AD or other cloud IdP unless the client already uses it as their IdP. MFA/OIDC will be fulfilled through the client’s SSO/IdP capability.
- **Infrastructure (WAF, session, monitoring):** We will align with **client’s existing infrastructure** (e.g. client’s WAF/reverse proxy, client’s SQL Server for session state, client’s SIEM/monitoring). We will document requirements and integration points only.
- **APIs and integrations:** API security (OAuth2/OIDC, scopes) will use the **same SSO/IdP** where applicable. UNIS, Alpeta, and ESB are existing/client integrations; we will document them via ICDs without adding new third-party products.

This keeps the solution defensible, compliant with the findings, and within the constraint of **no third-party integration except SSO**.

---

## Critical Findings

### 1. Hard-coded “Sudo” credential

**Finding:** Hard-coded Sudo login (e.g. Sudo@TimeTune1) presents privilege escalation and credential leak risk.

**Our response:** We acknowledge this finding. We will remove all hard-coded Sudo credentials from the codebase and replace them with **client-approved or in-house secret management** (e.g. secure configuration, environment variables, or client’s existing secret store—no new third-party service). Sudo validation will be performed at runtime from secure configuration only. Timeline will be agreed as part of the immediate remediation plan.

---

### 2. Super Admin stored in local file (ShadowFile.rsa)

**Finding:** Storing high-privilege credentials in a file is non-compliant and easily compromised.

**Our response:** We agree. We will **migrate Super Admin authentication** away from the file-based store (ShadowFile.rsa) to the **client’s IdP via SSO** (the only third-party integration we introduce) or to a secured database store with appropriate access control and audit. ShadowFile.rsa will be eliminated in production deployments. No additional IdP or directory product will be introduced beyond the client’s chosen SSO/IdP.

---

### 3. Encryption key in source code

**Finding:** The AES key is in code; must be migrated to Key Vault/HSM with proper rotation.

**Our response:** We accept this finding. We will **migrate the encryption key** to **client-approved or in-house secure storage** (e.g. client’s HSM, client’s key store, or secured configuration—no new third-party key service). A defined **key rotation policy** will be documented. Keys will no longer reside in source code or be committed to source control. Implementation evidence and key custody model will be documented.

---

## High Findings — IAM

### 4. Legacy Forms Authentication; no SSO/MFA/OIDC

**Finding:** Documents show Forms Auth and custom providers only; no Azure AD / OIDC / MFA.

**Our response:** We will **modernize IAM** to meet enterprise standards **only through SSO**. We will deliver a **plan and timeline** for integration with the **client’s chosen IdP for SSO** (OIDC/SSO; MFA where supported by the client’s IdP). No separate third-party identity service will be introduced. This will replace the current Forms Authentication and custom providers in line with client security standards.

---

### 5. Permission sprawl and complexity

**Finding:** Multiple permission tables increase misconfiguration risk and privilege drift.

**Our response:** We will propose **role consolidation and least-privilege reviews**. This will include a simplified role/permission model, mapping from current to target state, and recommendations to reduce permission table sprawl and enforce least privilege.

---

## Medium Findings — Audit

### 6. Partial audit coverage / split data lineage

**Finding:** AuditTrail exists but raw device logs (HaTransit) are separate; correlation and retention should be centrally governed (SIEM).

**Our response:** We will ensure **correlation and retention are centrally governed**. We will document data lineage from device → HaTransit → consolidated attendance → audit_trail, and define retention and **integration with the client’s existing SIEM/monitoring** (no new third-party SIEM). Requirements for both application audit trail and device/raw logs will be documented.

---

## High Findings — Architecture & Infrastructure

### 7. Web (MVC) and BLL run in-process

**Finding:** BLL runs within the same IIS process as the web app; weakens N-tier separation and limits horizontal scaling.

**Our response:** We will provide a **plan to separate the Application tier**. The BLL will be deployed on dedicated, load-balanced application servers, with the web tier (MVC) calling the application tier via a defined interface. This will enable true N-tier separation and horizontal scaling.

---

### 8. Diagram ambiguity: front end appears to touch DB

**Finding:** At least one diagram suggests direct MVC→DB access, contradicting the tiered approach.

**Our response:** We will **correct and reissue** all architecture diagrams. Data flow will be explicitly shown as: Client → Web (MVC) → Application (BLL/DLL) → Database only. There is no direct MVC→DB access; revised diagrams and data flow documentation will be issued to the client.

---

### 9. Session state reliance on sticky sessions

**Finding:** Guidance recommends affinity (sticky session) when in-process session is used; this undermines HA.

**Our response:** We will **enforce distributed session** using **client’s existing infrastructure** (e.g. SQL Server session state on client’s SQL Server—no new third-party session store such as Redis unless the client already uses it). The web tier will be stateless; sticky sessions will not be required. We will document the session store configuration and high-availability behaviour.

---

### 10. HA for jobs/services not fully defined

**Finding:** Windows services and console jobs (AttendancePipeLine, CSV jobs, UNIS sync) lack failover/orchestration strategy.

**Our response:** We will provide a **runbook and resiliency design** for critical jobs and services, including failover, restart, and orchestration strategy, plus a dependency matrix.

---

### 11. Missing File Server sizing

**Finding:** File Server specs (SSD/RAM/cores) were left blank, blocking capacity planning.

**Our response:** We will provide **full sizing and performance assumptions** for any file or shared storage used by TRMS (IOPS, capacity, redundancy) to support capacity planning.

---

### 12. No explicit WAF / reverse proxy posture

**Finding:** Public-facing deployments typically require a WAF; L7 protection and TLS termination not described.

**Our response:** We will define and document **requirements for WAF and reverse proxy** and align with the **client’s existing L7 protection** (e.g. client’s WAF, reverse proxy, or load balancer). We will not introduce a new third-party WAF product; we will document L7 protection, bot mitigation, and TLS termination strategy for the client’s infrastructure team.

---

### 13. Unclear SQL HA mode and RTO/RPO

**Finding:** Docs mention Always On AG or FCI but do not commit to one or quantify RTO/RPO or quorum.

**Our response:** We will provide a **concrete HA/DR design** for SQL Server, including chosen HA mode (AG vs FCI), **RTO/RPO** targets, **quorum design**, and backup/restore verification schedule.

---

## High Findings — Integrations & Data

### 14. API/gRPC security details not specified

**Finding:** Web API/OData/gRPC are listed without token model, scopes, mTLS, rate limits, or gateway controls.

**Our response:** We will submit an **API security architecture** document covering **OAuth2/OIDC** (using the **same client IdP/SSO**—no additional third-party auth service), **scopes**, **mTLS** where applicable, **throttling/rate limits**, and **gateway controls** (aligned with client’s existing gateway where applicable).

---

### 15. External systems (UNIS, Alpeta, ESB) lack trust boundary detail

**Finding:** Need clear ICDs: auth method, data classification, error handling, and audit for each integration.

**Our response:** We will provide **detailed Interface Control Documents (ICDs)** for UNIS, Alpeta, and ESB (existing/client integrations—no new third-party systems). Each ICD will include authentication method, data classification, error handling, and audit requirements.

---

### 16. Database credential management for jobs

**Finding:** Jobs and services appear to use SQL logins via config; require secret storage and least-privileged service accounts.

**Our response:** We will implement **client-approved or in-house secret storage** (e.g. client’s secret store, secured config, or environment-based credentials—no new third-party vault) for database credentials used by jobs and services, and use **least-privileged service accounts**. Credentials will not be stored in config files in source control.

---

### 17. Data at rest encryption status unclear

**Finding:** TDE and encrypted backups are “recommended,” not confirmed.

**Our response:** We will confirm **implementation of TDE and encrypted backups** and provide **implementation evidence** and the **key custody model** (including rotation) in line with client policy.

---

## Medium Findings — Operations & Governance

### 18. Monitoring & alerting not defined

**Finding:** Need end-to-end monitoring (IIS, jobs, queues, DB, integrations) plus SLIs/SLOs and alert runbooks.

**Our response:** We will deliver a **monitoring and alerting coverage map** covering IIS, jobs, queues, database, and integrations, together with **SLIs/SLOs** and **alert runbooks**, aligned with the **client’s existing monitoring/SIEM** (no new third-party monitoring product).

---

### 19. Patch/upgrade windows vs HA

**Finding:** Maintenance approach is high level; need rolling upgrade plan, blue-green/canary options, and dependency matrix.

**Our response:** We will document the **maintenance approach**, including **rolling upgrade plan**, **blue-green/canary** options where applicable, and a **dependency matrix** for patches and upgrades.

---

### 20. Change control around permissions

**Finding:** With multiple permission tables, require maker-checker, approvals, and periodic access recertification.

**Our response:** We will implement **maker-checker**, **approvals**, and **periodic access recertification** for permission changes, and document these in change management and operational procedures.

---

## Immediate Remediation Path — Summary of Deliverables

We commit to the following deliverables as part of the immediate remediation path:

| Area | Deliverables |
|------|--------------|
| **Security (deadline-bound)** | Remove hard-coded and file-based Super Admin/Sudo credentials; move all keys and secrets to client-approved/in-house store (no new third party); deliver SSO-only plan and timeline (client’s IdP). |
| **Architecture** | Revised diagrams (no MVC→DB); separate Application tier design; distributed session using client’s SQL (or existing store); stateless web tier. |
| **Infrastructure** | Confirm SQL HA mode (AG vs FCI) with RTO/RPO; provide File Server/storage sizing; define WAF/reverse proxy requirements (client’s existing infrastructure). |
| **Integration security** | API security architecture (OAuth2/OIDC via client SSO, scopes, mTLS, throttling); detailed ICDs for UNIS, Alpeta, ESB with audit and error handling (no new third-party integrations). |
| **Operations & monitoring** | Monitoring/alerting coverage map (client’s existing SIEM/monitoring); DR test plan; change management controls for permissions and jobs. |

---

We remain available to discuss timelines and priorities for each item and to align with your security and architecture standards. All remediations above are designed to be **defensible and compliant** while respecting our commitment to **no new third-party integrations except SSO with your chosen IdP**.

**[Vendor / Project Team]**
