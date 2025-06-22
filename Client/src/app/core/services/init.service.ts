import { inject, Injectable } from '@angular/core';
import { AccountService } from './account.service';
import { User } from '../../shared/models/User';

@Injectable({
  providedIn: 'root'
})
export class InitService {

  accountService = inject(AccountService);

  setCurrentUser() {

    const jsonUser = localStorage.getItem('schoolNet.user');

    if (jsonUser) {

      const user: User = JSON.parse(jsonUser);

      this.accountService.currentUser.set(user);
    }
  }
}
