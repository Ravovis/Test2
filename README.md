# Team Task Manager

Mini Jira-style app for the interview.

- Frontend: Angular 17 (standalone, SSR-ready)
- Backend: .NET 9 Web API
- Storage: In-memory (no DB)

## Prerequisites
- Node.js 18+ (20+ recommended)
- npm 9+
- .NET SDK 9.0+

## Backend
```bash
cd back/WebApplication1/WebApplication1
# run in dev (seeds 4 users, 8 tasks)
dotnet run
```
- Swagger available in Development.
- API base printed in console (e.g. https://localhost:7042).

Endpoints (base `/api`):
- Tasks: GET `/tasks`, GET `/tasks/{id}`, POST `/tasks`, PUT `/tasks/{id}`, DELETE `/tasks/{id}`
- Dashboard: GET `/tasks/stats`, GET `/tasks/activities?limit=5`
- Users: GET `/users`, POST `/users`, PUT `/users/{id}`, DELETE `/users/{id}` (blocked if user has tasks)

Notes:
- Status: `ToDo|InProgress|Done`; Priority: `Low|Medium|High`.
- `POST/PUT` tasks accept optional `userId` query (defaults to `4`) for activity logging.

## Frontend
```bash
cd front/frontApp
npm install
npm start       # dev
# or production build
npm run build
```
- Currently uses in-memory services and runs standalone.
- To use the API: switch services to HttpClient and map enums/dates accordingly.

## What’s Implemented
- 5 pages: Dashboard, Task List, Create Task, Task Detail, User Management
- CRUD for tasks and users, validation, stats, recent activity
- Prevent deleting users with assigned tasks

## Troubleshooting
- Angular CSS budgets are configured in `front/frontApp/angular.json` (production section).
- Enum ambiguity in backend resolved by qualifying our `TaskStatus` usages.

## License
For interview usage.


By default website should be at http://localhost:4200/dashboard when running
