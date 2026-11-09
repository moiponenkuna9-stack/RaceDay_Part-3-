# Implemented endpoint inventory

| Method | Route | Access |
|---|---|---|
| POST | `/api/auth/register` | Public |
| POST | `/api/auth/login` | Public |
| POST | `/api/auth/logout` | Public |
| GET | `/api/profile` | Authenticated |
| PUT | `/api/profile` | Authenticated |
| GET | `/api/events` | Public |
| GET | `/api/events/{id}` | Public |
| POST | `/api/events` | Organiser |
| PUT | `/api/events/{id}` | Organiser |
| DELETE | `/api/events/{id}` | Organiser |
| GET | `/api/categories` | Public |
| POST | `/api/events/{eventId}/categories` | Organiser |
| DELETE | `/api/events/{eventId}/categories/{categoryId}` | Organiser |
| POST | `/api/events/{eventId}/enrolments` | Participant |
| GET | `/api/enrolments` | Participant |
| GET | `/api/events/{eventId}/enrolments` | Organiser |
| DELETE | `/api/enrolments/{id}` | Participant |
| GET | `/api/events/{eventId}/results` | Public |
| POST | `/api/events/{eventId}/results` | Organiser |
| PUT | `/api/results/{id}` | Organiser |
| GET | `/api/results/{id}` | Public / participant-own enforcement |
