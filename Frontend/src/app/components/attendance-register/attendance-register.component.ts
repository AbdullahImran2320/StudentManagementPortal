import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AcademicService, AcademicClass, Subject, AttendanceRow } from '../../services/academic.service';

function localToday(){const d=new Date();return `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`}

@Component({
  selector: 'app-attendance-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `<section class="register"><div><p>TEACHER WORKSPACE</p><h2>Whole-class attendance</h2><span>Select a class and course; the lecture date defaults to today. Every enrolled student appears in the register.</span></div>
<div class="filters"><select [(ngModel)]="classId" (change)="onClassChange()"><option [ngValue]="0">Select class</option><option *ngFor="let c of classes" [ngValue]="c.id">{{c.program}} · Semester {{c.semester}} · {{c.section}}</option></select><select [(ngModel)]="subjectId" (change)="loadAttendance()"><option [ngValue]="0">Select course</option><option *ngFor="let s of subjects" [ngValue]="s.id">{{s.code}} — {{s.name}}</option></select><input type="date" [(ngModel)]="date" (change)="loadAttendance()"></div>
<div class="toolbar" *ngIf="attendance.length"><b>{{attendance.length}} students · {{markedCount}} marked</b><button (click)="setAll(1)">Mark all present</button><button (click)="setAll(2)">Mark all absent</button><button (click)="setAll(0)">Clear all</button><button class="save" [disabled]="markedCount===0 || saving" (click)="save()">{{saving?'Saving…':'Save attendance'}}</button></div>
<p class="message" [class.error]="isError" *ngIf="message">{{message}}</p>
<article *ngFor="let s of attendance;let i=index"><span>{{i+1}}</span><div><b>{{s.studentName}}</b><small>{{s.email}}</small></div><div class="tri"><button [class.present]="statusOf(s)===1" (click)="setStatus(s,1)">Present</button><button [class.absent]="statusOf(s)===2" (click)="setStatus(s,2)">Absent</button><button [class.unmarked]="statusOf(s)===0" (click)="setStatus(s,0)">Not marked</button></div></article></section>`,
  styles: [`.register{font-family:Inter,Arial}.register>div:first-child p{font-size:10px;letter-spacing:1px;color:#7190ae;font-weight:800;margin:0}.register h2{font-size:24px;margin:6px 0}.register>div:first-child span{font-size:12px;color:#74819a}.filters{display:flex;gap:10px;margin:22px 0}.filters select,.filters input{flex:1;padding:11px;border:1px solid #dce7f2;border-radius:9px;background:white}.toolbar{display:flex;gap:9px;align-items:center;padding:12px 0;flex-wrap:wrap}.toolbar b{font-size:12px;margin-right:auto}.toolbar button,.tri button{border:0;border-radius:8px;padding:8px 11px;background:#edf5fb;color:#4b7c9b;font-size:11px;font-weight:700;cursor:pointer}.toolbar .save{background:linear-gradient(100deg,#36d9d5,#6194f6);color:#fff}.toolbar .save[disabled]{opacity:.5;cursor:not-allowed}.register article{display:grid;grid-template-columns:28px 1fr auto;gap:10px;align-items:center;padding:13px;margin:8px 0;border:1px solid #e5edf6;border-radius:13px;background:#ffffffc9}.register article>span{font-size:11px;color:#7d8ca2}.register article b,.register article small{display:block}.register article b{font-size:12px}.register article small{font-size:10px;color:#74819a;margin-top:3px}.tri{display:flex;gap:6px}.tri button.present{background:#dffaf3;color:#218e75}.tri button.absent{background:#ffe9ed;color:#ba5363}.tri button.unmarked{background:#eef1f6;color:#5a6b85;outline:2px solid #c7d2e0}.message{padding:10px;background:#e1faf2;color:#218e75;border-radius:8px;font-size:12px}.message.error{background:#ffe9ed;color:#ba5363}@media(max-width:650px){.filters{flex-direction:column}.toolbar{flex-wrap:wrap}.toolbar b{width:100%}}`]
})
export class AttendanceRegisterComponent implements OnInit {
  classes: AcademicClass[] = []; subjects: Subject[] = []; attendance: AttendanceRow[] = [];
  classId = 0; subjectId = 0; date = localToday();
  message = ''; isError = false; saving = false;

  constructor(private api: AcademicService) {}
  ngOnInit() { this.api.classes().subscribe(x => this.classes = x); }

  onClassChange() { this.subjectId = 0; this.subjects = []; this.attendance = []; this.message = ''; if (this.classId) this.api.subjects(this.classId).subscribe(x => this.subjects = x); }

  loadAttendance() {
    this.message = '';
    if (!this.subjectId || !this.date) { this.attendance = []; return; }
    this.api.attendance(this.subjectId, this.date).subscribe({ next: x => this.attendance = x, error: e => this.fail(e) });
  }

  statusOf(s: AttendanceRow) { return typeof s.status === 'number' ? s.status : (s.status === 'Present' ? 1 : s.status === 'Absent' ? 2 : 0); }
  setStatus(s: AttendanceRow, v: number) { (s as any).status = v; }
  setAll(v: number) { this.attendance.forEach(s => (s as any).status = v); }

  get markedCount() { return this.attendance.filter(s => this.statusOf(s) !== 0).length; }

  save() {
    if (!this.subjectId) { this.message = 'Select a course first.'; this.isError = true; return; }
    if (!this.attendance.length) return;
    this.saving = true;
    const entries = this.attendance.map(s => ({ studentId: s.studentId, status: this.statusOf(s) }));
    this.api.saveAttendance(this.subjectId, this.date, entries).subscribe({
      next: () => { this.saving = false; this.isError = false; this.message = `Attendance saved for ${this.markedCount} of ${this.attendance.length} students on ${this.date}.`; },
      error: e => { this.saving = false; this.fail(e); }
    });
  }

  fail(e: any) { this.isError = true; this.message = e?.error?.message || e?.error || e?.message || 'Could not save attendance.'; }
}
