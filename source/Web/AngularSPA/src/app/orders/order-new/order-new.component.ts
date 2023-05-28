import { Component } from '@angular/core';
import { OrdersService } from '../orders.service';
import { UntypedFormGroup, UntypedFormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { BasketService } from 'src/app/basket/basket.service';
import { IOrder } from 'src/app/shared/models/order.model';

@Component({
  selector: 'app-order-new',
  templateUrl: './order-new.component.html',
  styleUrls: ['./order-new.component.scss']
})
export class OrderNewComponent {
  newOrderForm: UntypedFormGroup;  // new order form
  isOrderProcessing: boolean = false;
  errorReceived: boolean = false;
  order: IOrder;

  constructor(private orderService: OrdersService, private basketService: BasketService, fb: UntypedFormBuilder, private router: Router) {
    // Obtain user profile information
    this.order = orderService.mapOrderAndIdentityInfoNewOrder();
    this.newOrderForm = fb.group({
      'street': [this.order.street, Validators.required],
      'city': [this.order.city, Validators.required],
      'state': [this.order.state, Validators.required],
      'country': [this.order.country, Validators.required],
      'cardnumber': [this.order.cardNumber, Validators.required],
      'cardholdername': [this.order.cardHolderName, Validators.required],
      'expirationdate': [this.order.expiration, Validators.required],
      'securitycode': [this.order.cardSecurityNumber, Validators.required],
    });
  }

  submitForm() {
    this.order.street = this.newOrderForm.controls['street'].value;
    this.order.city = this.newOrderForm.controls['city'].value;
    this.order.state = this.newOrderForm.controls['state'].value;
    this.order.country = this.newOrderForm.controls['country'].value;
    this.order.cardNumber = this.newOrderForm.controls['cardnumber'].value;
    this.order.zipCode = '1';
    this.order.cardTypeId = 1;
    this.order.cardHolderName = this.newOrderForm.controls['cardholdername'].value;
    this.order.cardExpiration = new Date(20 + this.newOrderForm.controls['expirationdate'].value.split('/')[1], this.newOrderForm.controls['expirationdate'].value.split('/')[0]);
    this.order.cardSecurityNumber = this.newOrderForm.controls['securitycode'].value;
    let basketCheckout = this.basketService.mapBasketInfoCheckout(this.order);
    this.basketService.setBasketCheckout(basketCheckout)
      .pipe(catchError((error) => {
        this.errorReceived = true;
        this.isOrderProcessing = false;
        return throwError(() => error);
      }))
      .subscribe(res => {
        this.router.navigate(['orders']);
      });
    this.errorReceived = false;
    this.isOrderProcessing = true;
  }
}
