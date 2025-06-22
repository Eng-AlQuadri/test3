import { Routes } from '@angular/router';
import { GuestLayoutComponent } from './shared/components/Guest/guest-layout.component';
import { AdminLayoutComponent } from './shared/components/Admin/admin-layout.component';
import { adminGuard } from './core/guards/admin.guard';
import { StudentsComponent } from './shared/components/Admin/students/students.component';
import { ClassesComponent } from './shared/components/Admin/classes/classes.component';
import { MarkComponent } from './shared/components/Admin/mark/mark.component';
import { MyAccountComponent } from './shared/components/student/my-account/my-account.component';
import { StudentComponent } from './shared/components/student/student.component';
import { studentGuard } from './core/guards/student.guard';
import { SubjectsComponent } from './shared/components/student/subjects/subjects.component';
import { MessagesComponent } from './shared/components/student/messages/messages.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: GuestLayoutComponent
  },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    runGuardsAndResolvers: 'always',
    canActivate: [adminGuard],
    children: [
      {
        path: 'students',
        component: StudentsComponent
      },
      {
        path: 'subjects',
        component: ClassesComponent
      },
      {
        path: 'marks',
        component: MarkComponent
      }
    ]
  },
  {
    path: 'student',
    component: StudentComponent,
    runGuardsAndResolvers: 'always',
    canActivate: [studentGuard],
    children: [
      {
        path: 'subjects',
        component: SubjectsComponent
      },
      {
        path: 'my-account',
        component: MyAccountComponent
      },
      {
        path: 'messages',
        component: MessagesComponent
      }
    ]
  }
];
