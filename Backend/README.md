# School Portal — Backend

ASP.NET Core 10 Web API with SQL Server, JWT authentication, Entity Framework Core, Swagger and an idempotent academic-schema upgrade.

## Run in Visual Studio 2022+
1. Open `StudentAPI.slnx` or `StudentAPI.csproj`.
2. Ensure SQL Server LocalDB is available, or change `ConnectionStrings:DefaultConnection` in `appsettings.json`.
3. Run the API project.
4. In Development, Swagger is available from the API launch URL followed by `/swagger`.

On startup the API applies existing EF migrations and then runs an idempotent SQL upgrade for the academic tables/columns, so an existing installation is upgraded without losing its data.

## Default principal account
- Email: `admin@portal.local`
- Password: `admin 123`

Change this password before production use.

## Academic workflow
Principal creates a class/semester/section/session, enrolls active students, adds subjects and assigns approved teachers, then creates exams. Assigned teachers mark daily attendance and enter marks per exam. Principal publishes an exam; only published results become visible to students. Students can never modify academic data.

## Backend security
Teacher operations are restricted to subjects assigned to the logged-in teacher. Principal/Admin operations are separated from teacher/student operations. Attendance supports Present, Absent and Not Marked; Not Marked is not persisted as a lecture record. Marks are keyed by exam + subject + student, allowing multiple exams for the same subject.

This backend ZIP is source-only; build/publish folders are intentionally excluded.
