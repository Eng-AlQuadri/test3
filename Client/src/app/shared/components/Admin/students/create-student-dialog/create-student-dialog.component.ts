import { Component, EventEmitter, inject, OnInit, Output, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DialogService } from '../../../../../core/services/dialog.service';
import { CreateStudent } from '../../../../models/createStudent';

@Component({
  selector: 'app-create-student-dialog',
  imports: [
    Dialog,
    ButtonModule,
    InputTextModule,
    CommonModule,
    FormsModule
  ],
  templateUrl: './create-student-dialog.component.html',
  styleUrl: './create-student-dialog.component.css'
})
export class CreateStudentDialogComponent {

  dialogService = inject(DialogService);

  @Output() studentCreated = new EventEmitter<any>();

  @ViewChild('editForm') editForm!: NgForm;

  student: CreateStudent | null | undefined = {
    email: '',
    userName: ''
  };


  get visible(): boolean {  // Use a getter to keep it reactive
    return this.dialogService.dialogVisible();
  }

  createStudent() {
    this.studentCreated.emit(this.student);
    this.dialogService.hideDialog();
  }

  closeDialog() {
    this.dialogService.hideDialog();
    this.editForm.reset();
  }
}
