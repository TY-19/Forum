import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject, tap } from "rxjs";
import { LoginRequest } from "../common/models/login-request";
import { baseUrl } from "../app.config";
import { LoginResponse } from "../common/models/login-response";

@Injectable({
    providedIn: 'root',
})

export class AuthenticationService {

    readonly tokenKey: string = "token";
    readonly userNameKey: string = "userName";
    readonly userProfileIdKey: string = "userProfileId";
    readonly rolesKey: string = "roles";

    private _authStatus = new Subject<boolean>();
    public authStatus = this._authStatus.asObservable();

    constructor(private http: HttpClient) {
        this._authStatus.next(this.isAuthenticated());

    }

    getToken(): string | null {
        return localStorage.getItem(this.tokenKey);
    }

    isAuthenticated(): boolean {
        return this.getToken() != null;
    }

    login(request: LoginRequest): Observable<LoginResponse> {
        const url: string = baseUrl + "/api/account/login";
        return this.http.post<LoginResponse>(url, request)
            .pipe(tap(response => {
                if(response.succeeded && response.token)
                {
                    this._authStatus.next(true);
                    this.writeToLocalStorage(response);
                }
            }));
    }

    logout(): void {
        this.removeUserItemsFromLocalStorage();
        this._authStatus.next(false);
    }

    private writeToLocalStorage(response: LoginResponse): void {
        if(response.token) {
            localStorage.setItem(this.tokenKey, response.token);
        }
        if(response.userName) {
            localStorage.setItem(this.userNameKey, response.userName)
        }
        if(response.userProfileId) {
            localStorage.setItem(this.userProfileIdKey, response.userProfileId.toString())
        }
        localStorage.setItem(this.rolesKey, JSON.stringify(response.roles));
    }

    private removeUserItemsFromLocalStorage(): void {
        localStorage.removeItem(this.tokenKey);
        localStorage.removeItem(this.userNameKey);
        localStorage.removeItem(this.userProfileIdKey);
        localStorage.removeItem(this.rolesKey);
    }
}