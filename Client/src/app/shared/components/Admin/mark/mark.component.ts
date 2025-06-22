import { Component, inject } from '@angular/core';
import { SubjectService } from '../../../../core/services/subject.service';
import { ToastrService } from 'ngx-toastr';
import { DialogService } from '../../../../core/services/dialog.service';
import { CreateMark } from '../../../models/CreateMark';
import { CreateMarkComponent } from "./create-mark/create-mark.component";

@Component({
  selector: 'app-mark',
  imports: [CreateMarkComponent],
  templateUrl: './mark.component.html',
  styleUrl: './mark.component.css'
})
export class MarkComponent {

  private subjectService = inject(SubjectService);

  private toastr = inject(ToastrService);

  private dialogService = inject(DialogService);

  showAddMarkDialog() {
    this.dialogService.showMarkDialog();
  }

  createMark(mark: CreateMark) {
    this.subjectService.createMark(mark).subscribe(() => this.toastr.success("Mark assigned successfully"));
  }
}
