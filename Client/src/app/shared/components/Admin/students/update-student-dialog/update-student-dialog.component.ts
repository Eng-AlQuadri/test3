import { Component, effect, EventEmitter, inject, OnInit, Output, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DialogService } from '../../../../../core/services/dialog.service';
import { CreateStudent } from '../../../../models/createStudent';
import { forkJoin } from 'rxjs';
import { Student } from '../../../../models/Student';

@Component({
  selector: 'app-update-student-dialog',
  imports: [
    Dialog,
    ButtonModule,
    InputTextModule,
    CommonModule,
    FormsModule
  ],
  templateUrl: './update-student-dialog.component.html',
  styleUrl: './update-student-dialog.component.css'
})
export class UpdateStudentDialogComponent {
  dialogService = inject(DialogService);

  @Output() studentUpdated = new EventEmitter<any>();

  @ViewChild('editForm') editForm!: NgForm;

  initialStudent?: Student | null = null;

  constructor() {
    effect(() => {
      const selectedStudent = this.dialogService.selectedStudent();
      if (selectedStudent && !this.initialStudent) {
        this.initialStudent = JSON.parse(JSON.stringify(selectedStudent)); // Deep clone once
      }
    });
  }


  get visible(): boolean {  // Use a getter to keep it reactive
    return this.dialogService.editStudentDialogVisible();
  }

  get student() {
    return this.dialogService.selectedStudent();
  }

  updateStudent() {
    this.studentUpdated.emit(this.student);
    this.initialStudent = null;
    this.dialogService.hideDialog();
  }

  closeDialog() {
    this.editForm.reset({...this.initialStudent});
    this.initialStudent = null;
    this.dialogService.hideDialog();
  }
}
