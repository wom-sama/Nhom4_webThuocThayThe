# Sprint S1,5 - Manual Review Checklist

## UI Review

- Header remains readable on mobile and desktop.
- Drug cards do not overlap when text is long.
- Inventory tables remain horizontally scrollable on small screens.
- Login and access-denied pages provide clear recovery paths.

## Browser Paths

| Path | Expected |
| --- | --- |
| `/` | Dashboard and quick search visible |
| `/Drugs?keyword=para` | Paracetamol results visible |
| `/Drugs/Details/1` | Panadol detail and Paracetamol DHG alternative visible |
| `/Auth/Login` | Login form and sample accounts visible |
| `/Inventory` after pharmacist login | Stock metrics and batch table visible |
| `/DrugCatalog` after normal user login | Access denied |

## Evidence To Capture Later

- Desktop screenshot of dashboard.
- Mobile screenshot of search results.
- Inventory screenshot after adding a near-expiry batch.
- Access denied screenshot with a normal user.
