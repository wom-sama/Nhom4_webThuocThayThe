# Sprint S1,6 - Security Test Case Matrix

| ID | Jira | Area | Scenario | Expected Result |
| --- | --- | --- | --- | --- |
| SEC01 | N4WTT-57 | Password storage | Inspect account model/service | AppUser stores `PasswordHash` and `PasswordSalt`; account service uses PBKDF2; no plaintext seed password in storage service |
| SEC02 | N4WTT-58 | Authentication | Login with a valid hashed account | Dashboard loads successfully |
| SEC03 | N4WTT-58 | Authentication | Invalid login | No auth cookie is issued |
| SEC04 | N4WTT-58 | Cookie security | Successful login response | Auth cookie has HttpOnly and SameSite=Lax |
| SEC05 | N4WTT-59 | RBAC | Pharmacist and normal user access inventory | Pharmacist succeeds; normal user is redirected to access denied |
| SEC06 | N4WTT-60 | CSRF | POST protected form without token | Request is rejected with HTTP 400 |
| SEC07 | N4WTT-61 | Open redirect | Login with external `ReturnUrl` | App redirects locally, not to the external domain |
| SEC08 | N4WTT-62 | XSS encoding | Search with script payload | Raw script payload is not rendered |
| SEC09 | N4WTT-63 | Error leakage | Missing drug detail | 404 response does not leak stack traces, paths or password fields |
| SEC10 | N4WTT-63 | Token rendering | Authenticated protected page | Anti-forgery token is not blank or malformed |

## Risk Traceability

- Passwords: SEC01, SEC02, SEC03.
- Session and cookies: SEC03, SEC04.
- Authorization: SEC05.
- Request forgery and tampering: SEC06.
- Redirect and input handling: SEC07, SEC08.
- Debug and sensitive leakage: SEC09, SEC10.
