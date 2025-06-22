import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { CreateSubject } from '../../shared/models/CreateSubject';
import { AssignSubject } from '../../shared/models/AssignSubject';
import { CreateMark } from '../../shared/models/CreateMark';
import { StudentSubjects } from '../../shared/models/StudentSubjects';

@Injectable({
  providedIn: 'root'
})
export class SubjectService {

  baseUrl: string = environment.apiUrl;

  private http = inject(HttpClient);

  createSubject(subject: CreateSubject) {
    return this.http.post(this.baseUrl + 'subjects', subject);
  }

  assignSubjectToStudent(data: AssignSubject) {
    return this.http.post(this.baseUrl + 'subjects/assign/' + data.subjectName + '/student/' + data.studentId, {});
  }

  createMark(mark: CreateMark) {
    return this.http.post(this.baseUrl + 'marks', mark);
  }

  getSubjects() {
    return this.http.get<string[]>(this.baseUrl + 'subjects');
  }

  getSubjectsForStudent() {
    return this.http.get<StudentSubjects[]>(this.baseUrl + 'subjects/student-subjects');
  }
}
