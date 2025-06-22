import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { InputTextComponent } from "../input-text/input-text.component";

@Component({
  selector: 'app-guest-layout',
  imports: [
    InputTextComponent,
    ReactiveFormsModule
  ],
  templateUrl: './guest-layout.component.html',
  styleUrl: './guest-layout.component.css'
})
export class GuestLayoutComponent implements OnInit {

    private formBuilder = inject(FormBuilder);

    private router = inject(Router);

    private accountService = inject(AccountService);

    loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(4)]]
    });

    ngOnInit(): void {
      if (this.accountService.currentUser())
        this.redirectTo(this.accountService.currentUser()?.role);
    }

    login() {
      this.accountService.login(this.loginForm.value).subscribe(user => {
        this.redirectTo(user.role)
      })
    }

    redirectTo(role?: string) {
      switch(role?.toLowerCase()) {
        case 'admin': this.router.navigateByUrl('/admin'); break;
        case 'student': this.router.navigateByUrl('/student'); break;
      }
    }
}
