import { Injectable } from "@angular/core";
import { AuthenticationService } from "./authentication.service";
import { Role } from "../common/models/role";
import { takeUntil } from "rxjs";

@Injectable({
    providedIn: 'root',
})

export class CurrentUserService {
    
    get userName(): string | null {
        return localStorage.getItem(this.authService.userNameKey);
    }

    get userProfileId(): string | null {
        return localStorage.getItem(this.authService.userProfileIdKey);
    }

    get roles(): Role[] | null {
        const roles = localStorage.getItem(this.authService.rolesKey);
        if(roles) {
            return JSON.parse(roles);
        } else {
            return [];
        }
    }

    constructor(private authService: AuthenticationService) {
        
    }

}