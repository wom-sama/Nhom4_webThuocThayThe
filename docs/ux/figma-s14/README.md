# S14 Figma-ready UI V3 handoff

Target Figma source of truth:
<https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design>

Jira: `N4WTT-256`, implementation gate `N4WTT-257`, release gate `N4WTT-259`.

The SVG files in this directory are the design source for the S14 UI V3 implementation. They are
created and committed before Razor/CSS changes, and are sized for direct import into the existing
Figma file as a new `V3 - S14 Professional Bootstrap UI` section.

| Board | Purpose |
| --- | --- |
| `01-foundations-components-v3.svg` | color, type, spacing, elevation, buttons, badges, inputs and states |
| `02-public-flow-v3.svg` | home/search, result list, drug decision detail and login |
| `03-role-mobile-v3.svg` | Admin/Pharmacist/Expert/User shells plus mobile responsive contract |

## Design direction

- Keep Bootstrap 5 as the component and responsive foundation; do not mix Tailwind into the same
  rendering pipeline.
- Use a neutral canvas and four semantic accents: teal for primary actions, blue for information,
  green for available/safe, amber/red for risk.
- Keep the public search as the first-screen task. Do not add a marketing hero or instructional copy.
- Present search results as dense decision rows with stock, price and one clear detail action.
- On drug detail, show availability and top recommendation before pharmacology. Collapse repeated
  reasons and keep AI explanation secondary to rule score and safety.
- Role dashboards use compact side navigation, task groups, metrics and recent activity. Avoid nested
  cards and decorative gradients.
- Demo credentials never appear in design or production UI.

## Bootstrap mapping

| Design component | Bootstrap implementation |
| --- | --- |
| Search command bar | `input-group`, responsive grid, `form-control`, `form-select` |
| Status/score | `badge`, semantic custom tokens, `progress` where useful |
| Result/recommendation | `list-group`, `card`, responsive flex utilities |
| Detail disclosures | `accordion` / native `details` with Bootstrap styling |
| Role navigation | responsive sidebar + Bootstrap offcanvas on mobile |
| Feedback | `alert`, validation summary, loading and error states |

## Acceptance gate

- Imported frames preserve 12-column alignment and do not overlap at 1440, 1024, 768 or 390 px.
- Home, search, detail, login and all four role dashboards have desktop and mobile contracts.
- Keyboard focus is visible; controls have labels; icon-only controls have tooltips.
- Implementation may start only after this directory is committed and linked from `N4WTT-256`.

