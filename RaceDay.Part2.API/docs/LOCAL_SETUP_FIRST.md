# RaceDay Part 2 — run locally in Visual Studio

1. Extract the ZIP completely. Open `RaceDay.Part2.sln` (not `Program.cs`, a folder, or a test source file).
2. In Visual Studio Installer, install **ASP.NET and web development** and **Data storage and processing** (including SQL Server Express LocalDB). Use Visual Studio 2022 17.8+ for .NET 8.
3. Set `RaceDay.Part2` as startup project. Select the **http** launch profile and press F5.
4. Swagger: http://localhost:5168/swagger . Check http://localhost:5168/api/system/database-status for database connectivity.
5. The default connection in `appsettings.json` is `(localdb)\\MSSQLLocalDB`, database `RaceDayDB_Part2`. If LocalDB is unavailable, install it with Visual Studio Installer (Individual components: SQL Server Express LocalDB) or set `ConnectionStrings:RaceDayConnection` to your installed SQL Server instance. No Azure/publishing required.
6. Swagger can display without SQL Server, but data endpoints require a working SQL Server connection. Database status reports a clear error instead of a blank console.
7. For local demo only, organiser: thabo.organiser@raceday.test / Password123!; participant: naledi.participant@raceday.test / Password123! . Change/remove demo accounts before real deployment.
8. Build > Rebuild Solution; Test > Run All Tests. GitHub Actions will also build and test the solution.

If Visual Studio shows duplicate assembly attributes, close the solution, delete `bin` and `obj` folders inside the extracted project and its `Tests` folder, reopen the `.sln`, and rebuild. The main project excludes `Tests/**/*.cs` from compilation.
