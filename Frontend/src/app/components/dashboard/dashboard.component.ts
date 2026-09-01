import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { StudentService } from '../../services/student.service';
import { AuthService } from '../../services/auth.service';
import { Stats, Student } from '../../models/student.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <!-- NAVBAR -->
    <nav>
      <span class="brand">🎓 SMS Dashboard</span>
      <div class="nav-right">
        <span>{{ userName }} ({{ userRole }})</span>
        <a routerLink="/students">Students</a>
        <a routerLink="/attendance" *ngIf="isAdmin || isTeacher">Attendance</a>
        <a routerLink="/admin-users" *ngIf="isAdmin">User Approvals</a>
        <button (click)="logout()">Logout</button>
      </div>
    </nav>

    <div class="container">
      <h2>Welcome back, {{ userName }}! 👋</h2>

      <!-- STATS CARDS -->
      <div class="stats-grid" *ngIf="stats">
        <div class="stat-card blue">
          <div class="stat-number">
            {{ stats.totalStudents }}
          </div>
          <div class="stat-label">Total Students</div>
        </div>
        <div class="stat-card green">
          <div class="stat-number">
            {{ stats.activeStudents }}
          </div>
          <div class="stat-label">Active Students</div>
        </div>
        <div class="stat-card purple">
          <div class="stat-number">
            {{ stats.averageGPA | number:'1.2-2' }}
          </div>
          <div class="stat-label">Average GPA</div>
        </div>
      </div>

      <!-- TOP STUDENTS -->
      <div class="section">
  <h3>🏆 Top 5 Students by GPA</h3>

  <button class="action-btn blue" (click)="loadTopStudents()" *ngIf="!topStudentsLoaded">
    Show Top 5 Students
  </button>

  <table *ngIf="topStudents.length > 0">
    <thead>
      <tr>
        <th>Rank</th>
        <th>Name</th>
        <th>Course</th>
        <th>City</th>
        <th>GPA</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let s of topStudents; let i = index">
        <td>
          {{ i === 0 ? '🥇' :
             i === 1 ? '🥈' :
             i === 2 ? '🥉' : '#' + (i+1) }}
        </td>
        <td>{{ s.name }}</td>
        <td>{{ s.course }}</td>
        <td>{{ s.city }}</td>
        <td><span class="gpa-badge">{{ s.gpa }}</span></td>
      </tr>
    </tbody>
  </table>

  <p *ngIf="topStudentsLoaded && topStudents.length === 0">
    No students found.
  </p>
</div>

      <!-- QUICK ACTIONS -->
      <div class="section">
        <h3>⚡ Quick Actions</h3>
        <div class="actions">
          <a routerLink="/students" class="action-btn blue">
            📋 View All Students
          </a>
          <a routerLink="/students"
             *ngIf="isAdmin"
             class="action-btn green">
            ➕ Add New Student
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    nav {
      background: #2c3e50;
      color: white;
      padding: 15px 30px;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .brand { font-size: 20px; font-weight: bold; }
    .nav-right {
      display: flex;
      align-items: center;
      gap: 20px;
    }
    .nav-right a {
      color: white;
      text-decoration: none;
    }
    .nav-right button {
      background: #e74c3c;
      color: white;
      border: none;
      padding: 8px 15px;
      border-radius: 4px;
      cursor: pointer;
    }
    .container {
      max-width: 1000px;
      margin: 30px auto;
      padding: 0 20px;
    }
    h2 { color: #01070c; margin-bottom: 25px; }
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 20px;
      margin-bottom: 15px;
    }
    .stat-card {
      padding: 10px;
      border-radius: 5px;
      text-align: center;
      color: white;
    }
    .stat-card.blue   { background: #62a1ca; }
    .stat-card.green  { background: #6dd197; }
    .stat-card.purple { background: #7baa3d; }
    .stat-number {
      font-size: 12px;
      font-weight: bold;
    }
    .stat-label { font-size: 14px; opacity: 0.9; }
    .section {
      background: white;
      border-radius: 5px;
      padding: 15px;
      margin-bottom: 5px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.08);
    }
    .section h3 { margin-bottom: 20px; color: #2c3e50; }
    table { width: 100%; border-collapse: collapse; }
    th, td {
      padding: 12px;
      text-align: left;
      border-bottom: 1px solid #eee;
    }
    th {
      background: #f8f9fa;
      font-weight: 600;
      color: #555;
    }
    .gpa-badge {
      background: #2ecc71;
      color: white;
      padding: 4px 10px;
      border-radius: 20px;
      font-weight: bold;
    }
    .actions { display: flex; gap: 15px; }
    .action-btn {
      padding: 12px 25px;
      border-radius: 8px;
      color: white;
      text-decoration: none;
      font-weight: 500;
    }
    .action-btn.blue   { background: #3498db; }
    .action-btn.green  { background: #2ecc71; }
  `]
})
export class DashboardComponent implements OnInit {

  stats       : Stats | null = null;
  topStudents : Student[]    = [];
  userName                   = '';
  userRole                   = '';
  isAdmin                    = false;
  isTeacher                  = false;

  constructor(
    private studentService: StudentService,
    private auth: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    this.userName = this.auth.getName();
    this.userRole = this.auth.getRole();
    this.isAdmin  = this.auth.isAdmin();
    this.isTeacher = this.auth.isTeacher();
    this.loadStats();
   // this.loadTopStudents();
  }

 loadStats() {
  this.studentService.getStats().subscribe({
    next: (data) => {
      this.stats = data;
      this.cdr.detectChanges();
    },
    error: (err) => console.error('Stats error:', err)
  });
}
topStudentsLoaded = false;
loadTopStudents() {
  this.studentService.getTop(5).subscribe({
    next: (data) => {
      this.topStudents = data;
        this.topStudentsLoaded = true;
      this.cdr.detectChanges();
    },
    error: (err) => console.error('Top students error:', err)
  });
}

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
