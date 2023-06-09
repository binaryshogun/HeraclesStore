import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { DataService } from '../shared/services/data.service';
import { SecurityService } from '../shared/services/security.service';
import { IBasket } from '../shared/models/basket.model';
import { IOrder } from '../shared/models/order.model';
import { IBasketCheckout } from '../shared/models/basketCheckout.model';
import { BasketWrapperService } from '../shared/services/basket.wrapper.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { StorageService } from '../shared/services/storage.service';

import { Observable, ReplaySubject, Subject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { IBasketItem } from '../shared/models/basketItem.model';

@Injectable()
export class BasketService {
  private basketUrl: string = '';
  private purchaseUrl: string = '';
  basket: IBasket = {
    buyerId: '',
    items: []
  };

  //observable that is fired when item is removed from basket
  basketUpdateSource = new ReplaySubject(1);
  basketUpdate$ = this.basketUpdateSource.asObservable();

  constructor(private service: DataService, private authService: SecurityService, private basketWrapperService: BasketWrapperService, private router: Router, private configurationService: ConfigurationService, private storageService: StorageService) {
    this.basket.items = [];

    if (this.authService.IsAuthorized) {
      if (this.authService.UserData) {
        this.basket.buyerId = this.authService.UserData.userId;
        if (this.configurationService.isReady) {
          this.basketUrl = this.configurationService.serverSettings.basketUrl;
          this.purchaseUrl = this.configurationService.serverSettings.purchaseUrl;
          this.loadData();
        }
        else {
          this.configurationService.settingsLoaded$.subscribe(x => {
            this.basketUrl = this.configurationService.serverSettings.basketUrl;
            this.purchaseUrl = this.configurationService.serverSettings.purchaseUrl;
            this.loadData();
          });
        }
      }
    }

    this.authService.authenticationChallenge$.subscribe((response) => {
      if (!response) {
        this.basket.buyerId = '';
        this.basket.items = [];
        this.storageService.store('basket', '');
      }
      else {
        this.loadData();
      }
    })

    this.basketWrapperService.orderCreated$.subscribe(() => {
      this.dropBasket();
    });
  }

  addItemToBasket(item: IBasketItem): Observable<boolean> {
    let basketItem = this.basket.items.find(value => value.productId == item.productId);

    if (basketItem) {
      basketItem.quantity++;
    } else {
      this.basket.items.push(item);
    }

    return this.setBasket(this.basket);
  }

  setBasket(basket: IBasket): Observable<boolean> {
    console.log(basket);
    let url = this.purchaseUrl + '/gw/basket/update';

    this.basket = basket;

    return this.service.put(url, basket).pipe<boolean>(tap((response: any) => {
      return true;
    }));
  }

  setBasketCheckout(basketCheckout: IBasketCheckout): Observable<boolean> {
    let url = this.basketUrl + '/api/basket';

    return this.service.postWithId(url, basketCheckout).pipe<boolean>(tap((response: any) => {
      this.basketWrapperService.orderCreated();

      return true;
    }));
  }

  getBasket(): Observable<IBasket> {
    let url = this.purchaseUrl + '/gw/basket/' + this.authService.UserData?.userId;
    return this.service.get(url).pipe<IBasket>(tap((response: any) => {
      if (response.status === 204) {
        return null;
      }

      return response;
    }));
  }

  mapBasketInfoCheckout(order: IOrder): IBasketCheckout {
    let basketCheckout = <IBasketCheckout>{};

    basketCheckout.street = order.street
    basketCheckout.city = order.city;
    basketCheckout.country = order.country;
    basketCheckout.state = order.state;
    basketCheckout.zipCode = order.zipCode;
    basketCheckout.cardExpiration = order.cardExpiration;
    basketCheckout.cardNumber = order.cardNumber;
    basketCheckout.cardSecurityNumber = order.cardSecurityNumber;
    basketCheckout.cardTypeId = order.cardTypeId;
    basketCheckout.cardHolder = order.cardHolderName;

    return basketCheckout;
  }

  updateQuantity() {
    this.loadData();
  }

  dropBasket() {
    console.log('basket dropped');
    this.basket.items = [];
    this.basketWrapperService.basket.items = [];
  }

  private loadData() {
    this.getBasket().subscribe({
      next: basket => {
        this.basket = basket;
        this.basket.buyerId = `${this.authService.UserData?.userId}`;
        this.storageService.store('basket', this.basket);
        this.basketUpdateSource.next(basket);
      },
      error: err => this.basketUpdateSource.error(err)
    });
  }
}