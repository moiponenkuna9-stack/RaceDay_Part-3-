# RaceDay API Endpoint Plan

## Purpose
This planning document defines the REST endpoints that the RaceDay system is expected to expose in Part 2. It is designed before API implementation and aligns with the database model in `RaceDay_ERD.png` and `RaceDay_Database.sql`.

| HTTP Method | Route | Description | Role Required | Request Body (if any) | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/auth/register` | Creates a new participant or organiser account after validating the email and role. | Public | `firstName, lastName, email, password, role` | 201 Created with userId/token; 400 validation; 409 duplicate email. |
| POST | `/api/auth/login` | Authenticates a user and returns an access token. | Public | `email, password` | 200 OK token/user; 401 invalid credentials. |
| GET | `/api/profile` | Returns the logged-in user's profile. | Logged-in | None | 200 OK profile; 401 unauthorised. |
| PUT | `/api/profile` | Updates the logged-in user's profile and contact details. | Logged-in | Profile fields | 200 OK updated profile; 400 validation. |
| GET | `/api/events` | Lists available events with optional filters. | Public | Query parameters: `date`, `status` | 200 OK event collection. |
| GET | `/api/events/{id}` | Returns one event and its categories. | Public | None | 200 OK event; 404 not found. |
| POST | `/api/events` | Creates a race event for the logged-in organiser. | Organiser | `name, description, venue, eventDate, registrationOpen, registrationClose, status` | 201 Created; 400 validation; 403 forbidden. |
| PUT | `/api/events/{id}` | Updates an event owned by the organiser. | Organiser | Editable event fields | 200 OK; 403 forbidden; 404 not found. |
| DELETE | `/api/events/{id}` | Deletes an event that has not started and is owned by the organiser. | Organiser | None | 204 No Content; 403 forbidden; 404 not found; 409 conflict. |
| GET | `/api/categories` | Lists all race categories. | Public | None | 200 OK category collection. |
| POST | `/api/events/{eventId}/categories` | Adds a category to an event. | Organiser | `categoryId, entryFee` | 201 Created; 404 not found; 409 duplicate. |
| DELETE | `/api/events/{eventId}/categories/{categoryId}` | Removes a category when it has no enrolments. | Organiser | None | 204 No Content; 409 conflict. |
| POST | `/api/events/{eventId}/enrolments` | Enrols the logged-in participant in an event category. | Participant | `categoryId` | 201 Created with race number; 400 invalid; 409 closed/full/duplicate. |
| GET | `/api/enrolments` | Returns the logged-in participant's enrolments. | Participant | None | 200 OK enrolment collection. |
| GET | `/api/events/{eventId}/enrolments` | Returns enrolments for an organiser's event. | Organiser | None | 200 OK; 403 forbidden; 404 not found. |
| DELETE | `/api/enrolments/{id}` | Cancels a participant enrolment before the deadline. | Participant | None | 204 No Content; 404 not found; 409 deadline passed. |
| GET | `/api/events/{eventId}/results` | Returns published event results ordered by position. | Public | None | 200 OK result collection. |
| POST | `/api/events/{eventId}/results` | Records a finishing result for an enrolment. | Organiser | `enrolmentId, finishTime, position, resultStatus` | 201 Created; 400 invalid; 403 forbidden; 409 duplicate. |
| PUT | `/api/results/{id}` | Corrects an existing result. | Organiser | `finishTime, position, resultStatus` | 200 OK; 403 forbidden; 404 not found. |
| GET | `/api/results/{id}` | Returns one published result. | Public | None | 200 OK result; 404 not found. |

## Role model
- **Organiser:** creates and manages events, configures categories, views event enrolments and records results.
- **Participant:** maintains a profile, views events/categories, enrols in events and cancels their own enrolments before the deadline.

## Consistency rule
Part 2 implementation should closely follow these routes. Any deliberate deviation should be documented in the project README.
