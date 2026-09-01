import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environment/environment';

type Account = { id:number; name:string; email:string; role:string; requestedRole:string; isApproved:boolean; createdAt:string };

@Component({selector:'app-admin-users',standalone:true,imports:[CommonModule,FormsModule],template:`
<section class="admin-page">
  <div class="title"><div><p>ADMINISTRATION</p><h2>Account approvals & promotions</h2><span>Review all new registrations, assign access roles, or create staff accounts directly.</span></div><button (click)="load()">Refresh</button></div>
  <div class="notice error" *ngIf="error">{{error}}</div>
  <div class="notice ok" *ngIf="message">{{message}}</div>

  <section class="create-panel">
    <div class="section-head"><p>DIRECT ACCESS</p><h3>Create a staff account</h3><span>Skip the approval queue — instantly create a Teacher or Admin login.</span></div>
    <form (ngSubmit)="createStaff()" class="formgrid">
      <label>Full name<input [(ngModel)]="newStaff.name" name="staffName" required placeholder="Ali Hassan"></label>
      <label>Email<input type="email" [(ngModel)]="newStaff.email" name="staffEmail" required placeholder="ali@school.com"></label>
      <label>Phone<input [(ngModel)]="newStaff.phone" name="staffPhone" placeholder="03001234567"></label>
      <label>Role<select [(ngModel)]="newStaff.role" name="staffRole"><option>Teacher</option><option>Admin</option></select></label>
      <label>Password<input type="password" [(ngModel)]="newStaff.password" name="staffPassword" minlength="6" required placeholder="At least 6 characters"></label>
      <button class="primary full" type="submit">Create account</button>
    </form>
  </section>

  <article *ngFor="let user of accounts" class="account">
    <div class="initial">{{user.name.slice(0,2).toUpperCase()}}</div>
    <div class="name"><b>{{user.name}}</b><small>{{user.email}} · requested {{user.requestedRole}}</small></div>
    <span [class.pending]="!user.isApproved" [class.approved]="user.isApproved">{{user.isApproved?'Approved':'Pending approval'}}</span>
    <select #role [value]="user.role"><option>Student</option><option>Teacher</option><option>Admin</option></select>
    <button class="save" (click)="save(user,role.value,user.isApproved)">Save role</button>
    <button class="approve" (click)="save(user,user.role,!user.isApproved)">{{user.isApproved?'Suspend':'Approve'}}</button>
  </article>
  <p class="muted" *ngIf="!accounts.length">No accounts yet.</p>
</section>`,styles:[`
.admin-page{font-family:Inter,Arial}
.title{display:flex;justify-content:space-between;align-items:center;margin:5px 0 22px}
.title p,.section-head p{font-size:10px;letter-spacing:1px;color:#7190ae;font-weight:800;margin:0}
.title h2{margin:6px 0;font-size:24px}
.title span,.section-head span{font-size:12px;color:#74819a}
.title button,.save,.approve{border:0;border-radius:9px;padding:9px 12px;background:#e8f8ff;color:#287da7;font-weight:700;cursor:pointer;transition:transform .15s ease,box-shadow .15s ease,background .15s ease}
.title button:hover,.save:hover{background:#d6f0fb;box-shadow:0 4px 12px rgba(40,125,167,.25);transform:translateY(-1px)}
.title button:active,.save:active,.approve:active{transform:translateY(0)}
.create-panel{background:#ffffffc9;border:1px solid #e5edf6;border-radius:16px;padding:18px;margin-bottom:18px}
.section-head{margin-bottom:10px}
.section-head h3{margin:5px 0}
.formgrid{display:grid;grid-template-columns:1fr 1fr;gap:12px}
.formgrid label{display:flex;flex-direction:column;gap:6px;font-size:11px;font-weight:700;color:#5f7187}
.formgrid input,.formgrid select{padding:11px;border:1px solid #d8e3ed;border-radius:10px;background:white;transition:border-color .15s ease,box-shadow .15s ease}
.formgrid input:hover,.formgrid select:hover{border-color:#9bb8d1}
.formgrid input:focus,.formgrid select:focus{outline:none;border-color:#4f83c6;box-shadow:0 0 0 3px rgba(79,131,198,.15)}
.full{grid-column:1/-1}
.primary{border:0;border-radius:10px;padding:11px 14px;background:linear-gradient(100deg,#32cfc7,#648df5);color:white;font-weight:800;cursor:pointer;transition:transform .15s ease,box-shadow .15s ease,filter .15s ease}
.primary:hover{filter:brightness(1.06);box-shadow:0 8px 20px rgba(80,140,230,.35);transform:translateY(-1px)}
.primary:active{transform:translateY(0)}
.account{display:grid;grid-template-columns:42px 1fr auto 115px auto auto;gap:12px;align-items:center;padding:16px;margin:10px 0;background:#ffffffc9;border:1px solid #e5edf6;border-radius:16px;transition:box-shadow .15s ease,border-color .15s ease}
.account:hover{box-shadow:0 8px 20px rgba(40,65,95,.08);border-color:#d3e2ef}
.initial{width:38px;height:38px;display:grid;place-items:center;border-radius:11px;background:linear-gradient(135deg,#50e3dd,#72a7ff);color:#17415e;font-size:11px;font-weight:800}
.name b,.name small{display:block}
.name small{font-size:10px;color:#74819a;margin-top:3px}
.pending,.approved{font-size:10px;font-weight:700;padding:6px 8px;border-radius:7px}
.pending{color:#b06d20;background:#fff2dc}
.approved{color:#218e75;background:#e1faf2}
.account select{padding:8px;border:1px solid #dce7f2;border-radius:8px;background:white;transition:border-color .15s ease}
.account select:hover{border-color:#9bb8d1}
.approve{background:linear-gradient(100deg,#36d9d5,#6194f6);color:white}
.approve:hover{filter:brightness(1.06);box-shadow:0 4px 14px rgba(60,140,220,.3);transform:translateY(-1px)}
.notice{padding:12px;border-radius:9px;margin-bottom:12px;font-size:12px}
.notice.error{background:#ffe9ed;color:#b63d54}
.notice.ok{background:#e4f8f0;color:#257b69}
.muted{color:#8996a6;font-size:12px}
@media(max-width:750px){.account{grid-template-columns:42px 1fr;}.account>*:not(.initial):not(.name){grid-column:2}.title{align-items:flex-start}.title button{white-space:nowrap}.formgrid{grid-template-columns:1fr}}
`]})
export class AdminUsersComponent implements OnInit {
  accounts:Account[]=[]; error=''; message='';
  newStaff = { name:'', email:'', phone:'', role:'Teacher', password:'' };
  constructor(private http:HttpClient){}
  ngOnInit(){ this.load() }
  load(){ this.http.get<Account[]>(`${environment.apiUrl}/admin/users`).subscribe({next:x=>this.accounts=x,error:e=>this.fail(e,'Could not load accounts.')}) }
  save(user:Account,role:string,approved:boolean){
    this.error=''; this.message='';
    this.http.put(`${environment.apiUrl}/admin/users/${user.id}`,{role,isApproved:approved}).subscribe({
      next:()=>{ this.message=`${user.name}'s account was updated.`; this.load() },
      error:e=>this.fail(e,'Could not update account.')
    });
  }
  createStaff(){
    this.error=''; this.message='';
    if(!this.newStaff.name||!this.newStaff.email||!this.newStaff.password){ this.error='Name, email and password are required.'; return }
    this.http.post(`${environment.apiUrl}/admin/users`,this.newStaff).subscribe({
      next:()=>{ this.message=`${this.newStaff.role} account created for ${this.newStaff.name}.`; this.newStaff={name:'',email:'',phone:'',role:'Teacher',password:''}; this.load() },
      error:e=>this.fail(e,'Could not create the account.')
    });
  }
  private fail(e:any,fallback:string){
    this.message='';
    if(e?.error?.message) this.error = e.error.message;
    else if(typeof e?.error === 'string' && e.error.trim()) this.error = e.error;
    else if(e?.error?.errors) this.error = Object.values(e.error.errors).flat().join(' ');
    else if(e?.status === 0) this.error = 'Could not reach the server. Check your connection and that the API is running.';
    else if(e?.message) this.error = e.message;
    else this.error = fallback;
  }
}
