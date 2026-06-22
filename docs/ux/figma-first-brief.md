# Figma-first product design brief

Jira epic: `N4WTT-206`

The design source of truth is a versioned Figma file. Repository documents define requirements and
acceptance criteria; they do not replace the approved Figma frames.

## Design candidate under review

- Figma Design file: [N4WTT – S8 UI Design](https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design)
- File key: `fu43wNGxTFRMghHrlH9lhI`
- Authoring method: manual Figma Design; no Figma AI credits or Make generation.
- Owner approval: pending `N4WTT-212`; this candidate does not authorize implementation yet.
- Interactive reference only: [Pharmacy Web App UI Design](https://www.figma.com/make/UOQ4LcUQnhG6yArGXS9aYS/Pharmacy-Web-App-UI-Design)
- Repository handoff: `docs/ux/figma-s8/`

The Free plan permits three pages, so the original ten-page information architecture is consolidated
without dropping required content:

1. `01 Foundations & Role Architecture`: cover, changelog, role routing, foundations, components and
   accessibility contract.
2. `02 Desktop Role Areas`: Public, User, Pharmacist, Expert, Admin and Auth desktop frames.
3. `03 Mobile & State Matrix`: six mobile role/auth frames, async states, route matrix and responsive
   rules.

## Product intent

Help a user find a safe, in-stock medicine-substitution candidate when the requested medicine is
unavailable, while keeping deterministic scoring, safety warnings and pharmacist/expert review
authoritative.

## Primary users

| User | Primary job | UX priority |
| --- | --- | --- |
| Public user | Search and understand availability | Fast, plain-language, no management noise |
| Pharmacist | Check stock and compare substitutes | Dense but scannable operational information |
| Expert | Review and record a recommendation decision | Evidence, warnings and auditability |
| Admin | Maintain catalogue, sources and reports | Efficient repeated actions and safe forms |

## Required Figma pages

1. `00 Cover & changelog`
2. `01 Current audit`
3. `02 Foundations`
4. `03 Components`
5. `04 User flows`
6. `05 Low-fi wireframes`
7. `06 Hi-fi desktop`
8. `07 Hi-fi mobile`
9. `08 Prototype`
10. `09 Dev handoff`

The Figma version URL and review date must be recorded in `N4WTT-212`. Implementation tickets remain
blocked until Nam comments `APPROVED` with the exact version link.

## Required frames

Create desktop `1440 x 900` and mobile `390 x 844` frames for:

- Public home/search entry.
- Search results, filters, no-result and error states.
- Drug detail with stock, safety and substitute recommendations.
- AI explanation: idle, loading, success, fallback, rate-limit and error.
- Login, access denied and session state.
- Drug catalogue list/create/edit.
- Inventory list and create batch.
- Expert review queue and decision form.
- Reports, source status and backup metadata.

## Core flows

### Public substitution flow

`Home -> Search -> Result -> Drug detail -> Compare candidates -> Optional AI explanation`

The user must see out-of-stock status, candidate stock and safety warnings before optional AI content.

### Pharmacist flow

`Login -> Inventory risk -> Drug detail -> Compare candidate -> Record/hand off decision`

### Expert flow

`Login -> Review queue -> Inspect deterministic evidence -> Approve/reject -> Audit confirmation`

### Admin flow

`Login -> Catalogue/inventory/source -> Create or edit -> Validation -> Success/audit feedback`

## Role-specific Area contract

The application must not expose one global management interface and rely on `AccessDenied` for normal
navigation. Authenticated users land directly in a role-specific ASP.NET Core Area:

| Experience | Route root | Own navigation |
| --- | --- | --- |
| Public | `/` and `/Drugs` | Search, medicine detail, safety information, login |
| Standard user | `/User` | Personal overview, history, saved medicines, profile |
| Pharmacist | `/Pharmacist` | Search workspace, substitutions, stock risk, consultation history |
| Medical expert | `/Expert` | Review queue, evidence, decisions, audit history |
| Administrator | `/Admin` | System overview, catalogue, inventory, users/roles, sources, logs/backups |

Shared components and domain services are expected; shared navigation and dashboards are not. After
authentication the server redirects by the persisted role. `AccessDenied` remains an edge-case response
for a direct URL that violates authorization, not a primary workflow.

The login UI must never display sample email addresses or passwords. Demo accounts belong only in
internal testing documentation and must not appear in HTML, JavaScript, tooltips, page source or
production configuration rendered to the client.

## Foundations direction

These are starting constraints, not final tokens. Tú owns the final Figma variables after review.

### Typography

- Preferred UI family: `Be Vietnam Pro`, with complete Vietnamese glyph support.
- Implementation must self-host the approved font or use a reviewed system fallback; no runtime CDN
  dependency is required for the demo host.
- Use a compact operational scale. Reserve large type for page-level headings only.
- Letter spacing remains `0`; do not scale font size by viewport width.

### Semantic color

- Neutral canvas/surface/text are the majority of the interface.
- Teal is the primary action/brand color, not the color of every surface.
- Blue: information and AI provenance.
- Green: available/success.
- Amber: caution/expiring/needs review.
- Red: out of stock, danger or destructive action.
- Every status must include text or icon, never color alone.

### Shape and spacing

- Component radius: 6-8 px maximum.
- Use unframed page sections; cards only for repeated entities or genuinely bounded tools.
- Do not place recommendation cards inside another decorative card.
- Define stable input, button, badge, metric and table dimensions.

### Icons

- Use Lucide icons during implementation, paired with text for high-risk or unfamiliar actions.
- Back, search, filter, stock, warning, AI provenance, edit and download need consistent symbols.
- Every icon-only button needs an accessible label and tooltip when meaning is not universal.

## Component inventory

- App shell and role-aware navigation.
- Search input, filter controls and active-filter summary.
- Primary/secondary/tertiary/icon buttons.
- Form field with help, validation, disabled and loading states.
- Stock/status badge with icon and text.
- Drug result row/card with stable dimensions.
- Recommendation comparison row with score, stock, reasons and warning priority.
- AI disclosure/action/panel with all async states.
- Operational table with desktop and mobile behavior.
- Empty, loading, error, permission and offline states.
- Toast/inline confirmation and destructive confirmation dialog.

## Home-page direction

- Make search and current availability the first task.
- Remove the fake disabled scope control unless it becomes functional.
- Show management workflows only after login and order them by role.
- Reduce equal-weight cards; use a compact metric band and a prioritized work list.

## Drug-detail direction

- Show name, prescription status, stock and primary safety warning together.
- On mobile, place availability and substitute summary before long pharmacology details.
- Use a compact comparison structure for candidates rather than repeating full paragraphs.
- Treat AI explanation as optional tertiary assistance with clear provenance and limitations.

## Design gate

No frontend implementation begins until all items are checked:

- [x] Current audit and user flows reviewed.
- [x] Low-fi information and role architecture complete.
- [x] Vietnamese content/glossary approved.
- [x] Foundations and component variants complete.
- [x] Desktop and mobile role-specific frames complete.
- [x] Loading, empty, error, offline, rate-limit and session states complete.
- [x] Primary and role-specific flows are covered by Design frames plus the interactive reference.
- [x] Contrast, keyboard order and minimum 44 px touch targets reviewed.
- [x] Login frames contain no sample account or password information.
- [ ] Nam records `APPROVED` with the Figma version URL in Jira.

After approval, Tân implements only the locked frames and variables. Changes require a Jira change
request; Vũ validates the build against the same Figma version.
