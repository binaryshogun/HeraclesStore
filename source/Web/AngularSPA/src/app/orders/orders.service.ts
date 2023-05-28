import { Injectable } from '@angular/core';

import { DataService } from '../shared/services/data.service';
import { IOrder } from '../shared/models/order.model';
import { IOrderItem } from '../shared/models/orderItem.model';
import { IOrderDetail } from "../shared/models/order-detail.model";
import { SecurityService } from '../shared/services/security.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { BasketWrapperService } from '../shared/services/basket.wrapper.service';

import { Observable, ReplaySubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { StorageService } from './../shared/services/storage.service';
import { Subject } from 'rxjs';

@Injectable()
export class OrdersService {
  private ordersUrl: string = '';
  ordersUpdated$: Subject<IOrderDetail[]> = new ReplaySubject(1);
  orders: Observable<IOrderDetail[]> = new Observable<IOrderDetail[]>();

  constructor(
    private service: DataService,
    private basketService: BasketWrapperService,
    private identityService: SecurityService,
    private configurationService: ConfigurationService,
    private storageService: StorageService,
    private router: Router) {
    if (this.configurationService.isReady) {
      this.ordersUrl = this.storageService.retrieve('signalrUrl');
      this.getOrders(true);
    } else {
      this.configurationService.settingsLoaded$.subscribe(() => {
        this.ordersUrl = this.configurationService.serverSettings.signalrUrl;
        this.getOrders(true);
      });
    }

    if (!this.identityService.IsAuthorized) {
      router.navigate(['login'])
    }

    this.identityService.authenticationChallenge$.subscribe(response => {
      if (!response) {
        this.router.navigate(['login']);
      }
    })

    this.basketService.orderCreated$.subscribe(() => {
      this.getOrders(true);
    })
  }

  getOrders(refresh: boolean = false): Observable<IOrderDetail[]> {
    if (refresh || !this.orders) {
      const url = this.ordersUrl + '/api/orders';

      this.orders = this.service.get(url).pipe<IOrderDetail[]>((response: any) => response);
      this.orders.subscribe({
        next: result => this.ordersUpdated$.next(result),
        error: error => this.ordersUpdated$.error(error)
      })
    }

    return this.ordersUpdated$.asObservable();
  }

  cancelOrder(orderNumber: number): Observable<any> {
    let url = this.ordersUrl + '/api/orders/cancel';
    let data = { OrderId: orderNumber };

    const update = this.service.putWithId(url, data).pipe<any>((response: any) => response);
    update.subscribe((response: any) =>
      this.getOrders(true)
    );

    return update;
  }

  getOrder(id: number): Observable<IOrderDetail> {
    if (this.ordersUrl === '') {
      this.ordersUrl = this.storageService.retrieve('signalrUrl');
    }
    let url = this.ordersUrl + '/api/orders/' + id;

    return this.service.get(url).pipe<IOrderDetail>(tap((response: any) => {
      return response;
    }));
  }

  mapOrderAndIdentityInfoNewOrder(): IOrder {
    let order = <IOrder>{};
    let basket = this.basketService.basket;

    order.total = 0;
    order.orderItems = new Array<IOrderItem>();
    basket.items.forEach(x => {
      let item: IOrderItem = <IOrderItem>{};
      item.pictureUrl = x.pictureUrl;
      item.productId = +x.productId;
      item.productName = x.productName;
      item.unitPrice = x.unitPrice;
      item.units = x.quantity;

      order.total += (item.unitPrice * item.units);

      order.orderItems.push(item);
    });

    order.buyer = basket.buyerId;

    return order;
  }

}

