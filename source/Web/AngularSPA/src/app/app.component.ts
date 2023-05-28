import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { Subscription } from 'rxjs';

import { SecurityService } from './shared/services/security.service';
import { ConfigurationService } from './shared/services/configuration.service';

@Component({
  selector: 'app-root',
  styleUrls: ['./app.component.scss'],
  templateUrl: './app.component.html'
})
export class AppComponent {
  Authenticated: boolean = false;
  subscription: Subscription;

  constructor(private titleService: Title,
    public router: Router,
    private securityService: SecurityService,
    private configurationService: ConfigurationService,
  ) {
    this.configurationService.load();
    this.Authenticated = this.securityService.IsAuthorized;
    this.subscription = this.securityService.authenticationChallenge$.subscribe(res => this.Authenticated = res);
  }

  public setTitle(newTitle: string) {
    this.titleService.setTitle('Heracles Store');
  }
}
