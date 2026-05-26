# Sprint S0,5 - Front-end Template Notes

## Scope

- Build a reusable Razor layout shell for the drug substitution web app.
- Standardize design tokens, spacing, surfaces, status badges, tables, cards, action bars and filter panels.
- Prioritize the real workflows already implemented in S1: public drug search, drug detail, catalog administration and inventory administration.

## Interface Direction

- Tone: clinical, operational and compact.
- Navigation: top application header with role-aware links.
- Surfaces: use bordered panels for forms and repeated result cards only.
- Status language: inventory availability, prescription warning and active/inactive catalog state must be visible without opening each record.
- Responsive rule: search filters, metric cards, workflow panels and detail grids collapse to one column on mobile.

## Initial Screen List

| Screen | Purpose | Sprint Ticket |
| --- | --- | --- |
| Dashboard | Quick search and system status summary | N4WTT-24 |
| Drug search | Public search by keyword/category | N4WTT-26 |
| Catalog admin | Manage drug master data | N4WTT-28 |
| Shared components | Layout, cards, badges, forms, tables | N4WTT-30 |

## Review Checklist

- Text fits on mobile without overlapping.
- Keyboard focus is visible on links, buttons and form controls.
- Empty and no-result states are explicit.
- Role-only links remain hidden from unauthenticated users.
- Tables remain horizontally scrollable when columns exceed mobile width.
