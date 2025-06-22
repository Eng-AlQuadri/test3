import { Component, inject, Input } from '@angular/core';
import { Photo, Student } from '../../../models/Student';
import { AccountService } from '../../../../core/services/account.service';
import { PhotoService } from '../../../../core/services/photo.service';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FileUpload } from 'primeng/fileupload';

@Component({
  selector: 'app-photo-editor',
  imports: [
    FormsModule,
    CommonModule,
    FileUpload
  ],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css'
})
export class PhotoEditorComponent {

  @Input() student?: Student;

  private accountService = inject(AccountService);

  private photoService = inject(PhotoService);

  private toastr = inject(ToastrService);

  currentUser = this.accountService.currentUser();

  uploadedFiles: any[] = [];

  setMainPhoto(photo: Photo) {
    this.photoService.setMainPhoto(photo.id).subscribe({
      next: () => {
        this.currentUser!.photoUrl = photo.url;
        this.accountService.setCurrentUser(this.currentUser!);
        this.student?.photos.forEach(p => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        })
      }
    })
  }

  deletePhoto(photoId: number) {
    this.photoService.deletePhoto(photoId).subscribe({
      next: () => {
        this.student!.photos = this.student!.photos.filter(x => x.id !== photoId);
      }
    })
  }

  onUpload(event: any, fileUpload: any) {

    const formData = new FormData();

    this.uploadedFiles.push(event.files[0]);

    formData.append('file', event.files[0]);

    this.photoService.uploadPhoto(formData).subscribe({
      next: (response) => {
        fileUpload.clear();
        this.toastr.success('Uploaded Successfully');
        const photo: Photo = response;
        this.student?.photos.push(photo);

        if (photo.isMain) {
          this.currentUser!.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.currentUser!);
        }
      }
    })
  }
}
