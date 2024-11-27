import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { catchError, tap, throwError } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

export const authenticationInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authenticationService = inject(AuthenticationService);
  const token = authenticationService.getToken();

  if (token) {
    req = req.clone({
      setHeaders: {
        'Authorization': `Bearer ${token}`
      }
    });
  }

  return next(req).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        authenticationService.logout();
        router.navigate(['login']);
      }
      return throwError(() => error);
    })
  );
};