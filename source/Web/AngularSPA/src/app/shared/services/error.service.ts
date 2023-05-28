import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class ErrorService {
  error$ = new Subject<string>();

  constructor() { }

  handle(message: string) {
    console.log(message);
    this.error$.next(message);
  }

  clear() {
    this.error$.next('');
  }
}