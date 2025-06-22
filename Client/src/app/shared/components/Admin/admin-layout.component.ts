import { Component, inject } from '@angular/core';
import { AccountService } from '../../../core/services/account.service';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { User } from '../../models/User';

@Component({
  selector: 'app-admin-layout',
  imports: [
    RouterModule
  ],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.css'
})
export class AdminLayoutComponent {

  accountService = inject(AccountService);

  router = inject(Router);

  currentUser?: User | null;

  isAsideOpen: boolean = true;
  isPageContentOpen: boolean = false;

  handleIconClick() {
    this.isAsideOpen = !this.isAsideOpen;
    this.isPageContentOpen = !this.isPageContentOpen;
  }


  ngOnInit(): void {
    this.currentUser = this.accountService.currentUser();
    if (!this.currentUser) this.router.navigateByUrl('/login');
    if (this.currentUser?.role !== 'admin') this.router.navigateByUrl('/login');
  }

  logout(): void {
    this.accountService.logout();
    this.router.navigateByUrl('/login');
  }
}
