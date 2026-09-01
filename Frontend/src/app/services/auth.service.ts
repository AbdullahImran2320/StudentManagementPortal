import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environment/environment';
import { LoginRequest, RegisterRequest, TokenResponse } from '../models/auth.model';

@Injectable({ providedIn:'root' })
export class AuthService {
  private apiUrl=`${environment.apiUrl}/auth`;
  constructor(private http:HttpClient){}
  login(data:LoginRequest):Observable<TokenResponse>{ return this.http.post<TokenResponse>(`${this.apiUrl}/login`,data).pipe(tap(r=>this.saveToken(r))); }
  register(data:RegisterRequest):Observable<TokenResponse>{ return this.http.post<TokenResponse>(`${this.apiUrl}/register`,data).pipe(tap(()=>{})); }
  logout(){ localStorage.clear(); }
  private saveToken(r:TokenResponse){ localStorage.setItem('token',r.token);localStorage.setItem('role',r.role);localStorage.setItem('name',r.name);localStorage.setItem('email',r.email); }
  isLoggedIn(){ return !!localStorage.getItem('token'); }
  getRole(){ return localStorage.getItem('role')||'Student'; }
  getName(){ return localStorage.getItem('name')||''; }
  getEmail(){ return localStorage.getItem('email')||''; }
  isAdmin(){ return this.getRole().toLowerCase()==='admin'; }
  isTeacher(){ return this.getRole().toLowerCase()==='teacher'; }
  isStudent(){ return this.getRole().toLowerCase()==='student'; }
}
