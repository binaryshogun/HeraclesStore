import { Component, OnChanges, Output, Input, EventEmitter } from '@angular/core';

import { IPager } from '../../models/pager.model';

@Component({
  selector: 'pager',
  templateUrl: './pager.component.html',
  styleUrls: ['./pager.component.scss']
})
export class PagerComponent<T> implements OnChanges {

  @Output()
  changed: EventEmitter<number> = new EventEmitter<number>();

  @Input()
  model: IPager = {
    totalItems: 0,
    items: 0,
    itemsPage: 0,
    totalPages: 0,
    actualPage: 0
  };

  buttonStates: any = {
    nextDisabled: true,
    previousDisabled: true,
  };

  ngOnChanges() {
    this.model.items = (this.model.itemsPage > this.model.totalItems) ? this.model.totalItems : this.model.itemsPage;

    this.buttonStates.previousDisabled = (this.model.actualPage == 0);
    this.buttonStates.nextDisabled = (this.model.actualPage + 1 >= this.model.totalPages);
  }

  onNextClicked(event: any) {
    console.log('Pager next clicked');
    this.changed.emit(this.model.actualPage + 1);
  }

  onPreviousCliked(event: any) {
    console.log('Pager previous clicked');
    this.changed.emit(this.model.actualPage - 1);
  }

}
