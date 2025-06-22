import { Component, inject, OnInit } from '@angular/core';
import { TabsModule } from 'primeng/tabs';
import { PhotoEditorComponent } from "../photo-editor/photo-editor.component";
import { StudentService } from '../../../../core/services/student.service';
import { Student } from '../../../models/Student';
import { DialogService } from '../../../../core/services/dialog.service';
import { UpdateStudent } from '../../../models/UpdateStudent';
import { ToastrService } from 'ngx-toastr';
import { UpdateStudentDialogComponent } from "../../Admin/students/update-student-dialog/update-student-dialog.component";
import { AccountService } from '../../../../core/services/account.service';


@Component({
  selector: 'app-my-account',
  imports: [
    TabsModule,
    PhotoEditorComponent,
    UpdateStudentDialogComponent
],
  templateUrl: './my-account.component.html',
  styleUrl: './my-account.component.css'
})
export class MyAccountComponent implements OnInit{

  private studentService = inject(StudentService);

  private dialogService = inject(DialogService);

  private accountService = inject(AccountService);

  private toastr = inject(ToastrService);

  student?: Student;

  ngOnInit(): void {
    this.loadMyProfile();
  }

  loadMyProfile() {
    this.studentService.getMyAccount().subscribe({
      next: (response) => this.student = response
    })
  }

  showUpdateDialog(student: Student) {
    this.dialogService.showEditStudentDialog(student);
  }

  updateStudent(student: Student) {
    let updatedStudent: UpdateStudent = {
      updateStudentId: student.id,
      email: student.email,
      userName: student.userName
    };

    this.studentService.updateMyProfile(updatedStudent).subscribe({
      next: () => {
        this.toastr.success("Updated successfully");
        this.accountService.setCurrentUser({...this.accountService.currentUser()!, userName: updatedStudent.userName})
      },
      error: () => {
        this.dialogService.showEditStudentDialog({
          id: student.id,
          email: 'Try another email',
          photos: student.photos,
          userName: student.userName
        })
      }
    })
  }
}
