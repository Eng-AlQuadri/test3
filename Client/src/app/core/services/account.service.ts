import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal, WritableSignal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { User } from '../../shared/models/User';
import { map, Observable } from 'rxjs';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private http: HttpClient = inject(HttpClient);

  private presenceService = inject(PresenceService);

  baseUrl: string = environment.apiUrl;

  currentUser = signal<User | null>(null);

  login(data: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', data).pipe(
      map(user => {
        this.setCurrentUser(user);
        this.presenceService.createHubConnection(user);
        return user;
      })
    )
  }

  logout() {
    localStorage.removeItem('schoolNet.user');
    this.presenceService.stopHubConnection();
    this.currentUser.set(null);
  }

  setCurrentUser(user: User) {

    const role: string = this.getDecodedToken(user.token).role;

    user.role = role.toLowerCase();

    const id: number = this.getUserId(user.token).nameid;

    user.id = id;

    localStorage.setItem('schoolNet.user', JSON.stringify(user));

    this.currentUser.set(user);
  }

  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]));
  }

  getUserId(token: string) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
