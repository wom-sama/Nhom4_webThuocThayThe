# S18 Figma-ready production reconciliation

Target Figma source of truth:
<https://www.figma.com/design/fu43wNGxTFRMghHrlH9lhI/N4WTT-%E2%80%93-S8-UI-Design>

Date: 2026-06-23
Production: <https://nnhom4web.somee.com>

## Purpose

This folder records the Figma update required after the production mobile/login hotfix. The SVG artefact was also pasted into the signed-in Figma board on 2026-06-23 as a production reconciliation frame.

## Frames to import or update

1. `S18 Mobile Login - No stale CSRF`
   - Login page has no instructional chips under the headline.
   - Heading wraps safely on 390 px mobile width.
   - Form remains first actionable control after the trust panel.

2. `S18 Role Mobile Navigation`
   - Role sidebar becomes a compact bottom navigation on mobile.
   - Items use short labels, icon-first layout and horizontal scroll.
   - Navigation has safe-area padding and no text overflow.

3. `S18 Production Fingerprint`
   - Somee CSS contains `scroll-snap-type: x proximity`.
   - Login page response contains `Cache-Control: no-store,no-cache`.
   - Anti-forgery cookie contains `secure; samesite=lax; httponly`.

## Implementation mapping

| Design item | Production implementation |
| --- | --- |
| Mobile login title wrap | `wwwroot/css/site.css` `.auth-trust-panel .page-title` |
| Login no-cache behavior | `Controllers/AuthController.cs` `ResponseCache(NoStore = true)` |
| Anti-forgery cookie hardening | `Program.cs` `AddAntiforgery` cookie settings |
| Bottom role navigation | `wwwroot/css/site.css` mobile `.role-nav` rules |
| Visible mobile role logout | Role area shared layouts plus `wwwroot/css/site.css` `.role-mobile-logout` |

## Acceptance evidence

- Deployment completed through `scripts/Publish-Somee.ps1` and `scripts/Deploy-Somee.ps1`.
- Latest production smoke: `/health` returns `healthy/database connected`.
- Latest CSS smoke: production `site.css` contains the mobile navigation and auth wrapping fingerprint.
- Browser production smoke verified mobile login, visible mobile logout, protected-route redirects and Gemini explanation output.
