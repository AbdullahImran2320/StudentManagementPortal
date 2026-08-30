Student Management System

A full-stack student management platform built with ASP.NET Core Web API and Angular, featuring JWT authentication, role-based authorization, and multi-channel welcome notifications (Email, SMS, and WhatsApp) via Twilio.

Features
Authentication & Authorization — JWT-based auth with role-based access control (Admin/User roles)
Student CRUD — create, read, update, delete student records with search, filtering by city/course, and top-student ranking by GPA
Dashboard — live stats for total students, active students, and average GPA
Multi-channel welcome notifications — on registration, users receive a welcome message via:
Email (HTML template, sent via SMTP/Gmail using MailKit)
SMS (via Twilio)
WhatsApp (via Twilio's WhatsApp Business API)
Global exception handling — centralized middleware returning consistent JSON error responses
Input validation — DTO-level validation with data annotations on every write endpoint
Swagger/OpenAPI — interactive API documentation with JWT bearer auth support built in
Tech Stack

Backend

ASP.NET Core 10 (Web API)
Entity Framework Core 10 (Code-First, SQL Server)
JWT Bearer Authentication
BCrypt.Net for password hashing
MailKit for SMTP email delivery
Twilio SDK for SMS and WhatsApp messaging
Swashbuckle (Swagger)

Frontend

Angular (standalone components)
TypeScript
RxJS

Database

SQL Server (LocalDB for development)
Architecture

The API follows a layered architecture:

Controllers  →  Services  →  Repositories  →  EF Core DbContext  →  SQL Server
Controllers handle HTTP concerns only (routing, status codes)
Services contain business logic (auth, token generation, notification orchestration)
Repositories encapsulate data access behind interfaces (IStudentRepository)
DTOs decouple API contracts from EF Core entities, with validation attributes
Dependency Injection wires everything together via interfaces (IAuthService, IEmailService, ISmsService, IStudentRepository), keeping each layer independently testable
Getting Started
Prerequisites
.NET 10 SDK
SQL Server / SQL Server LocalDB
Node.js and Angular CLI (for the frontend)
A Twilio account (free trial works) for SMS/WhatsApp
A Gmail account with an App Password for email sending
Backend Setup
Clone the repository and navigate to the StudentAPI folder.
Configure your connection string in appsettings.json if it differs from the default LocalDB setup.
Set your secrets using .NET User Secrets (never commit real credentials to appsettings.json):
bash
   dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
   dotnet user-secrets set "EmailSettings:SenderPassword" "your-app-password"
   dotnet user-secrets set "SmsSettings:AccountSid" "your-twilio-account-sid"
   dotnet user-secrets set "SmsSettings:AuthToken" "your-twilio-auth-token"
   dotnet user-secrets set "SmsSettings:TwilioPhoneNumber" "+1XXXXXXXXXX"
   dotnet user-secrets set "SmsSettings:TwilioWhatsAppNumber" "+14155238886"
   dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key-min-32-characters"
Apply EF Core migrations:
bash
   dotnet ef database update
Run the API:
bash
   dotnet run

Swagger UI will be available at https://localhost:<port>/swagger.

Frontend Setup
Navigate to the student-app folder.
Install dependencies:
bash
   npm install
Update src/environment/environment.ts with your API base URL if needed.
Run the dev server:
bash
   ng serve

The app will be available at http://localhost:4200.

API Overview
Method	Endpoint	Auth	Description
POST	/api/Auth/Register	—	Register a new user; triggers welcome email, SMS, and WhatsApp message
POST	/api/Auth/Login	—	Authenticate and receive a JWT
GET	/api/students	—	List all students
GET	/api/students/{id}	—	Get a student by ID
GET	/api/students/search?name=	—	Search students by name
GET	/api/students/city/{city}	—	Filter students by city
GET	/api/students/course/{course}	—	Filter students by course
GET	/api/students/top?count=5	—	Get top students by GPA
GET	/api/students/Stats	—	Dashboard stats (total, active, average GPA)
POST	/api/students	Required	Create a student
PUT	/api/students/{id}	Required	Update a student
DELETE	/api/students/{id}	Admin role	Delete a student
Notes on the Notification System

Registration triggers three independent notification channels, each wrapped in its own try/catch so that a failure in one (e.g. an invalid phone number) never blocks account creation or the other channels:

Email uses MailKit over SMTP with an HTML template styled to match the app's branding.
SMS and WhatsApp both go through the same Twilio account and client — WhatsApp messages simply use the whatsapp: prefix on the sender/recipient numbers, so no separate service was needed.

During development, Twilio's WhatsApp Sandbox and trial-account verified numbers were used; a production deployment would require a Twilio paid number and Meta Business verification for WhatsApp.

Future Improvements
Automated unit and integration tests
Live deployment (Azure App Service + Azure SQL)
Pagination on student list endpoints
Refresh token support alongside JWT access tokens

**Frontend repo:** https://github.com/AbdullahImran2320/StudentManagementFrontend


Author

Abdullah — BBIT student, .NET Intern