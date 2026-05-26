# Sprint S1,5 - Test Case Matrix

| ID | Jira | Direction | Automated Check | Expected Result |
| --- | --- | --- | --- | --- |
| TC01 | N4WTT-33 | Smoke | Home page renders app dashboard | HTTP 200, dashboard title and skip link visible |
| TC02 | N4WTT-36 | Search | Search finds medicine by active ingredient | Paracetamol result and stock status visible |
| TC03 | N4WTT-40 | Edge data | Unknown keyword returns empty state | No-result state is clear |
| TC04 | N4WTT-36 | Search filter | Category filter narrows results | Antibiotic appears, unrelated analgesic is absent |
| TC05 | N4WTT-37 | Drug detail | Detail page shows same-active-ingredient alternatives | Alternative section includes Paracetamol DHG |
| TC06 | N4WTT-41 | Error handling | Invalid drug detail returns clean 404 | 404 without exception text |
| TC07 | N4WTT-35 | RBAC | Anonymous user opens catalog admin | Redirects to login |
| TC08 | N4WTT-34 | Auth/session | Invalid login attempt | Validation error shown |
| TC09 | N4WTT-39 | Inventory | Pharmacist opens inventory | Inventory dashboard loads |
| TC10 | N4WTT-35 | RBAC | Normal user opens catalog admin | Redirects to access denied |
| TC11 | N4WTT-34 | Session | Logout clears protected access | Protected page redirects after logout |
| TC12 | N4WTT-38 | Catalog CRUD | Invalid catalog create post | Form remains open with validation error |
| TC13 | N4WTT-42 | Security | Inventory POST without anti-forgery token | Request rejected with 400 |
| TC14 | N4WTT-47 | Cache/static asset | `site.css` request | CSS served with revalidation metadata |
| TC15 | N4WTT-43 | Responsive | CSS breakpoint rules | Responsive collapse rules are present |
| TC16 | N4WTT-44 | Accessibility | Login form markup | Labels and autocomplete attributes exist |
| TC17 | N4WTT-45 | Performance | 20 search requests | Average under 650 ms, max under 2000 ms |
| TC18 | N4WTT-48 | Memory | 50 repeated requests | Working set under 350 MB |
| TC19 | N4WTT-49 | Debug | Unknown route | 404 without stack trace text |
| TC20 | N4WTT-51 | Concurrency | 12 parallel public reads | All requests succeed |
| TC21 | N4WTT-50 | Maintainability | Build and test project structure | Tests are isolated from Web SDK compile glob |
| TC22 | N4WTT-52 | Regression traceability | PR/Jira/test mapping | PR body and matrix link test coverage to tickets |

## Coverage Summary

- Functional: TC01 to TC05, TC09, TC12.
- Access and security: TC07, TC08, TC10, TC11, TC13.
- UI quality: TC14, TC15, TC16.
- Non-functional: TC17, TC18, TC20.
- Debug and maintainability: TC06, TC19, TC21, TC22.
