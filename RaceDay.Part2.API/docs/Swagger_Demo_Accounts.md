# Swagger demonstration accounts

Use these only for local assessment demonstration. The seeded password is `Password123!`.

- Organiser: `thabo.organiser@raceday.test`
- Organiser: `lerato.organiser@raceday.test`
- Participant: `naledi.participant@raceday.test`
- Participant: `kabelo.participant@raceday.test`

Recommended demo order:
1. `POST /api/auth/login` as Organiser.
2. `GET /api/profile`.
3. `POST /api/events`.
4. `POST /api/events/{eventId}/categories`.
5. `GET /api/events/{eventId}/enrolments`.
6. Log out.
7. Log in as Participant.
8. `GET /api/events` and `GET /api/categories`.
9. `POST /api/events/{eventId}/enrolments`.
10. `GET /api/enrolments`.
11. Demonstrate that the Participant receives 403 from an Organiser-only endpoint.
12. As Organiser, create a result and then show the Participant's own result.
