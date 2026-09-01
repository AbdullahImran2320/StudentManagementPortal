import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from './services/auth.service';
import { PrincipalAcademicComponent } from './components/principal-academic/principal-academic.component';
import { TeacherAcademicComponent } from './components/teacher-academic/teacher-academic.component';
import { StudentAcademicComponent } from './components/student-academic/student-academic.component';

@Component({selector:'app-root',standalone:true,imports:[CommonModule,FormsModule,PrincipalAcademicComponent,TeacherAcademicComponent,StudentAcademicComponent],templateUrl:'./app.html',styleUrl:'./app.css'})
export class AppComponent {
  loggedIn=false; authMode:'login'|'register'='login'; authError=''; authNotice=''; email='';password='';name='';phone='';requestedRole='Student'; dark=false;
  constructor(public auth:AuthService){this.loggedIn=auth.isLoggedIn()}
  submitAuth(){this.authError='';this.authNotice='';if(this.authMode==='login'){this.auth.login({email:this.email,password:this.password}).subscribe({next:()=>{this.loggedIn=true},error:e=>this.fail(e)})}else{this.auth.register({name:this.name,email:this.email,password:this.password,phone:this.phone,role:this.requestedRole}).subscribe({next:()=>{this.authMode='login';this.password='';this.authNotice='Registration submitted. A principal/admin must approve the account before login.'},error:e=>this.fail(e)})}}
  fail(e:any){
    if(e?.error?.message) this.authError=e.error.message;
    else if(typeof e?.error==='string' && e.error.trim()) this.authError=e.error;
    else if(e?.error?.errors) this.authError=Object.values(e.error.errors).flat().join(' ');
    else if(e?.status===0) this.authError='Could not reach the server. Check your connection and that the API is running.';
    else if(e?.message) this.authError=e.message;
    else this.authError='Unable to complete the request.';
  }
  logout(){this.auth.logout();this.loggedIn=false;this.email='';this.password=''}
}
