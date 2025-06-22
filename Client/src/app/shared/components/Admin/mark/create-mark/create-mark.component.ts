import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DialogService } from '../../../../../core/services/dialog.service';
import { CreateStudent } from '../../../../models/createStudent';
import { CreateMark } from '../../../../models/CreateMark';
import { SubjectService } from '../../../../../core/services/subject.service';

@Component({
  selector: 'app-create-mark',
  imports: [
    Dialog,
    ButtonModule,
    InputTextModule,
    CommonModule,
    FormsModule
  ],
  templateUrl: './create-mark.component.html',
  styleUrl: './create-mark.component.css'
})
export class CreateMarkComponent implements OnInit {

  dialogService = inject(DialogService);

  subjectService = inject(SubjectService);

  @Output() markCreated = new EventEmitter<any>();

  @ViewChild('editForm') editForm!: NgForm;

  mark: CreateMark | null | undefined = {
    subjectName: '',
    gainedMark: 0,
    studentId: 0
  };

  subjects: string[] = [];

  ngOnInit(): void {
    this.subjectService.getSubjects().subscribe((res) => this.subjects = res);
  }

  get visible(): boolean {  // Use a getter to keep it reactive
    return this.dialogService.markDialogVisible();
  }

  createMark() {
    this.markCreated.emit(this.mark);
    this.dialogService.hideDialog();
  }

  closeDialog() {
    this.dialogService.hideDialog();
    this.editForm.reset();
  }
}
