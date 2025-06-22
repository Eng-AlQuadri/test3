import { Component, inject } from '@angular/core';
import { DialogService } from '../../../../core/services/dialog.service';
import { CreateSubject } from '../../../models/CreateSubject';
import { ToastrService } from 'ngx-toastr';
import { AssignSubject } from '../../../models/AssignSubject';
import { SubjectService } from '../../../../core/services/subject.service';
import { CreateSubjectDialogComponent } from "./create-subject-dialog/create-subject-dialog.component";
import { AssignSubjectDialogComponent } from "./assign-subject-dialog/assign-subject-dialog.component";

@Component({
  selector: 'app-classes',
  imports: [CreateSubjectDialogComponent, AssignSubjectDialogComponent],
  templateUrl: './classes.component.html',
  styleUrl: './classes.component.css'
})
export class ClassesComponent {

  private dialogService = inject(DialogService);

  private toastr = inject(ToastrService);

  private subjectService = inject(SubjectService);

  showAddNewSubjectDialog() {
    this.dialogService.showAddNewSubjectDialog();
  }

  showAssignSubjectToStudentDialog() {
    this.dialogService.showAssignSubjectToStudentDialog();
  }

  createSubject(subject: CreateSubject) {
    this.subjectService.createSubject(subject).subscribe({
      next: () => this.toastr.success("Created successfully")
    })
  }

  assignSubjectToStudent(assignSubject: AssignSubject) {
    this.subjectService.assignSubjectToStudent(assignSubject).subscribe({
      next: () => this.toastr.success("Subject assigned successfully")
    })
  }
}
