import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environment/environment';
import {
  LoginRequest,
  RegisterRequest,
  TokenResponse
} from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {

  private apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient) { }

  login(data: LoginRequest): Observable<TokenResponse> {
    return this.http
      .post<TokenResponse>(`${this.apiUrl}/login`, data)
      .pipe(tap(r => this.saveToken(r)));
  }

  register(data: RegisterRequest): Observable<TokenResponse> {
    return this.http
      .post<TokenResponse>(`${this.apiUrl}/register`, data)
      .pipe(tap(r => this.saveToken(r)));
  }

  logout(): void {
    localStorage.clear();
  }

  private saveToken(r: TokenResponse): void {
    localStorage.setItem('token', r.token);
    localStorage.setItem('role',  r.role);
    localStorage.setItem('name',  r.name);
    localStorage.setItem('email', r.email);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }
  getToken() : string  { return localStorage.getItem('token') ?? ''; }
  getRole()  : string  { return localStorage.getItem('role')  ?? 'User'; }
  getName()  : string  { return localStorage.getItem('name')  ?? ''; }
  isAdmin()  : boolean { return this.getRole() === 'Admin'; }
}
