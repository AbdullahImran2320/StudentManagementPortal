import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  template: `
    <div class="page">
      <div class="card">
        <div class="logo">🎓</div>
        <h2>Student Management System</h2>
        <p class="subtitle">Login to continue</p>
        <div class="pending" *ngIf="pending">Account created. An administrator must approve it before you can sign in.</div>

        <div class="error" *ngIf="error">{{ error }}</div>

        <div class="field">
          <label>Email</label>
          <input type="email" [(ngModel)]="email"
            placeholder="admin@sms.com"/>
        </div>

        <div class="field">
          <label>Password</label>
          <input type="password" [(ngModel)]="password"
            placeholder="admin123"/>
        </div>

        <button (click)="login()" [disabled]="loading">
          {{ loading ? 'Logging in...' : 'Login' }}
        </button>

        <p class="link">
          New user?
          <a routerLink="/register">Register here</a>
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
    input {
      width: 100%;
      padding: 10px;
      border: 1px solid #ddd;
      border-radius: 6px;
      font-size: 14px;
      box-sizing: border-box;
    }
    input:focus {
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
    .pending { background:#e5fbf8; color:#187a75; padding:10px; border-radius:6px; margin-bottom:15px; font-size:13px; }
    .link { margin-top: 20px; color: #666; font-size: 14px; }
    a { color: #667eea; text-decoration: none; }
  `]
})
export class LoginComponent {
  email    = '';
  password = '';
  loading  = false;
  error    = '';
  pending = false;

  constructor(
    private auth: AuthService,
    private router: Router,
    route: ActivatedRoute
  ) { this.pending = route.snapshot.queryParamMap.get('pending') === '1'; }

  login() {
    if (!this.email || !this.password) {
      this.error = 'Please fill all fields!';
      return;
    }
    this.loading = true;
    this.error   = '';

    this.auth.login({
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.error   = err.error?.message ?? 'Login failed!';
        this.loading = false;
      }
    });
  }
}
