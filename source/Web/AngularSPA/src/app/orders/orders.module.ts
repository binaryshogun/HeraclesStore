import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BasketService } from '../basket/basket.service';
import { SharedModule } from '../shared/shared.module';
import { OrdersComponent } from './orders.component';
import { OrdersService } from './orders.service';
import { OrderDetailsComponent } from './order-details/order-details.component';
import { OrderNewComponent } from './order-new/order-new.component';

@NgModule({
  imports: [BrowserModule, SharedModule],
  declarations: [OrdersComponent, OrderDetailsComponent, OrderNewComponent],
  providers: [OrdersService, BasketService]
})
export class OrdersModule { }
