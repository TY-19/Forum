import { HttpErrorResponse } from "@angular/common/http";

export class HttpResponseHelper {
  static getErrorFromBadRequest(response: HttpErrorResponse): string {
    let error = (response as HttpErrorResponse).error;
    let message = error.message;
    if(Symbol.iterator in Object(error.errors)) {
      for(let e of error.errors) {
        message += "\n" + e;
      }
    } else
    {
      for(let prop in error.errors) {
        message += "\n" + error.errors[prop];
      }
    }
    return message;
  }
}