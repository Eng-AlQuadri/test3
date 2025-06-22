import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy.service';
import { delay, finalize, switchMap, timer } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {

  const busyService = inject(BusyService);

  busyService.busy();

  return timer(1000).pipe(
    switchMap(() => next(req).pipe(finalize(() => busyService.idle())))
  );
};
