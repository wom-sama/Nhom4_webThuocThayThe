# S11 Figma V2 handoff

Figma source of truth:
<https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design>

Jira: `N4WTT-243`, `N4WTT-244`, `N4WTT-248`, `N4WTT-249`

The Free plan already uses all three pages, so S11 adds a `V2 - S11 Usability Upgrade` section to
each existing page. The SVG files in this directory are reviewable import sources, not the production
frontend.

| Existing Figma page | S11 section source | Purpose |
| --- | --- | --- |
| `01 Foundations & Role Architecture` | `01-foundations-v2.svg` | tokens, icons, state family and UX rules |
| `02 Desktop Role Areas` | `02-desktop-role-areas-v2.svg` | public plus four distinct role workspaces |
| `03 Mobile & State Matrix` | `03-mobile-decision-states-v2.svg` | decision-first mobile flow and failure states |

## Imported nodes

Imported and verified on 22/06/2026:

- Foundations V2: [node `6:129`](https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design?node-id=6-129)
- Desktop role areas V2: [node `6:255`](https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design?node-id=6-255)
- Mobile and state matrix V2: [node `6:417`](https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design?node-id=6-417)

All three nodes are positioned in a separate canvas region on their corresponding existing page. The
Version 1 frames were not overwritten or moved.

## Design decisions

- Public search has a working category control; no disabled control that looks interactive.
- An out-of-stock decision summary appears before long pharmacology content on mobile.
- Recommendation cards expose stock, safety and match first; detailed reasons are progressive.
- User, Pharmacist, Expert and Admin each receive role-specific navigation and quick actions.
- AI explanation is optional and never hides deterministic score, stock, warnings or professional review.
- Demo credentials are absent from every frame and component.

## Gate

Implementation ticket `N4WTT-244` may enter In Progress only after Nam records `APPROVED` on
`N4WTT-248` with the exact Figma file and imported V2 sections.
