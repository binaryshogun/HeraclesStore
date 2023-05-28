import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

import { ICatalogItem } from '../models/catalogItem.model';
import { IBasketItem } from '../models/basketItem.model';
import { SecurityService } from './security.service';
import { IBasket } from '../models/basket.model';
import { StorageService } from './storage.service';

@Injectable()
export class BasketWrapperService {
  basket: IBasket = {
    buyerId: '',
    items: []
  }

  constructor(private identityService: SecurityService, private storage: StorageService) {
    this.basket = storage.retrieve('basket');
  }

  // observable that is fired when a product is added to the cart
  private addItemToBasketSource = new Subject<IBasketItem>();
  addItemToBasket$ = this.addItemToBasketSource.asObservable();

  private orderCreatedSource = new Subject<void>();
  orderCreated$ = this.orderCreatedSource.asObservable();

  addItemToBasket(item: ICatalogItem) {
    if (this.identityService.IsAuthorized) {
      let basketItem: IBasketItem = {
        pictureUrl: item.pictureUri,
        productId: item.id,
        productName: item.name,
        quantity: 1,
        unitPrice: item.price,
        id: crypto.randomUUID(),
        oldUnitPrice: item.price
      };

      this.addItemToBasketSource.next(basketItem);
    }
  }

  orderCreated() {
    this.orderCreatedSource.next();
  }
}