# Figma-first product design brief

Jira epic: `N4WTT-206`

The design source of truth is a versioned Figma file. Repository documents define requirements and
acceptance criteria; they do not replace the approved Figma frames.

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

- [ ] Current audit and user flows reviewed.
- [ ] Low-fi information architecture approved.
- [ ] Vietnamese content/glossary approved.
- [ ] Foundations and component variants complete.
- [ ] Desktop and mobile frames complete.
- [ ] Loading, empty, error, disabled and permission states complete.
- [ ] Prototype covers the primary and role-specific flows.
- [ ] Contrast, keyboard order and touch targets reviewed.
- [ ] Nam records `APPROVED` with the Figma version URL in Jira.

After approval, Tân implements only the locked frames and variables. Changes require a Jira change
request; Vũ validates the build against the same Figma version.
