import { Routes } from '@angular/router';
import { AuthenticationComponent } from './authentication/authentication.component';
import { ForumsComponent } from './forums/forums.component';

export const routes: Routes = [
    { path: '', pathMatch: 'full', component: ForumsComponent },
    { path: 'login', component: AuthenticationComponent }
];
