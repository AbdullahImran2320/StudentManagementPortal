import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  template: `
    <div class="page">
      <div class="card">
        <div class="logo">🎓</div>
        <h2>Student Management System</h2>
        <p class="subtitle">Create an account</p>

        <div class="error" *ngIf="error">{{ error }}</div>

        <div class="field">
          <label>Name</label>
          <input [(ngModel)]="name" placeholder="Full name"/>
        </div>
        <div class="field">
          <label>Email</label>
          <input type="email" [(ngModel)]="email"
            placeholder="you@example.com"/>
        </div>

        <div class="field">
          <label>Phone</label>
          <input type="tel" [(ngModel)]="phone"
            placeholder="+923001234567"/>
        </div>



        <div class="field">
          <label>Password</label>
          <input type="password" [(ngModel)]="password"
            placeholder="At least 6 characters"/>
        </div>
        <div class="field">
          <label>Role</label>
          <select [(ngModel)]="role">
            <option value="Student">Student</option>
            <option value="Teacher">Teacher</option>
          </select>
        </div>

        <button (click)="register()" [disabled]="loading">
          {{ loading ? 'Creating account...' : 'Register' }}
        </button>

        <p class="link">
          Already have an account?
          <a routerLink="/login">Login here</a>
        </p>
      </div>
    </div>
  `,
  styles: [`
    .page {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea, #764ba2);
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .card {
      background: white;
      padding: 40px;
      border-radius: 12px;
      width: 380px;
      box-shadow: 0 20px 60px rgba(0,0,0,0.3);
      text-align: center;
    }
    .logo { font-size: 48px; margin-bottom: 10px; }
    h2 { color: #333; margin: 0 0 5px; font-size: 20px; }
    .subtitle { color: #666; margin-bottom: 25px; }
    .field { text-align: left; margin-bottom: 15px; }
    label {
      display: block;
      margin-bottom: 5px;
      color: #555;
      font-size: 14px;
    }
    input, select {
      width: 100%;
      padding: 10px;
      border: 1px solid #ddd;
      border-radius: 6px;
      font-size: 14px;
      box-sizing: border-box;
    }
    input:focus, select:focus {
      outline: none;
      border-color: #667eea;
    }
    button {
      width: 100%;
      padding: 12px;
      background: linear-gradient(135deg, #667eea, #764ba2);
      color: white;
      border: none;
      border-radius: 6px;
      font-size: 16px;
      cursor: pointer;
      margin-top: 5px;
    }
    button:disabled { opacity: 0.7; }
    .error {
      background: #ffe0e0;
      color: #c0392b;
      padding: 10px;
      border-radius: 6px;
      margin-bottom: 15px;
      font-size: 14px;
    }
    .link { margin-top: 20px; color: #666; font-size: 14px; }
    a { color: #667eea; text-decoration: none; }
  `]
})
export class RegisterComponent {
  name     = '';
  email    = '';
  password = '';
  phone    = '';
  role     = 'Student';
  loading  = false;
  error    = '';

  constructor(
    private auth: AuthService,
    private router: Router
  ) { }

 register() {
  if (!this.name || !this.email || !this.password || !this.phone) {
    this.error = 'Please fill all fields!';
    return;
  }
  if (this.password.length < 6) {
    this.error = 'Password must be at least 6 characters!';
    return;
  }
  this.loading = true;
  this.error   = '';

  this.auth.register({
    name: this.name,
    email: this.email,
    password: this.password,
    phone: this.phone,
    role: this.role
  }).subscribe({
    next: () => this.router.navigate(['/login'], { queryParams: { pending: '1' } }),
    error: (err) => {
      this.error   = err.error?.message ?? 'Registration failed!';
      this.loading = false;
    }
  });
}}
