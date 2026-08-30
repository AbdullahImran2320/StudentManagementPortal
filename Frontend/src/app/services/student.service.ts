import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import {
  Student,
  CreateStudent,
  UpdateStudent,
  Stats
} from '../models/student.model';

@Injectable({ providedIn: 'root' })
export class StudentService {

  private apiUrl = `${environment.apiUrl}/students`;

  constructor(private http: HttpClient) { }

  getAll()    : Observable<Student[]> {
    return this.http.get<Student[]>(this.apiUrl);
  }
  getById(id: number): Observable<Student> {
    return this.http.get<Student>(`${this.apiUrl}/${id}`);
  }
  search(name: string): Observable<Student[]> {
    return this.http.get<Student[]>(
      `${this.apiUrl}/search?name=${name}`);
  }
  getTop(count = 5): Observable<Student[]> {
    return this.http.get<Student[]>(
      `${this.apiUrl}/top?count=${count}`);
  }
  getStats(): Observable<Stats> {
    return this.http.get<Stats>(
      `${this.apiUrl}/stats`);
  }
  create(data: CreateStudent): Observable<Student> {
    return this.http.post<Student>(this.apiUrl, data);
  }
  update(id: number, data: UpdateStudent): Observable<Student> {
    return this.http.put<Student>(
      `${this.apiUrl}/${id}`, data);
  }
  delete(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`);
  }
}
