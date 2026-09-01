# School Portal — Frontend

Angular 22 standalone frontend for the School Management Portal.

## Run
1. Install Node.js 22+.
2. Run `npm install`.
3. Run `npm start`.
4. Open `http://localhost:4200`.

Set the API base URL in `src/environment/environment.ts` before running if your ASP.NET API uses a different address.

## Roles
- Principal/Admin: classes, semester structure, students, subjects, teacher assignment, exams, result publication and account approvals.
- Teacher: assigned classes/subjects, whole-class attendance, Present/Absent/Not marked workflow, exam marks and class results.
- Student: read-only published results and personal attendance.

The frontend is source-only. No `node_modules`, build output, or publish output is included.
