import { Routes } from '@angular/router';
import { LoginComponent }
  from './components/login/login.component';
import { RegisterComponent }
  from './components/register/register.component';
import { DashboardComponent }
  from './components/dashboard/dashboard.component';
import { StudentsComponent }
  from './components/students/students.component';

export const routes: Routes = [
  { path: '',
    redirectTo: 'login',
    pathMatch: 'full' },
  { path: 'login',
    component: LoginComponent },
  { path: 'register',
    component: RegisterComponent },
  { path: 'dashboard',
    component: DashboardComponent },
  { path: 'students',
    component: StudentsComponent },
  { path: '**',
    redirectTo: 'login' }
];
