# Part 2 testing checklist

Use this checklist when demonstrating the API and collecting evidence.

| Area | Success test | Failure / role test |
|---|---|---|
| Registration | Register a new Organiser or Participant | Duplicate email rejected |
| Login | Correct credentials create a session | Incorrect password returns 401 |
| Profile | Logged-in user can view/update own profile | Anonymous request returns 401 |
| Events | Organiser creates/updates own event | Participant receives 403 |
| Categories | Organiser adds a category to own event | Duplicate category returns 409 |
| Enrolments | Participant enrols in an open event | Organiser receives 403; duplicate enrolment returns 409 |
| Organiser enrolments | Organiser views enrolments for own event | Other organiser receives 403 |
| Results | Organiser records/corrects result | Participant cannot create/correct results |
| Own results | Participant can view own result | Participant cannot view another participant's result by ID |
| Swagger | All implemented endpoints visible and testable | N/A |
| CI | Build and tests pass | Failed test/build should fail the workflow |
