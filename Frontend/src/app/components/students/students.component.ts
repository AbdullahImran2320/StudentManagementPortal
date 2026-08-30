import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { StudentService } from '../../services/student.service';
import { AuthService } from '../../services/auth.service';
import { Student } from '../../models/student.model';

@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <!-- NAVBAR -->
    <nav>
      <span class="brand">🎓 Students</span>
      <div class="nav-right">
        <a routerLink="/dashboard">Dashboard</a>
        <span>{{ userName }}</span>
        <button class="logout" (click)="logout()">
          Logout
        </button>
      </div>
    </nav>

    <div class="container">

      <!-- SEARCH + ADD -->
      <div class="toolbar">
        <div class="search">
          <input
            [(ngModel)]="searchTerm"
            placeholder="Search students..."
            (keyup.enter)="search()"
          />
          <button (click)="search()">🔍</button>
          <button (click)="loadAll()">All</button>
        </div>
        <button
          *ngIf="isAdmin"
          class="btn-add"
          (click)="showAddForm = true">
          + Add Student
        </button>
      </div>

      <!-- MESSAGES -->
      <div class="success" *ngIf="successMsg">
        ✅ {{ successMsg }}
      </div>
      <div class="error" *ngIf="errorMsg">
        ❌ {{ errorMsg }}
      </div>

      <!-- ADD STUDENT FORM -->
      <div class="form-card" *ngIf="showAddForm">
        <h3>Add New Student</h3>
        <div class="form-grid">
          <div class="field">
            <label>Name</label>
            <input [(ngModel)]="form.name"
              placeholder="Full name"/>
          </div>
          <div class="field">
            <label>Email</label>
            <input [(ngModel)]="form.email"
              placeholder="email@example.com"/>
          </div>
          <div class="field">
            <label>GPA (0-4)</label>
            <input type="number" [(ngModel)]="form.gpa"
              placeholder="3.50" step="0.01"/>
          </div>
          <div class="field">
            <label>City</label>
            <input [(ngModel)]="form.city"
              placeholder="Lahore"/>
          </div>
          <div class="field">
            <label>Course</label>
            <input [(ngModel)]="form.course"
              placeholder="BSIT"/>
          </div>
        </div>
        <div class="form-actions">
          <button class="btn-save"
            (click)="addStudent()">
            Save
          </button>
          <button class="btn-cancel"
            (click)="showAddForm = false">
            Cancel
          </button>
        </div>
      </div>

      <!-- EDIT STUDENT FORM -->
      <div class="form-card" *ngIf="editingStudent">
        <h3>Edit Student — {{ editingStudent.name }}</h3>
        <div class="form-grid">
          <div class="field">
            <label>Name</label>
            <input [(ngModel)]="editForm.name"/>
          </div>
          <div class="field">
            <label>GPA</label>
            <input type="number"
              [(ngModel)]="editForm.gpa" step="0.01"/>
          </div>
          <div class="field">
            <label>City</label>
            <input [(ngModel)]="editForm.city"/>
          </div>
          <div class="field">
            <label>Course</label>
            <input [(ngModel)]="editForm.course"/>
          </div>
        </div>
        <div class="form-actions">
          <button class="btn-save"
            (click)="saveEdit()">
            Update
          </button>
          <button class="btn-cancel"
            (click)="editingStudent = null">
            Cancel
          </button>
        </div>
      </div>

      <!-- LOADING -->
      <div class="loading" *ngIf="loading">
        Loading students...
      </div>

      <!-- STUDENTS TABLE -->
      <div class="table-card"
           *ngIf="!loading && students.length > 0">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Email</th>
              <th>GPA</th>
              <th>City</th>
              <th>Course</th>
              <th>Status</th>
              <th *ngIf="isAdmin">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let s of students">
              <td>{{ s.id }}</td>
              <td>{{ s.name }}</td>
              <td>{{ s.email }}</td>
              <td>
                <span class="gpa"
                  [style.background]="
                    s.gpa >= 3.5 ? '#2ecc71' :
                    s.gpa >= 2.5 ? '#f39c12' : '#e74c3c'">
                  {{ s.gpa }}
                </span>
              </td>
              <td>{{ s.city }}</td>
              <td>{{ s.course }}</td>
              <td>
                <span [class]="s.isActive ?
                  'badge-active' : 'badge-inactive'">
                  {{ s.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td *ngIf="isAdmin">
                <button class="btn-edit"
                  (click)="startEdit(s)">
                  ✏️
                </button>
                <button class="btn-del"
                  (click)="deleteStudent(s.id)">
                  🗑️
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- EMPTY -->
      <div class="empty"
           *ngIf="!loading && students.length === 0">
        No students found!
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
    .nav-right a { color: white; text-decoration: none; }
    .logout {
      background: #e74c3c;
      color: white;
      border: none;
      padding: 8px 15px;
      border-radius: 4px;
      cursor: pointer;
    }
    .container {
      max-width: 1200px;
      margin: 30px auto;
      padding: 0 20px;
    }
    .toolbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }
    .search {
      display: flex;
      gap: 10px;
    }
    .search input {
      padding: 10px;
      border: 1px solid #ddd;
      border-radius: 6px;
      width: 280px;
    }
    .search button {
      padding: 10px 15px;
      border: none;
      border-radius: 6px;
      cursor: pointer;
      background: #ecf0f1;
    }
    .btn-add {
      background: #2ecc71;
      color: white;
      border: none;
      padding: 10px 20px;
      border-radius: 6px;
      cursor: pointer;
      font-size: 14px;
    }
    .form-card {
      background: white;
      border-radius: 10px;
      padding: 25px;
      margin-bottom: 20px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.08);
    }
    .form-card h3 { margin-bottom: 20px; }
    .form-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 15px;
    }
    .field label {
      display: block;
      margin-bottom: 5px;
      font-size: 13px;
      color: #555;
    }
    .field input {
      width: 100%;
      padding: 8px;
      border: 1px solid #ddd;
      border-radius: 6px;
      box-sizing: border-box;
    }
    .form-actions {
      display: flex;
      gap: 10px;
      margin-top: 20px;
    }
    .btn-save {
      background: #3498db;
      color: white;
      border: none;
      padding: 10px 25px;
      border-radius: 6px;
      cursor: pointer;
    }
    .btn-cancel {
      background: #95a5a6;
      color: white;
      border: none;
      padding: 10px 25px;
      border-radius: 6px;
      cursor: pointer;
    }
    .table-card {
      background: white;
      border-radius: 10px;
      padding: 20px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.08);
    }
    table { width: 100%; border-collapse: collapse; }
    th, td {
      padding: 12px 15px;
      text-align: left;
      border-bottom: 1px solid #eee;
    }
    th {
      background: #f8f9fa;
      font-weight: 600;
      color: #555;
      font-size: 13px;
    }
    tr:hover { background: #fafafa; }
    .gpa {
      color: white;
      padding: 3px 10px;
      border-radius: 20px;
      font-size: 13px;
      font-weight: bold;
    }
    .badge-active {
      background: #2ecc71;
      color: white;
      padding: 3px 10px;
      border-radius: 20px;
      font-size: 12px;
    }
    .badge-inactive {
      background: #e74c3c;
      color: white;
      padding: 3px 10px;
      border-radius: 20px;
      font-size: 12px;
    }
    .btn-edit {
      background: none;
      border: none;
      cursor: pointer;
      font-size: 16px;
      margin-right: 5px;
    }
    .btn-del {
      background: none;
      border: none;
      cursor: pointer;
      font-size: 16px;
    }
    .success {
      background: #d4edda;
      color: #155724;
      padding: 12px;
      border-radius: 6px;
      margin-bottom: 15px;
    }
    .error {
      background: #f8d7da;
      color: #721c24;
      padding: 12px;
      border-radius: 6px;
      margin-bottom: 15px;
    }
    .loading, .empty {
      text-align: center;
      padding: 50px;
      color: #999;
    }
  `]
})
export class StudentsComponent implements OnInit {

  students      : Student[] = [];
  loading                   = false;
  successMsg                = '';
  errorMsg                  = '';
  searchTerm                = '';
  showAddForm               = false;
  editingStudent: Student | null = null;
  isAdmin                   = false;
  userName                  = '';

  form = {
    name: '', email: '',
    gpa: 0, city: '', course: ''
  };

  editForm = {
    name: '', gpa: 0,
    city: '', course: ''
  };

  constructor(
    private studentService: StudentService,
    private auth: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    this.isAdmin  = this.auth.isAdmin();
    this.userName = this.auth.getName();
    this.loadAll();
  }

  loadAll() {
    this.loading   = true;
    this.searchTerm = '';
    this.studentService.getAll().subscribe({
      next:  (data) => {
        this.students = data;
        this.loading  = false;
      },
      error: () => {
        this.errorMsg = 'Failed to load students!';
        this.loading  = false;
      }
    });
  }

  search() {
    if (!this.searchTerm.trim()) {
      this.loadAll();
      return;
    }
    this.loading = true;
    this.studentService.search(this.searchTerm)
      .subscribe({
        next:  (data) => {
          this.students = data;
          this.loading  = false;
        },
        error: () => {
          this.errorMsg = 'Search failed!';
          this.loading  = false;
        }
      });
  }

  addStudent() {
    if (!this.form.name || !this.form.email) {
      this.errorMsg = 'Please fill all fields!';
      return;
    }
    this.studentService.create(this.form).subscribe({
      next: (s) => {
        this.students.push(s);
        this.showAddForm = false;
        this.form = {
          name: '', email: '',
          gpa: 0, city: '', course: ''
        };
        this.showSuccess('Student added successfully!');
      },
      error: (err) => {
        this.errorMsg = err.error?.message
                     ?? 'Failed to add student!';
      }
    });
  }

  startEdit(s: Student) {
    this.editingStudent = s;
    this.editForm = {
      name:   s.name,
      gpa:    s.gpa,
      city:   s.city,
      course: s.course
    };
  }

  saveEdit() {
    if (!this.editingStudent) return;

    this.studentService
      .update(this.editingStudent.id, this.editForm)
      .subscribe({
        next: (updated) => {
          const index = this.students
            .findIndex(s => s.id === updated.id);
          if (index !== -1)
            this.students[index] = updated;
          this.editingStudent = null;
          this.showSuccess('Student updated!');
        },
        error: (err) => {
          this.errorMsg = err.error?.message
                       ?? 'Update failed!';
        }
      });
  }

  deleteStudent(id: number) {
    if (!confirm('Delete this student?')) return;

    this.studentService.delete(id).subscribe({
      next: () => {
        this.students = this.students
          .filter(s => s.id !== id);
        this.showSuccess('Student deleted!');
      },
      error: (err) => {
        this.errorMsg = err.error?.message
                     ?? 'Delete failed!';
      }
    });
  }

  private showSuccess(msg: string) {
    this.successMsg = msg;
    this.errorMsg   = '';
    setTimeout(() => this.successMsg = '', 3000);
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
