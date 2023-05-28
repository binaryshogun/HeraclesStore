import { Component } from '@angular/core';

import { SecurityService } from '../../services/security.service';
import { SignalrService } from '../../services/signalr.service';

@Component({
  selector: 'app-identity',
  templateUrl: './identity.component.html',
  styleUrls: ['./identity.component.scss']
})
export class IdentityComponent {
  authenticated: boolean = false;
  userName: string = '';

  constructor(private service: SecurityService, private signalrService: SignalrService) {
    this.service.authenticationChallenge$.subscribe(response => {
      if (this.service.UserData) {
        this.authenticated = response;
        this.userName = this.service.UserData.username;
      }
    });

    this.authenticated = this.service.IsAuthorized;

    if (this.authenticated) {
      if (this.service.UserData)
        this.userName = this.service.UserData.username;
    }
  }

  logoutClicked(event: any) {
    event.preventDefault();
    this.logout();
  }

  logout() {
    this.signalrService.stop();
    this.service.Logout();
  }
}
