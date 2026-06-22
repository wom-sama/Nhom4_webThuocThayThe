# S11 production UI usability audit

Jira: `N4WTT-243`, `N4WTT-249`

Production baseline: <https://nnhom4web.somee.com>

Audit date: 22/06/2026

## Method

- Direct production inspection at `1440 x 900` and `390 x 844`.
- DOM and accessibility-tree review for public home, search results and out-of-stock drug detail.
- Comparison with Figma Manual V1, S8 visual QA and Vietnamese content rules.
- Read-only checks only; no production data was modified.

## What is already strong

- Public navigation and task labels are understandable in Vietnamese.
- Desktop home and search results have no horizontal overflow.
- Search, stock status, prescription status and price are visible without opening admin features.
- Role-based Areas remain isolated and the public shell does not expose management navigation.
- The out-of-stock detail page clearly shows deterministic scores, reasons, stock and warnings.
- Demo credentials are absent from the production UI.

## Usability findings

### P1 - Home search scope looks interactive but is disabled

The home page renders a disabled `Phạm vi` select with a chevron and filled-control styling. Users can
reasonably try to open it even though it has no behavior. V2 must either provide a working category
filter or replace the control with non-interactive scope text.

### P1 - Mobile detail delays the primary decision

At `390 x 844`, the complete pharmacology card appears before stock and alternatives. A user looking
for a substitute must scroll through description, usage and contraindication before seeing that the
medicine is out of stock. V2 must place a compact availability/decision summary directly below the
title, with pharmacology details in a secondary disclosure or later section.

### P1 - Recommendation cards are vertically repetitive

Every candidate repeats score, metadata, five reasons, warnings and an AI button. On mobile this
creates a long page before the second candidate. V2 should show the top three decision signals first:
stock, safety and deterministic match. Detailed reasons should use progressive disclosure while
warnings remain visible.

### P1 - Role dashboards need task-oriented quick actions

S8 separated the role shells correctly, but dashboard content is still mostly metrics plus text links.
V2 should give each role a compact first-action row:

- User: recent search, saved medicines, stock-change notice.
- Pharmacist: out-of-stock queue, compare substitute, inventory risk.
- Expert: pending review, evidence summary, decision history.
- Admin: catalogue change, stock exception, source status, backup/audit.

### P2 - Visual recognition depends too heavily on text

Navigation, search, stock and common actions use almost no icons. V2 should add a restrained icon set
for familiar actions without turning the interface into a marketing page. Icon-only controls require
tooltips and accessible names.

### P2 - Information hierarchy is flat

Metric cards, workflow cards and recommendation cards use similar borders and surface treatment. V2
should distinguish primary decisions, secondary context and repeated items with spacing, typography
and semantic status color rather than more shadows or nested cards.

### P2 - Interactive states need a single contract

Loading, empty, service-error, offline, session-expired, rate-limit and AI fallback states exist in
different forms or only in test descriptions. V2 must define one state component family and reuse it
across public and role Areas.

## Figma V2 scope

The existing three-page Figma file remains the source of truth because the Free plan page limit is
already reached. V2 is added as versioned sections inside existing pages:

1. `01 Foundations & Role Architecture`: V2 tokens, icon rules, state components and usability notes.
2. `02 Desktop Role Areas`: V2 public home/search, pharmacist detail and role quick-action dashboards.
3. `03 Mobile & State Matrix`: V2 mobile detail decision-first flow and reusable state matrix.

## Acceptance criteria

- No disabled control that visually implies available interaction.
- Stock and substitute decision visible within the first mobile viewport for an out-of-stock drug.
- No horizontal overflow at `320`, `390`, `768`, `1280` and `1440` CSS pixels.
- Minimum `44 x 44` touch target for primary mobile actions.
- Keyboard focus, accessible names and `vi-VN` labels verified.
- Demo credentials remain absent from UI and browser assets.
- AI remains optional and cannot hide score, stock, warnings or professional-review boundaries.
