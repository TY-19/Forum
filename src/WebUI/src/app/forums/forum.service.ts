import { HttpClient, HttpResponse, HttpResponseBase } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ForumCreate } from "../common/models/forum-create";
import { baseUrl } from "../app.config";
import { Observable } from "rxjs";
import { Forum } from "../common/models/forum";
import { ForumsStructure } from "../common/models/forums-structure";

@Injectable({
    providedIn: 'root',
})

export class ForumService {
    constructor(private http: HttpClient) {

    }

    getForumsStructure(): Observable<ForumsStructure[]> {
        const url = baseUrl + "/api/forums/structure";
        return this.http.get<ForumsStructure[]>(url);
    }
    
    getForums(): Observable<Forum> {
        const url = baseUrl + "/api/forums";
        return this.http.get<Forum>(url);
    }

    createForum(forum: ForumCreate): Observable<HttpResponse<Forum>> {
        let url = baseUrl + "/api/forums";
        if(forum.parentForumId) {
            url += "/" + forum.parentForumId;
        }
        return this.http.post<Forum>(url, forum, { observe: 'response'});
    }
}