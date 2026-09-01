import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

export interface AcademicClass { id:number; program:string; semester:number; section:string; session:string; studentCount:number; subjectCount:number; }
export interface Teacher { id:number; name:string; email:string; }
export interface SchoolStudent { id:number; name:string; email:string; course:string; city:string; }
export interface RosterStudent { studentId:number; name:string; email:string; course:string; }
export interface Subject { id:number; code:string; name:string; creditHours:number; teacherUserId?:number|null; teacherName:string; }
export interface Exam { id:number; name:string; examType:string; examDate:string; defaultTotalMarks:number; isPublished:boolean; }
export type AttendanceStatus = 'NotMarked'|'Present'|'Absent';
export interface AttendanceRow { studentId:number; studentName:string; email:string; lectureDate:string; status:AttendanceStatus|number; }
export interface MarkRow { studentId:number; studentName:string; email:string; obtainedMarks:number; totalMarks:number; isEntered:boolean; }
export interface ResultSubject { id:number; code:string; name:string; creditHours:number; obtainedMarks:number|null; totalMarks:number|null; percent:number|null; grade:string; point:number; }
export interface StudentResult { studentId:number; name:string; subjects:ResultSubject[]; gpa:number; status:string; }
export interface MyResults { id:number; name:string; gpa:number; subjects:Array<{examId:number|null;examName:string;code:string;subjectName:string;creditHours:number;obtainedMarks:number;totalMarks:number;percent:number;grade:string;point:number}>; }
export interface MyAttendance { code:string; name:string; totalLectures:number; present:number; absent:number; attendancePercent:number; }

@Injectable({ providedIn:'root' })
export class AcademicService {
  private readonly api = `${environment.apiUrl}/academic`;
  constructor(private http:HttpClient) {}
  classes(){ return this.http.get<AcademicClass[]>(`${this.api}/classes`); }
  createClass(data:{program:string;semester:number;section:string;session:string}){ return this.http.post(`${this.api}/classes`,data); }
  teachers(){ return this.http.get<Teacher[]>(`${this.api}/teachers`); }
  students(){ return this.http.get<SchoolStudent[]>(`${this.api}/students`); }
  roster(classId:number){ return this.http.get<RosterStudent[]>(`${this.api}/classes/${classId}/students`); }
  enroll(classId:number,studentIds:number[]){ return this.http.post(`${this.api}/classes/${classId}/students`,{studentIds}); }
  subjects(classId:number){ return this.http.get<Subject[]>(`${this.api}/classes/${classId}/subjects`); }
  addSubject(classId:number,data:{code:string;name:string;creditHours:number;teacherUserId:number|null}){ return this.http.post(`${this.api}/classes/${classId}/subjects`,data); }
  assignTeacher(subjectId:number,teacherUserId:number|null){ return this.http.put(`${this.api}/subjects/${subjectId}/teacher`,teacherUserId); }
  exams(classId:number){ return this.http.get<Exam[]>(`${this.api}/classes/${classId}/exams`); }
  createExam(classId:number,data:{name:string;examType:string;examDate:string;defaultTotalMarks:number}){ return this.http.post(`${this.api}/classes/${classId}/exams`,data); }
  publishExam(examId:number,published:boolean){ return this.http.put(`${this.api}/exams/${examId}/publish`,published); }
  attendance(subjectId:number,date:string){ return this.http.get<AttendanceRow[]>(`${this.api}/subjects/${subjectId}/attendance`,{params:new HttpParams().set('date',date)}); }
  saveAttendance(subjectId:number,lectureDate:string,entries:Array<{studentId:number;status:number}>){ return this.http.put(`${this.api}/subjects/${subjectId}/attendance`,{lectureDate,entries}); }
  marks(subjectId:number,examId:number){ return this.http.get<MarkRow[]>(`${this.api}/subjects/${subjectId}/marks`,{params:new HttpParams().set('examId',examId)}); }
  saveMarks(subjectId:number,examId:number,entries:Array<{studentId:number;obtainedMarks:number;totalMarks:number}>){ return this.http.put(`${this.api}/subjects/${subjectId}/marks`,{examId,entries}); }
  results(classId:number,examId:number|null){ let params=new HttpParams(); if(examId) params=params.set('examId',examId); return this.http.get<StudentResult[]>(`${this.api}/classes/${classId}/results`,{params}); }
  myResults(){ return this.http.get<MyResults>(`${this.api}/me/results`); }
  myAttendance(){ return this.http.get<MyAttendance[]>(`${this.api}/me/attendance`); }
}
