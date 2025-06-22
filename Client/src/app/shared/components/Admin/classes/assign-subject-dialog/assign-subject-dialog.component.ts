import { Component, EventEmitter, inject, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { DialogService } from '../../../../../core/services/dialog.service';
import { AssignSubject } from '../../../../models/AssignSubject';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { SelectedSubject } from '../../../../models/SelectedSubject';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { SubjectService } from '../../../../../core/services/subject.service';

@Component({
  selector: 'app-assign-subject-dialog',
  imports: [
    Dialog,
    ButtonModule,
    InputTextModule,
    CommonModule,
    FormsModule,
    SelectModule,
    AutoCompleteModule
  ],
  templateUrl: './assign-subject-dialog.component.html',
  styleUrl: './assign-subject-dialog.component.css'
})
export class AssignSubjectDialogComponent implements OnInit {

  dialogService = inject(DialogService);

  subjectService = inject(SubjectService);

  @Output() subjectAssigned = new EventEmitter<any>();

  @ViewChild('editForm') editForm!: NgForm;

  subject: AssignSubject | null | undefined = {
    studentId: 0,
    subjectName: ''
  };

  subjects: string[] = [];

  studentId?: number;

  ngOnInit(): void {
    this.subjectService.getSubjects().subscribe((res) => this.subjects = res);
  }

  get visible(): boolean {  // Use a getter to keep it reactive
    return this.dialogService.assignSubjectToStudentVisible();
  }

  assignSubject() {
    this.subjectAssigned.emit(this.subject);
    this.dialogService.hideDialog();
  }

  closeDialog() {
    this.dialogService.hideDialog();
    this.editForm.reset();
  }
}
