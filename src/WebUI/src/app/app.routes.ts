import { Routes } from '@angular/router';
import { AuthenticationComponent } from './authentication/authentication.component';
import { ForumsComponent } from './forums/forums.component';
import { RegistrationComponent } from './authentication/registration/registration.component';
import { AccountComponent } from './account/account.component';
import { ForumCreateComponent } from './forums/forum-create/forum-create.component';
import { ForumViewComponent } from './forums/forum-view/forum-view.component';
import { ForumUpdateComponent } from './forums/forum-update/forum-update.component';

export const routes: Routes = [
    { path: '', pathMatch: 'full', component: ForumsComponent },
    { path: 'login', component: AuthenticationComponent },
    { path: 'registration', component: RegistrationComponent },
    { path: 'profile', component: AccountComponent },
    { path: 'forums/create', component: ForumCreateComponent },
    { path: 'forums', component: ForumsComponent },
    { path: 'forums/:forumId', component: ForumViewComponent },
    { path: 'forums/:forumId/update', component: ForumUpdateComponent },
];
