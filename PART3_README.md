# RaceDay Part 3 – MVC and API solution

This package retains the original Part 2 REST API and its tests, and adds a separate ASP.NET Core MVC frontend (`RaceDay.Web`).

## Run in Visual Studio 2022

1. Install the .NET 8 SDK and SQL Server LocalDB or SQL Server Express.
2. Open `RaceDay.Part2.sln`. Restore NuGet packages.
3. Check the API `appsettings.json` connection string and follow `docs/LOCAL_SETUP_FIRST.md`.
4. Right-click the solution > Properties > Startup Project > **Multiple startup projects**; select **Start** for both `RaceDay.Part2` and `RaceDay.Web`.
5. Run (F5). API Swagger: `http://localhost:5100/swagger`; MVC site: `http://localhost:5200`.
6. Register an Organiser and a Participant, sign in, and try the dashboards.

The MVC project uses HttpClient to call the API; it has no EF Core dependency. The API session cookie is forwarded by the MVC server per logged-in browser session.

## Docker

Dockerfiles and a compose file are included. Set `RACEDAY_SQL_CONNECTION` to a SQL Server connection string reachable **from inside the API container** before running `docker compose up --build`. Windows LocalDB is not accessible from Linux containers. Docker configuration has not been integration-tested.

## Current implementation scope and remaining requirements

Implemented as source: separate MVC frontend, responsive site, account registration/login/logout, role-based dashboards, event discovery/search, event creation/deletion, enrolment/cancellation, results display/publication, and profile update.

**Still to implement or validate before submission:** full event editing UI, category management UI, Azure Blob Storage uploads, API-backed image URL persistence, hardened authentication/authorization across all endpoints, integration tests, deployment, and actual GitHub commits and video demonstration. Do not claim these are complete. External SQL Server and Azure configuration are required.

This environment did not have the .NET SDK installed; compilation and end-to-end runtime tests could not be performed here. Build and verify in Visual Studio before submitting.
