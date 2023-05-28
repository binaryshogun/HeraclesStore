import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subject, throwError, Subscription, of, delay } from 'rxjs';
import { Router } from '@angular/router';
import { ConfigurationService } from './configuration.service';
import { StorageService } from './storage.service';
import { ILogin } from '../models/login.model';
import { IUserData } from '../models/userData.model';
import { JwtHelperService } from '@auth0/angular-jwt';
import { IRegister } from '../models/register.model';
import { ILoginResponse } from '../models/loginResponse.model';
import { HttpResponse } from '@microsoft/signalr';

@Injectable()
export class SecurityService {
  private headers: HttpHeaders;
  private authenticationSource = new Subject<boolean>();
  authenticationChallenge$ = this.authenticationSource.asObservable();

  private tokenExpired$ = new Subscription();
  private authorityUrl = '';
  public error$ = new Subject<string>;

  constructor(private http: HttpClient, private router: Router, private jwt: JwtHelperService, private configurationService: ConfigurationService, private storageService: StorageService) {
    this.IsAuthorized = false;

    this.headers = new HttpHeaders();
    this.headers.append('Content-Type', 'application/json');
    this.headers.append('Accept', 'application/json');

    this.configurationService.settingsLoaded$.subscribe({
      next: () => {
        this.authorityUrl = this.configurationService.serverSettings.identityUrl
        this.storageService.store('identityUrl', this.authorityUrl);
      }
    });

    if (this.GetToken() !== '') {
      this.IsAuthorized = true;
      this.UserData = this.storageService.retrieve('userData');
      this.authenticationSource.next(true);
    }
  }

  public IsAuthorized: boolean;

  public GetToken(): string {
    const token = this.storageService.retrieve('auth_token');

    if (this.jwt.isTokenExpired(token)) {
      this.Logout();
      return '';
    } else {
      return token;
    }
  }

  public UserData?: IUserData;

  public Login(credentials: ILogin) {
    const url = `${this.authorityUrl}/api/auth/signin`;

    this.http.post<ILoginResponse>(url, credentials).subscribe({
      next: (response) => {
        this.storageService.store('auth_token', response.token);

        const userId: string = this.jwt.decodeToken(this.GetToken()).sub;

        this.UserData = {
          username: response.username,
          email: response.email,
          userId: userId,
          password: credentials.password
        }

        this.IsAuthorized = true;
        this.storageService.store('userData', this.UserData);
        const expiredAt = this.jwt.getTokenExpirationDate(response.token);
        if (expiredAt) {
          const timeout = expiredAt.valueOf() - new Date().valueOf();
          this.tokenExpired$.unsubscribe();
          this.tokenExpired$ = of(null).pipe(delay(timeout)).subscribe(() => {
            this.Logout();
          })
        }

        this.authenticationSource.next(true);

        this.router.navigate(['catalog']);
      },
      error: (error) => {
        this.error$.next(error.error)
      }
    })
  }

  public Register(credentials: IRegister) {
    const url = `${this.authorityUrl}/api/auth/signup`;

    this.http.post<HttpResponse>(url, credentials).subscribe({
      next: () => {
        this.router.navigate(['login']);
      },
      error: (error) => {
        this.error$.next(error.error.errors.DomainValidations.toString().replace(';', ' '));
        return throwError(() => error.message);
      }
    })
  }

  public Logout() {
    this.authenticationSource.next(false);
    this.UserData = undefined;
    this.IsAuthorized = false;
    this.storageService.store('auth_token', '');
    this.storageService.store('userData', '');
  }

  public ClearError() {
    this.error$.next('');
  }
}