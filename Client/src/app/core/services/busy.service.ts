import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {

  busyRequestCount: number = 0;

  loading: boolean = false;

  spinnerService = inject(NgxSpinnerService);

  busy() {
    this.loading = true;
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'line-spin-fade-rotating',
      bdColor: "rgba(255,255,255,0)",
      color: "#333333"
    });
  }

  idle() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.loading = false;
      this.spinnerService.hide();
    }
  }

  constructor() { }
}
