import { Component } from '@angular/core';
import { catchError, throwError, delay, Observable } from 'rxjs';
import { IOrder } from '../shared/models/order.model';
import { ConfigurationService } from '../shared/services/configuration.service';
import { SignalrService } from '../shared/services/signalr.service';
import { OrdersService } from './orders.service';
import { IOrderDetail } from '../shared/models/order-detail.model';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent {
  private oldOrders: IOrderDetail[] = [];
  private interval = null;
  errorReceived: boolean = false;

  constructor(public service: OrdersService, private configurationService: ConfigurationService, private signalrService: SignalrService) { }

  cancelOrder(orderNumber: number) {
    this.errorReceived = false;
    this.service.cancelOrder(orderNumber).pipe(catchError((err) => this.handleError(err)));
  }

  private handleError(error: any) {
    this.errorReceived = true;
    return throwError(() => error);
  }
}
