import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authenticationInterceptor } from './authentication/authentication.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([
      authenticationInterceptor
    ])),
    provideRouter(routes),
  ]
};

export const baseUrl = "https://localhost:40443";
