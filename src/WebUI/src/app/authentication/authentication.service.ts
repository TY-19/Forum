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

    private readonly tokenKey: string = "token";

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
                    localStorage.setItem(this.tokenKey, response.token);
                }
            }));
    }

    logout(): void {
        localStorage.removeItem(this.tokenKey);
    }
}