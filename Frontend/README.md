# Student Management System — Frontend

Angular frontend for the [Student Management System API](https://github.com/AbdullahImran2320/StudentManagementAPI).

## Features
- JWT-based login and registration
- Student dashboard with live stats (total/active students, average GPA)
- Student CRUD with search and filtering

## Setup

```bash
npm install
ng serve
```

Runs at `http://localhost:4200`. Requires the backend API running — see the [backend repo](https://github.com/AbdullahImran2320/StudentManagementAPI) for setup.

## Build

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory.

## Running unit tests

```bash
ng test
```

Runs unit tests via [Vitest](https://vitest.dev/).

## Running end-to-end tests

Angular CLI does not include an e2e framework by default — you can add one that suits your needs.

---

Generated using [Angular CLI](https://github.com/angular/angular-cli) v22.0.7 — see the [CLI reference](https://angular.dev/tools/cli) for the full command list.
