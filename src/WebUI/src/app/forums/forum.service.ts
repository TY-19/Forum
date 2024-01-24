import { HttpClient, HttpResponse, HttpResponseBase } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ForumCreate } from "../common/models/forum-create";
import { baseUrl } from "../app.config";
import { Observable } from "rxjs";
import { Forum } from "../common/models/forum";

@Injectable({
    providedIn: 'root',
})

export class ForumService {
    constructor(private http: HttpClient) {

    }
    
    createForum(forum: ForumCreate): Observable<HttpResponse<Forum>> {
        const url = baseUrl + "/api/forums";
        return this.http.post<Forum>(url, forum, { observe: 'response'});
    }
}