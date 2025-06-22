import { Injectable, signal } from '@angular/core';
import { Student } from '../../shared/models/Student';
import { single } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  public dialogVisible = signal<boolean>(false);

  public editStudentDialogVisible = signal<boolean>(false);

  public addNewSubjectDialogVisible = signal<boolean>(false);

  public assignSubjectToStudentVisible = signal<boolean>(false);

  public markDialogVisible = signal<boolean>(false);

  public selectedStudent = signal<Student | null | undefined>(null);

  showCreateStudentDialog() {
    this.dialogVisible.set(true);
  }

  showEditStudentDialog(student: Student) {
    this.editStudentDialogVisible.set(true);
    this.selectedStudent.set(student);
  }

  showAddNewSubjectDialog() {
    this.addNewSubjectDialogVisible.set(true);
  }

  showAssignSubjectToStudentDialog() {
    this.assignSubjectToStudentVisible.set(true);
  }

  showMarkDialog() {
    this.markDialogVisible.set(true);
  }

  hideDialog() {
    this.dialogVisible.set(false);
    this.addNewSubjectDialogVisible.set(false);
    this.assignSubjectToStudentVisible.set(false);
    this.markDialogVisible.set(false);
    this.selectedStudent.set(null);
  }
}
