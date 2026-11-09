
## Project purpose
This project implements the RaceDay event management system planned in Part 1. Part 2 turns the approved planning artefacts into a working ASP.NET Core Web API backed by SQL Server through Entity Framework Core Code-First.

## What was carried forward from Part 1
The Part 1 ERD, endpoint plan and database design remain the specification. The implementation keeps the planned route structure for authentication, profiles, events, categories, enrolments and results.

## Part 1 corrections and implementation notes
The planning work was retained, but the following issues were corrected or clarified when moving to implementation:

1. **REST API implementation:** Part 1 contained the endpoint plan rather than a working API. Part 2 now exposes the planned endpoints through ASP.NET Core Web API and Swagger.
2. **Event details:** The implementation includes the Part 2-required `EventType` (Run, Walk or Cycle) and `Location`/distance fields so every event captures the information required by the brief.
3. **Password storage:** Part 1 contained placeholder seed values such as `HASHED_PASSWORD_01`. These were not treated as real passwords. Part 2 hashes passwords with PBKDF2-SHA256 before storage and verifies hashes during login.
4. **Role enforcement:** Part 2 applies server-side session role checks so Organiser-only and Participant-only endpoints cannot be used by the wrong role.
5. **Database consistency:** The EF Core model and the SSMS reference script were aligned so the implemented database relationships and constraints match the planning model while satisfying the Part 2 functional requirements.

## Technology
- ASP.NET Core Web API (.NET 8)
- C#
- Entity Framework Core Code-First
- SQL Server / LocalDB
- Swagger / OpenAPI
- Server-side ASP.NET Core Session
- PBKDF2-SHA256 password hashing
- xUnit integration tests
- GitHub Actions CI

## Roles
### Organiser
Organisers can create, update and delete their own events, attach categories, view enrolments for their events, and record/correct results.

### Participant
Participants can view events and categories, maintain their own profile, enrol in an event category, cancel their own enrolment before the deadline, and view their own results.

## Run in Visual Studio
1. Install **Visual Studio 2022/2026** with the **ASP.NET and web development** workload.
2. Open `RaceDay.Part2.sln`.
3. Ensure SQL Server LocalDB is installed. The default connection string targets `(localdb)\\MSSQLLocalDB`.
4. Press **F5** or **Ctrl+F5**.
5. Swagger should open automatically at `/swagger`.
6. The first run creates `RaceDayDB_Part2` using EF Core Code-First.

### Seeded demonstration accounts
All seeded accounts use the same demonstration password:

`Password123!`

Organisers:
- `thabo.organiser@raceday.test`
- `lerato.organiser@raceday.test`

Participants:
- `naledi.participant@raceday.test`
- `kabelo.participant@raceday.test`

These are demonstration credentials only. They should not be used as real production credentials.

## Swagger testing flow
1. Open `/swagger`.
2. Use `POST /api/auth/login` with a seeded account.
3. The API stores the authenticated user ID and role in the server-side session cookie.
4. Continue testing protected endpoints in the same browser session.
5. Use `POST /api/auth/logout` to end the session.

## Database
The production API uses EF Core Code-First with the connection string in `appsettings.json`. The `docs/RaceDay_Database.sql` file is a reference SQL Server script for SSMS verification.

## Unit tests
Run from the solution directory:

```bash
dotnet test Tests/RaceDay.Tests.csproj
```

The tests use an EF Core InMemory database so they do not modify the LocalDB development database. Coverage includes registration, invalid login, authenticated profile access, organiser event management, participant enrolment, role rejection and Swagger availability.

## GitHub Actions
The workflow is stored in `.github/workflows/raceday-ci.yml`. It restores the solution, builds the project and runs the unit tests on pushes and pull requests.

> The green-build screenshot and YouTube link should be added after the repository workflow has successfully run and the demonstration video has been recorded.
