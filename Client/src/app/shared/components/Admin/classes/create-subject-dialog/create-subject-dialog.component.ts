import { Component, EventEmitter, inject, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { DialogService } from '../../../../../core/services/dialog.service';
import { CreateSubject } from '../../../../models/CreateSubject';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-create-subject-dialog',
  imports: [
    Dialog,
    ButtonModule,
    InputTextModule,
    CommonModule,
    FormsModule
  ],
  templateUrl: './create-subject-dialog.component.html',
  styleUrl: './create-subject-dialog.component.css'
})
export class CreateSubjectDialogComponent {

  dialogService = inject(DialogService);

  @Output() subjectCreated = new EventEmitter<any>();

  @ViewChild('editForm') editForm!: NgForm;

  subject: CreateSubject | null | undefined = {
    name: '',
    minMark: 0
  };


  get visible(): boolean {  // Use a getter to keep it reactive
    return this.dialogService.addNewSubjectDialogVisible();
  }

  createSubject() {
    this.subjectCreated.emit(this.subject);
    this.dialogService.hideDialog();
  }

  closeDialog() {
    this.dialogService.hideDialog();
    this.editForm.reset();
  }
}
