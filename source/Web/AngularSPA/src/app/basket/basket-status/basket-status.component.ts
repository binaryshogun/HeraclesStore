import { Component } from '@angular/core';

import { BasketService } from '../basket.service';
import { BasketWrapperService } from '../../shared/services/basket.wrapper.service';
import { SecurityService } from '../../shared/services/security.service';
import { ConfigurationService } from '../../shared/services/configuration.service';

@Component({
  selector: 'app-basket-status',
  styleUrls: ['./basket-status.component.scss'],
  templateUrl: './basket-status.component.html'
})
export class BasketStatusComponent {
  badge: number = 0;

  constructor(private basketService: BasketService, private basketWrapperService: BasketWrapperService, private authService: SecurityService, private configurationService: ConfigurationService) {
    this.basketWrapperService.addItemToBasket$.subscribe(item => {
      this.basketService.addItemToBasket(item).subscribe(() => {
        this.basketService.getBasket().subscribe({
          next: basket => {
            this.badge = basket.items.length;
          }
        });
      });
    });

    this.basketWrapperService.orderCreated$.subscribe(() => {
      this.badge = 0;
    })

    this.basketService.basketUpdate$.subscribe(() => {
      this.basketService.getBasket().subscribe(basket => {
        this.badge = basket.items.length;
      });
    });

    this.authService.authenticationChallenge$.subscribe((response) => {
      if (response) {
        this.basketService.getBasket().subscribe(basket => {
          this.badge = basket.items.length;
        });
      }
    });

    if (this.configurationService.isReady) {
      this.basketService.getBasket().subscribe(basket => {
        this.badge = basket.items.length;
      });
    } else {
      this.configurationService.settingsLoaded$.subscribe(() => {
        this.basketService.getBasket().subscribe(basket => {
          this.badge = basket.items.length;
        });
      });
    }
  }
}

