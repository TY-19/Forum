import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { catchError, tap, throwError } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

export const authenticationInterceptor: HttpInterceptorFn = (req, next) => {  
  const authenticationService = inject(AuthenticationService);
  const token = authenticationService.getToken();

  console.log(req.headers);

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }



  return next(req);

};
