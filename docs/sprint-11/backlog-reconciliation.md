# S11 Jira backlog reconciliation

Jira: `N4WTT-247`

Baseline: 64 issues were in a non-Done state at the start of S11.

## Rules

- `Done` requires code, test or release evidence.
- A visible placeholder page is not implementation evidence.
- Partial scope remains open; the comment must identify the missing behavior.
- Temporary permission-test issues are closed only after their original permission check is recorded.
- Legacy epics are not used to hide pending stories; they remain open or are explicitly marked
  superseded by a newer tracked epic.

## Implemented and ready to close

| Scope | Jira | Evidence |
| --- | --- | --- |
| Role UI and RBAC boundary | `N4WTT-54`, `N4WTT-55` | role Areas, authorization policies, acceptance TC37-TC39, security SEC05 |
| Deterministic substitute scoring | `N4WTT-79`, `N4WTT-93`, `N4WTT-94` | `RecommendationScoring`, unit boundary tests, acceptance TC21 |
| Prescription warning | `N4WTT-80`, `N4WTT-95`, `N4WTT-96` | prescription field, scoring alert, search/detail badges and tests |
| Admin audit log | `N4WTT-88`, `N4WTT-111`, `N4WTT-112` | SQL audit service, admin writes, persistence integration test |
| Out-of-stock dashboard | `N4WTT-90`, `N4WTT-115`, `N4WTT-116` | reporting service, Admin dashboard, acceptance TC25 |
| Explanation and confidence | `N4WTT-92`, `N4WTT-119`, `N4WTT-120` | deterministic score labels/reasons, guarded AI explanation and fallback tests |
| Temporary Jira permission checks | `N4WTT-122`, `N4WTT-123` | original Jira create/log permission test completed |

## Partial and must remain open

| Scope | Jira | Implemented portion | Missing portion |
| --- | --- | --- | --- |
| Allergy and contraindication safety | `N4WTT-81`, `N4WTT-82`, `N4WTT-97` to `N4WTT-100` | active-ingredient allergy and text contraindication warnings | structured excipient, diagnosis and severity model |
| Manual backup and history | `N4WTT-89`, `N4WTT-113`, `N4WTT-114` | JSON backup download and audit log | backup catalogue, retention and restore from UI |

## Genuine pending product scope

| Scope | Jira | Current gap |
| --- | --- | --- |
| Search history and saved medicines | `N4WTT-83`, `N4WTT-101`, `N4WTT-102` | User Area pages are placeholders without persistence |
| User health profile management | `N4WTT-84`, `N4WTT-103`, `N4WTT-104` | seeded profiles exist but users cannot maintain them |
| External catalogue import | `N4WTT-85`, `N4WTT-105`, `N4WTT-106` | source registry exists; no import pipeline |
| External ID mapping | `N4WTT-86`, `N4WTT-107`, `N4WTT-108` | status metadata exists; no medicine/ingredient mapping entity |
| Inventory API timeout fallback | `N4WTT-87`, `N4WTT-109`, `N4WTT-110` | no live inventory provider contract or fallback cache |
| PDF and Excel exports | `N4WTT-91`, `N4WTT-117`, `N4WTT-118` | dashboard and JSON backup exist; requested report formats do not |
| Public API and MCP governance | `N4WTT-230` to `N4WTT-234` | planned in future S9 |
| Alternative AI hosting or local ONNX | `N4WTT-241` | planned in future S9 after accepted Somee limitation |

## Legacy epic treatment

`N4WTT-4` to `N4WTT-9` and `N4WTT-73` to `N4WTT-78` overlap later sprint work and the detailed
stories above. They remain open during S11 reconciliation until every child scope is either closed with
evidence or assigned to a named future sprint. They must not be counted as independent missing product
features in addition to their child stories.

## S11 action

1. Close the implemented/evidenced issues in the first table.
2. Add explicit missing-scope comments to partial stories.
3. Create future delivery sprints for user persistence, external data and exports.
4. Keep S9 isolated for API/MCP and AI-hosting decisions.
5. Recount the backlog after transitions and attach the result to `N4WTT-247`.
