# Training API

> [!NOTE]
> This codebase is used for MetaCompliance engineering interviews. It was written for that purpose only and does not represent MetaCompliance's products, code or engineering practices.

A REST API for managing users and their compliance training. Admins assign training courses to users, and each user is notified through their preferred channel: email, SMS or Microsoft Teams.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Running locally

```bash
dotnet run --project src/TrainingApi
```

The API starts on `http://localhost:5251` and opens the API reference at `http://localhost:5251/scalar`.

On first run, a SQLite database (`training.db`) is created and seeded with sample users, courses and assignments. Delete the file to reset the data.

Notifications are simulated locally and written to the console.

## Sample data

| User | Role | Notification channel |
| --- | --- | --- |
| Sarah Mitchell | Admin | Email |
| Daniel Okafor | Learner | Teams |
| Priya Sharma | Learner | SMS |
| Tom Gallagher | Learner | Email |
| Aisling Byrne | Learner | Teams |

Courses: Phishing Awareness, GDPR Essentials, Password Security, Social Engineering.

## Endpoints

### Users

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/users` | List active users |
| GET | `/api/users/{id}` | Get a user |
| GET | `/api/users/search?email=` | Search users by email |
| POST | `/api/users` | Create a user |
| PUT | `/api/users/{id}` | Update a user |
| DELETE | `/api/users/{id}` | Deactivate a user |

### Assignments

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/assignments?userId=` | List assignments, optionally for one user |
| POST | `/api/assignments` | Assign a course to a user and notify them |
| POST | `/api/assignments/{id}/complete` | Mark an assignment as completed |
| POST | `/api/assignments/{id}/remind` | Send the user a reminder |
| GET | `/api/assignments/overdue` | List overdue assignments |

### Other

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/stats` | Training completion statistics |
| GET | `/health` | Health check |

## Example

Assign GDPR Essentials to Priya, as Sarah:

```bash
curl -X POST http://localhost:5251/api/assignments \
  -H "Content-Type: application/json" \
  -d '{ "userId": 3, "courseId": 2, "assignedByUserId": 1 }'
```
