import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";

import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

import { SecurityService } from './security.service';

@Injectable()
export class DataService {
  constructor(private http: HttpClient, private securityService: SecurityService) { }

  get(url: string, params?: any): Observable<Response> {
    let options = {};
    this.setHeaders(options);

    return this.http.get(url, options).pipe(tap((response: any) => response));
  }

  postWithId(url: string, data: any): Observable<Response> {
    return this.doPost(url, data, true);
  }

  post(url: string, data: any): Observable<Response> {
    return this.doPost(url, data, false);
  }

  put(url: string, data: any): Observable<Response> {
    return this.doPut(url, data, false);
  }

  putWithId(url: string, data: any, params?: any): Observable<Response> {
    return this.doPut(url, data, true, params);
  }

  private doPost(url: string, data: any, needId: boolean): Observable<Response> {
    let options = {};
    this.setHeaders(options, needId);

    return this.http.post(url, data, options).pipe(tap((response: any) => response), catchError(this.handleError));
  }

  delete(url: string) {
    let options = {};
    this.setHeaders(options);

    this.http.delete(url, options);
  }

  private handleError(error: any) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('Client side network error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error('Backend - ' +
        `status: ${error.status}, ` +
        `statusText: ${error.statusText}, ` +
        `message: ${error.error.message}`);
    }

    // return an observable with a user-facing error message
    return throwError(error || 'server error');
  }

  private doPut(url: string, data: any, needId: boolean, params?: any): Observable<Response> {
    let options = {};
    this.setHeaders(options, needId);
    return this.http.put(url, data, options).pipe(tap((response: any) => response), catchError(this.handleError));
  }

  private setHeaders(options: any, needId?: boolean) {
    if (needId && this.securityService) {
      options["headers"] = new HttpHeaders()
        .append('authorization', 'Bearer ' + this.securityService.GetToken())
        .append('x-requestid', crypto.randomUUID());
    }
    else if (this.securityService) {
      options["headers"] = new HttpHeaders()
        .append('authorization', 'Bearer ' + this.securityService.GetToken());
    }
  }
}
