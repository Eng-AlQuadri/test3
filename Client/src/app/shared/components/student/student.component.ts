import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { User } from '../../models/User';
import { PresenceService } from '../../../core/services/presence.service';

@Component({
  selector: 'app-student',
  imports: [
    RouterModule
  ],
  templateUrl: './student.component.html',
  styleUrl: './student.component.css'
})
export class StudentComponent {

  accountService = inject(AccountService);

  private presenceService = inject(PresenceService);

  router = inject(Router);

  currentUser?: User | null;

  isAsideOpen: boolean = true;

  isPageContentOpen: boolean = false;

  messageCounter: number = 0;

  handleIconClick() {
    this.isAsideOpen = !this.isAsideOpen;
    this.isPageContentOpen = !this.isPageContentOpen;
  }


  ngOnInit(): void {
    this.currentUser = this.accountService.currentUser();
    this.presenceService.createHubConnection(this.accountService.currentUser()!);
    if (!this.currentUser) this.router.navigateByUrl('/login');
    if (this.currentUser?.role !== 'student') this.router.navigateByUrl('/login');
  }

  logout(): void {
    this.accountService.logout();
    this.router.navigateByUrl('/login');
  }
}
