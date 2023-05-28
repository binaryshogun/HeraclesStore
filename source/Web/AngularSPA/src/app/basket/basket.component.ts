import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { IBasket } from '../shared/models/basket.model';
import { IBasketItem } from '../shared/models/basketItem.model';
import { BasketWrapperService } from '../shared/services/basket.wrapper.service';
import { BasketService } from './basket.service';
import { SecurityService } from '../shared/services/security.service';
import { ConfigurationService } from '../shared/services/configuration.service';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.scss']
})
export class BasketComponent {
  errorMessages: any;
  basket: IBasket = {
    buyerId: '',
    items: []
  };
  totalPrice: number = 0;

  constructor(private basketService: BasketService, private router: Router, private configurationService: ConfigurationService, private basketWrapperService: BasketWrapperService, private securityService: SecurityService) {
    if (!this.securityService.IsAuthorized) {
      this.router.navigate(['login']);
    }

    this.securityService.authenticationChallenge$.subscribe(response => {
      if (!response) {
        this.router.navigate(['login']);
      }
    })

    if (this.configurationService.isReady) {
      this.basketService.getBasket().subscribe(basket => {
        this.basket = basket;
        this.calculateTotalPrice();
      });
    } else {
      this.configurationService.settingsLoaded$.subscribe(() => {
        this.basketService.getBasket().subscribe(basket => {
          this.basket = basket;
          this.calculateTotalPrice();
        });
      })
    }

    this.basketService.basketUpdate$.subscribe(() => {
      this.basket = this.basketService.basket;
    })
  }

  deleteItem(id: String) {
    this.basket.items = this.basket.items.filter(item => item.id !== id);
    this.calculateTotalPrice();

    this.basketService.setBasket(this.basket).subscribe(x => {
      this.basketService.updateQuantity();
    }
    );
  }

  itemQuantityChanged(item: IBasketItem, quantity: number) {
    item.quantity = quantity > 0 ? quantity : 1;
    this.calculateTotalPrice();
    this.basketService.setBasket(this.basket);
  }

  update(event: any): Observable<boolean> {
    let setBasketObservable = this.basketService.setBasket(this.basket);
    setBasketObservable.subscribe({
      next: x => {
        this.errorMessages = [];
      },
      error: errMessage => this.errorMessages = errMessage.messages
    });

    return setBasketObservable;
  }

  checkOut(event: any) {
    this.update(event).subscribe(() => {
      this.errorMessages = [];
      this.basketWrapperService.basket = this.basket;
      this.router.navigate(['order']);
    });
  }

  private calculateTotalPrice() {
    this.totalPrice = 0;
    this.basket.items.forEach(item => {
      this.totalPrice += (item.unitPrice * item.quantity);
    });
  }
}
