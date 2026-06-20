# Scrum Roles And Working Agreement

## Team Roles

| Thanh vien | Vai tro | Trach nhiem |
|---|---|---|
| Nam | Lead, Product Owner, Scrum Master, reviewer | Product vision, backlog priority, architecture decision, PR review, sprint acceptance va release |
| Tan | Backend Developer | Search, recommendation engine, safety rules va external data workflow |
| Tu | Frontend/UI Developer | Search/detail/dashboard UI, responsive behavior va accessibility |
| Vu | QC Tester | Test execution, bug report, regression va UAT evidence tren Jira |
| Dat | Full-stack/DevOps | Reporting, audit, backup, automated tests, build va performance verification |

Vu chua co GitHub PAT nen chi thao tac Jira va test tren increment duoc cung cap.

## Git Workflow

1. Lay issue tu Sprint Backlog.
2. Chuyen issue/sub-task sang In Progress.
3. Tao `feature/N4WTT-<keys>-<scope>` tu `develop`.
4. Commit phai chua Jira key.
5. Mo pull request vao `develop`.
6. Nam review code; QC xac nhan test result.
7. Merge vao `develop`.
8. Ket thuc sprint moi merge release vao `main`.

## Definition Of Ready

- User Story co actor, muc tieu va gia tri.
- Co acceptance criteria co the test.
- Co design/UI note neu thay doi giao dien.
- Da xac dinh dependency va du lieu test.
- Co estimate va assignee.

## Definition Of Done

- Code build khong warning/error.
- Acceptance criteria dat.
- Automated test lien quan da them/cap nhat.
- Acceptance, security va performance suite dat.
- QC khong con bug blocker/critical.
- PR da duoc Nam review.
- Jira issue co comment ket qua, test evidence va commit/PR.
- Increment duoc merge vao `develop`; release sprint duoc merge vao `main`.

## Jira Evidence

Board hien chi co `To Do`, `In Progress`, `Done`. Cac buoc `Code Review` va `Testing` duoc the hien bang sub-task rieng, label va comment tren Jira de giu dau vet quy trinh.
