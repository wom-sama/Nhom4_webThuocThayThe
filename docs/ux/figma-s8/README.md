# S8 manual Figma handoff

Jira: `N4WTT-208`, `N4WTT-215`, `N4WTT-216`, `N4WTT-228`, `N4WTT-229`

## Source of truth

- File: [N4WTT – S8 UI Design](https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design)
- File key: `fu43wNGxTFRMghHrlH9lhI`
- Design candidate: `Manual V1 · 22/06/2026`
- Method: manually authored vector frames in Figma Design; no Figma AI.

Page links:

1. [Foundations & Role Architecture](https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design?node-id=2-230)
2. [Desktop Role Areas](https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design?node-id=2-1086)
3. [Mobile & State Matrix](https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design?node-id=2-2098)

The SVG files in this directory are the auditable source used to populate the Figma pages. Figma is
the visual source of truth after owner approval; the SVG copies make the exact reviewed vectors
available in Git history.

## Page inventory

### 01 Foundations & Role Architecture

- cover and changelog metadata;
- role-to-Area redirect flow after authentication;
- Public, User, Pharmacist, Expert and Admin route/navigation ownership;
- semantic colors, typography, spacing and accessibility constraints;
- button, field, badge, warning, AI and session-state components;
- explicit policy that `AccessDenied` is only for direct unauthorized URLs.

### 02 Desktop Role Areas

| Frame | Route | Primary job |
| --- | --- | --- |
| D01 Public search | `/Drugs` | Search, availability and safety before optional AI |
| D02 User dashboard | `/User` | Personal history, saved medicines and change notices |
| D03 Pharmacist workspace | `/Pharmacist` | Out-of-stock search, comparison and consultation confirmation |
| D04 Expert review | `/Expert` | Evidence review, decision, mandatory note and audit trail |
| D05 Admin dashboard | `/Admin` | Operational metrics, roles, sources and backup status |
| D06 Secure authentication | `/Auth/Login` | Authenticate without exposing demo accounts or role selection |
| D07 Public detail | `/Drugs/Details/{id}` | Stock, safety and deterministic candidate comparison |
| D08 User history | `/User/History` | Time-stamped history and stale-stock notice |
| D09 Admin catalogue | `/Admin/DrugCatalog` | CRUD, source status, backup and destructive-action controls |

Each authenticated frame has a distinct shell and navigation. Shared visual components do not imply a
shared dashboard or a menu containing inaccessible modules.

### 03 Mobile & State Matrix

- Public, User, Pharmacist, Expert, Admin and Auth frames at `390 x 844`;
- role-specific bottom navigation and minimum `44 px` touch targets;
- loading, no-results, service-error, offline, rate-limit and session-expired states;
- route/role matrix for Public, User, Pharmacist, Expert and Admin;
- table-to-list, warning priority, sticky-action and 200% zoom rules.

## Security and authorization contract

- Authentication determines the role server-side and redirects to the matching Area.
- Users do not choose a role on the login form.
- Sample email addresses and passwords are absent from UI, HTML, JavaScript and tooltips.
- Demo credentials may exist only in internal testing documentation.
- Area controllers enforce role authorization even though normal navigation never exposes other Areas.
- Direct unauthorized URLs return the access-denied response without leaking the target module.
- Shared domain services remain role-agnostic; Area controllers and layouts own presentation and route
  boundaries.

## Implementation gate

- `N4WTT-210`, `N4WTT-219`, `N4WTT-220`, `N4WTT-228` and `N4WTT-229` may start only after Nam
  records `APPROVED` against the exact file key and Manual V1 candidate.
- Visual changes after approval require a Jira-linked change request and a new Figma candidate label.
- Vũ validates desktop/mobile output against these frames, not against the old shared-shell UI.
