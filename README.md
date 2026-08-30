# Student Management Portal

A full-stack student management system — ASP.NET Core Web API backend + Angular
frontend — packaged as a single Windows installer.

This repo combines two previously separate repos into one monorepo with a build
pipeline that produces a single self-contained `.exe` installer (via Inno Setup),
the same way [Hardware-store-portal](https://github.com/AbdullahImran2320/Hardware-store-portal)
does.

- Original backend: [StudentManagementAPI](https://github.com/AbdullahImran2320/StudentManagementAPI)
- Original frontend: [StudentManagementFrontend](https://github.com/AbdullahImran2320/StudentManagementFrontend)

## Features

- JWT authentication with role-based access control (Admin/User)
- Student CRUD with search, filtering, and top-student ranking by GPA
- Live dashboard stats
- Welcome notifications on registration via Email (MailKit/SMTP), SMS and
  WhatsApp (Twilio)
- Angular 22 standalone-component frontend, served by the API itself in the
  packaged build (same origin — no separate frontend server needed)

## Repository layout

```
StudentManagementPortal/
├── Backend/            ASP.NET Core 10 Web API (StudentAPI)
├── Frontend/            Angular 22 app (student-app)
├── docs/                 Screenshots / extra docs
├── setup/                Compiled installer .exe lands here (git-ignored)
├── build.bat              Builds Angular, publishes a self-contained backend exe
├── compile-installer.bat   Compiles installer.iss into setup\StudentManagementPortalSetup.exe
├── installer.iss           Inno Setup script
└── LaunchStudentManagementPortal.ps1  Launcher used by the installed shortcuts
```

## Running in development

**Backend**

```bash
cd Backend
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key-min-32-characters"
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:SenderPassword" "your-app-password"
dotnet user-secrets set "SmsSettings:AccountSid" "your-twilio-account-sid"
dotnet user-secrets set "SmsSettings:AuthToken" "your-twilio-auth-token"
dotnet ef database update
dotnet run
```

Swagger UI: `https://localhost:7141/swagger`

**Frontend**

```bash
cd Frontend
npm install
ng serve
```

App: `http://localhost:4200` (talks to the backend via CORS, as configured today).

## Building the Windows installer

Requirements on the build machine:

- .NET 10 SDK
- Node.js + Angular CLI
- [Inno Setup 6](https://jrsoftware.org/isdl.php)

Steps:

```bat
build.bat
compile-installer.bat
```

`build.bat` will:
1. `npm install` + `ng build --configuration production` in `Frontend/`
2. Copy the Angular output into `Backend/wwwroot`
3. `dotnet publish` the backend as a self-contained, single-file `win-x64` exe
   into `publish/StudentManagementPortal/`

`compile-installer.bat` then runs Inno Setup's `ISCC.exe` against `installer.iss`
to produce `setup/StudentManagementPortalSetup.exe` — a single-file installer
that:
- Installs the app under Program Files
- Adds a Start Menu shortcut and an optional desktop shortcut
- Runs the app and opens it in the browser on first launch
- Applies EF Core migrations automatically on first startup

**Before running `build.bat` for a real release**, replace the placeholder
values in `Backend/appsettings.json` (JWT secret, email, Twilio credentials)
with real ones — those values get baked into the published exe. Don't commit
real secrets to this repo; keep `appsettings.json` placeholders in git and set
real values locally (or in `appsettings.Production.json`, which you can choose
to keep out of version control) before building.

### Database prerequisite

The packaged app uses SQL Server LocalDB by default (`appsettings.json` →
`ConnectionStrings:DefaultConnection`). The installer checks the registry for
LocalDB and warns if it isn't found — end users without it should install it
from https://aka.ms/localdbdotnetcore before first run, or you can switch the
connection string to a full SQL Server instance.

## Tech stack

**Backend:** ASP.NET Core 10, EF Core 10 (SQL Server), JWT auth, BCrypt.Net,
MailKit, Twilio SDK, Swashbuckle.

**Frontend:** Angular 22 (standalone components), TypeScript, RxJS.

## Author

Abdullah Imran — BBIT student, .NET Intern
