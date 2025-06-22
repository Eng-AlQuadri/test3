import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Photo } from '../../shared/models/Student';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {

  private http = inject(HttpClient);

  baseUrl: string = environment.apiUrl;

  mainPhoto = signal<string>('');

  setPhotoOnScreen(photoUrl: string) {
    this.mainPhoto.set(photoUrl);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'account/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'account/delete-photo/' + photoId);
  }

  uploadPhoto(formData: FormData) {
    return this.http.post<Photo>(this.baseUrl + 'account/add-photo', formData);
  }
}
