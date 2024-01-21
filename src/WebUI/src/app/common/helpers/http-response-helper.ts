import { HttpErrorResponse } from "@angular/common/http";

export class HttpResponseHelper {
  static getErrorFromBadRequest(response: HttpErrorResponse): string {
    let error = (response as HttpErrorResponse).error;
    if(error.type === "https://tools.ietf.org/html/rfc7231#section-6.5.1") {
      let message = error.title;
      for(let e in error.errors) {
        console.log(e);
        message += "\n" + error.errors[e];
      }
      return message;
    }
    return error.toString();
  }
}