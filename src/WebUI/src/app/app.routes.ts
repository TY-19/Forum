import { Routes } from '@angular/router';
import { AuthenticationComponent } from './authentication/authentication.component';
import { ForumsComponent } from './forums/forums.component';
import { RegistrationComponent } from './authentication/registration/registration.component';
import { AccountComponent } from './account/account.component';

export const routes: Routes = [
    { path: '', pathMatch: 'full', component: ForumsComponent },
    { path: 'login', component: AuthenticationComponent },
    { path: 'registration', component: RegistrationComponent },
    { path: 'profile', component: AccountComponent },
];
