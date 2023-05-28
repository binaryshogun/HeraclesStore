import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IOrderDetail } from 'src/app/shared/models/order-detail.model';
import { OrdersService } from '../orders.service';
import { ConfigurationService } from 'src/app/shared/services/configuration.service';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss']
})
export class OrderDetailsComponent {
  public order: IOrderDetail = <IOrderDetail>{};

  constructor(private service: OrdersService, private route: ActivatedRoute, private configurationService: ConfigurationService) {
    if (this.configurationService.isReady) {
      this.route.params.subscribe(params => {
        const id = +params['id']; // (+) converts string 'id' to a number
        this.getOrder(id);
      });
    } else {
      this.route.params.subscribe(params => {
        const id = +params['id']; // (+) converts string 'id' to a number
        this.getOrder(id);
      });
    }
  }

  getOrder(id: number) {
    this.service.getOrder(id).subscribe(order => {
      this.order = order;
      console.log('order retrieved: ' + order.id);
      console.log(this.order);
    });
  }
}
