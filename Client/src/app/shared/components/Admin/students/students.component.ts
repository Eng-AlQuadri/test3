import { Component, inject, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmationService } from 'primeng/api';
import { PaginatorModule } from 'primeng/paginator';
import { StudentService } from '../../../../core/services/student.service';
import { Pagination } from '../../../models/Pagination';
import { Student } from '../../../models/Student';
import { StudentParams } from '../../../models/StudentParams';
import { CreateStudent } from '../../../models/createStudent';
import { UpdateStudent } from '../../../models/UpdateStudent';
import { DialogService } from '../../../../core/services/dialog.service';
import { CreateStudentDialogComponent } from "./create-student-dialog/create-student-dialog.component";
import { SplitButtonModule } from 'primeng/splitbutton';
import { MenuItem } from 'primeng/api';
import { UpdateStudentDialogComponent } from "./update-student-dialog/update-student-dialog.component";

@Component({
  selector: 'app-students',
  imports: [
    FormsModule,
    CommonModule,
    RouterModule,
    ConfirmPopupModule,
    PaginatorModule,
    CreateStudentDialogComponent,
    SplitButtonModule,
    UpdateStudentDialogComponent
],
  templateUrl: './students.component.html',
  styleUrl: './students.component.css',
  providers: [ConfirmationService]
})
export class StudentsComponent implements OnInit{

  private toastr = inject(ToastrService);

  private studentService = inject(StudentService);

  private confirmationService = inject(ConfirmationService);

  private dialogService = inject(DialogService);

  students?: Pagination<Student>;

  items: MenuItem[];

  sortOptions = [
    {name: 'A - Z', value: 'nameAsc'},
    {name: 'Z - A', value: 'nameDesc'},
    {name: 'Last Active', value: 'lastActive'},
  ];

  studentParams = new StudentParams();

  constructor() {
    this.items = [
        {
            label: 'A - Z',
            command: () => {
              this.studentParams.sort = 'nameAsc';
              this.loadStudents();
            }
        },
        {
            label: 'Z - A',
            command: () => {
                this.studentParams.sort = 'nameDesc';
                this.loadStudents();
            }
        },
        {
            label: 'Last Active',
            command: () => {
                this.studentParams.sort = 'lastActive';
                this.loadStudents();
            }
        },
    ];
  }

  ngOnInit(): void {
      this.loadStudents();
  }

  loadStudents(): void {
    this.studentService.getStudents(this.studentParams).subscribe({
      next: (result) => this.students = result,
      error: (error) => console.log(error)
    });
  }

  onSearchChange() {
    this.studentParams.pageNumber = 1;
    this.loadStudents();
  }

  deletePatient(id: number): void {
    this.studentService.deleteStudent(id).subscribe(() => {
      this.toastr.success("Deleted Successfully");
      this.students!.data = this.students!.data.filter(x => x.id !== id);
    })
  }

  confirmDelete(event: Event, patientId: number) {
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: 'Do you want to delete this record?',
      icon: 'pi pi-info-circle',
      rejectButtonProps: {
          label: 'Cancel',
          severity: 'secondary',
          outlined: true
      },
      acceptButtonProps: {
          label: 'Delete',
          severity: 'danger'
      },
      accept: () => {
        this.deletePatient(patientId);
      },
      reject: () => {
          this.toastr.error("You have rejected!");
      }
  });
  }

  createStudent(newStudent: CreateStudent) {
    this.studentService.createStudent(newStudent).subscribe({
      next: (student) => {
        this.toastr.success("Created successfully");
        this.students?.data.unshift(student);
      },
      error: (error) => console.log(error)
    })
  }

  showCreateDialog() {
    this.dialogService.showCreateStudentDialog();
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

    this.studentService.updateStudent(updatedStudent).subscribe({
      next: () => this.toastr.success("Updated successfully"),
      error: () => this.dialogService.showEditStudentDialog(student)
    })
  }

  onPageChange(event: any): void {
    this.studentParams.pageNumber = event.page + 1;
    this.studentParams.pageSize = event?.rows;
    this.loadStudents();
  }
}
