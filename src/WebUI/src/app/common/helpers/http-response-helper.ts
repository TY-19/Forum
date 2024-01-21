import { HttpErrorResponse } from "@angular/common/http";

export class HttpResponseHelper {
  static getErrorFromBadRequest(response: HttpErrorResponse): string {
    let error = (response as HttpErrorResponse).error;
    let message = error.message;
    for(let e of error.errors) {
      message += "\n" + e;
    }
    return message;
  }
}