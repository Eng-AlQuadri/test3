import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { StudentParams } from '../../shared/models/StudentParams';
import { Pagination } from '../../shared/models/Pagination';
import { Student } from '../../shared/models/Student';
import { UpdateStudent } from '../../shared/models/UpdateStudent';
import { CreateStudent } from '../../shared/models/createStudent';

@Injectable({
  providedIn: 'root'
})
export class StudentService {

  baseUrl: string = environment.apiUrl;

  private http = inject(HttpClient);

  getStudents(studentParams: StudentParams) {

    let params = new HttpParams();

    if (studentParams.search) {
      params = params.append('search', studentParams.search);
    }

    if (studentParams.sort) {
      params = params.append('sort', studentParams.sort);
    }

    params = params.append('pageSize', studentParams.pageSize);

    params = params.append('pageIndex', studentParams.pageNumber);

    return this.http.get<Pagination<Student>>(this.baseUrl + 'students', {params});
  }

  deleteStudent(id: number) {
    return this.http.delete(this.baseUrl + 'students/' + id);
  }

  updateStudent(student: UpdateStudent) {
    return this.http.put(this.baseUrl + 'students', student);
  }

  updateMyProfile(student: UpdateStudent) {
    return this.http.put(this.baseUrl + 'students/my-profile', student);
  }

  createStudent(student: CreateStudent) {
    return this.http.post<Student>(this.baseUrl + 'students', student);
  }

  getMyAccount() {
    return this.http.get<Student>(this.baseUrl + 'students/my-profile');
  }
}
