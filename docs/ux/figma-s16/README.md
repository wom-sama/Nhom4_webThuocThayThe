# S16 Figma-ready production hardening handoff

Target Figma source of truth:
<https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design>

Jira sprint: `S16 Prod Hardening` (`N4WTT-260` to `N4WTT-265`).

These boards are created before implementation so the S16 code changes can be traced back to a
design-first artifact. They are sized for import into the existing Figma file as a new section named
`S16 Production hardening`.

| Board | Purpose |
| --- | --- |
| `01-expert-role-flows.svg` | separates Pending review, Evidence, and Decision history screens |
| `02-data-ai-policy-states.svg` | richer data states, role AI assistant panels, privacy policy sections |

## Design decisions

- Expert navigation entries must not share the same screen. Each item has a distinct task and visual
  hierarchy.
- Evidence is read-only and explains the deterministic score, stock signal, safety signal, and review
  risk. Editing remains only in Pending review.
- Decision history is a completed-review audit surface, not another editable form.
- Role AI support is deterministic and explainable. It produces suggestions from stock, score, safety
  and workflow data; Gemini explanation remains optional and bounded.
- Privacy policy is a real operational page with sections for data scope, role access, AI boundary,
  retention, user responsibility, incident response, and contact.
- Dense operational UI is preferred over placeholder copy. Cards must contain actionable data or a
  direct route.

## Instructor-standard traceability

- Scrum/Jira: `N4WTT-264` records this design handoff before implementation.
- UI/UX: wireframe, user flow, mockup states and design system tokens are represented in SVG boards.
- Testing: `N4WTT-265` requires acceptance, security, performance, browser UAT and production
  validation after deployment.
- Release: production increment must be tagged and deployed to Somee with a QA report.
