# S17 Figma-Ready Motion Polish

Sprint: S17 Motion Security Audit
Jira: N4WTT-266, N4WTT-267
Date: 2026-06-23

## Objective

Add a restrained motion layer that makes the application feel more professional without making the medicine decision workflow distracting or inaccessible.

## Motion Principles

- Functional first: motion confirms navigation, focus, hover and state changes.
- Short duration: most transitions are 120-220ms.
- Small distance: entry motion uses 6-14px translation only.
- Reduced motion safe: all decorative animation is disabled when `prefers-reduced-motion: reduce`.
- Production-safe: no heavy JavaScript animation dependency, no layout-thrashing loops.

## Figma Frames To Recreate

1. `Motion Tokens`
   - Duration: `120ms`, `160ms`, `220ms`, `420ms`.
   - Easing: `cubic-bezier(.2,.8,.2,1)` and `ease-out`.
   - Elevation: soft hover lift for cards, stronger active focus outline for forms.

2. `Public Search Flow`
   - Header fades/slides in once.
   - Search panel has soft surface shadow and focus ring.
   - Result cards lift on hover and use an accent line.

3. `Role Workspaces`
   - Sidebar nav items slide accent on hover/active.
   - Quick actions lift and reveal arrow motion.
   - Insight cards use a subtle shimmer band on the top edge.

4. `Decision States`
   - Status badges pulse only for high-priority live states.
   - Review, evidence, queue and comparison cards use stagger-friendly entry classes.
   - Buttons show clear pressed/focus feedback.

5. `Reduced Motion`
   - All transforms and keyframes disabled.
   - Focus outlines and color changes remain.

## Acceptance Criteria

- No horizontal overflow on desktop or mobile.
- No infinite movement except a tiny live-dot pulse; pulse disabled with reduced motion.
- Existing Bootstrap/Lucide stack remains.
- Browser QA checks public, Expert, Pharmacist and Privacy screens.
