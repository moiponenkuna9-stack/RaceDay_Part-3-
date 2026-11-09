# RaceDay Part 2 tests

The test project uses an EF Core InMemory database so tests are repeatable and do not require a local SQL Server instance. The production API continues to use SQL Server through `RaceDayConnection`.

Coverage includes registration, invalid login, authenticated profile access, organiser event management, participant enrolment, cross-role rejection, and Swagger availability.
