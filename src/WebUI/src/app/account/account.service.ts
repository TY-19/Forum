import { HttpClient, HttpResponseBase } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { User } from "../common/models/user";
import { baseUrl } from "../app.config";
import { ProfileUpdate } from "../common/models/profile-update";

@Injectable({
    providedIn: 'root',
})

export class AccountService {
    constructor(private http: HttpClient) {

    }

    getProfile(): Observable<User> {
        const url = baseUrl + "/api/account/view";
        return this.http.get<User>(url);
    }

    updateProfile(profile: ProfileUpdate): Observable<HttpResponseBase> {
        const url = baseUrl + "/api/account/update";
        return this.http.put<HttpResponseBase>(url, profile, {observe: 'response'});
    }
}