import { Component } from '@angular/core';
import { SecurityService } from '../../shared/services/security.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.scss']
})
export class NavigationBarComponent {
  private securityService: SecurityService;
  private Subscription: Subscription;

  Authenticated: Boolean = false;

  constructor(securityService: SecurityService, public router: Router) {
    this.securityService = securityService;

    this.Subscription = this.securityService.authenticationChallenge$.subscribe(response => this.Authenticated = response);
    this.Authenticated = securityService.IsAuthorized
  }
}