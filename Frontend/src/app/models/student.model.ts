export interface Student {
  id        : number;
  name      : string;
  email     : string;
  gpa       : number;
  city      : string;
  course    : string;
  enrollDate: string;
  isActive  : boolean;
}

export interface CreateStudent {
  name  : string;
  email : string;
  gpa   : number;
  city  : string;
  course: string;
}

export interface UpdateStudent {
  name  : string;
  gpa   : number;
  city  : string;
  course: string;
}

export interface Stats {
  totalStudents : number;
  activeStudents: number;
  averageGPA    : number;
}
