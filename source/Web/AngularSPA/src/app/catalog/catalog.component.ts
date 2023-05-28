import { Component } from '@angular/core';
import { Subscription, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ICatalog } from '../shared/models/catalog.model';
import { ICatalogBrand } from '../shared/models/catalogBrand.model';
import { ICatalogItem } from '../shared/models/catalogItem.model';
import { ICatalogType } from '../shared/models/catalogType.model';
import { IPager } from '../shared/models/pager.model';
import { BasketWrapperService } from '../shared/services/basket.wrapper.service';
import { SecurityService } from '../shared/services/security.service';
import { CatalogService } from './catalog.service';
import { ConfigurationService } from '../shared/services/configuration.service';

@Component({
  selector: 'catalog .catalog .mb-5',
  styleUrls: ['./catalog.component.scss'],
  templateUrl: './catalog.component.html'
})
export class CatalogComponent {
  brands: ICatalogBrand[] = [];
  types: ICatalogType[] = [];
  catalog: ICatalog = {
    pageIndex: 0,
    pageSize: 0,
    data: [],
    displayData: [],
    count: 0,
  };
  brandSelected: number = -1;
  typeSelected: number = -1;
  paginationInfo: IPager = {
    totalItems: 0,
    items: 0,
    itemsPage: 0,
    totalPages: 0,
    actualPage: 0,
  };
  authenticated: Boolean = false;
  authSubscription: Subscription;
  errorReceived: Boolean = false;

  constructor(public catalogService: CatalogService, private configurationService: ConfigurationService, private basketService: BasketWrapperService, private securityService: SecurityService) {
    if (this.configurationService.isReady) {
      this.loadData();
    }
    else {
      this.configurationService.settingsLoaded$.subscribe({
        next: () => this.loadData()
      });
    }

    this.authenticated = this.securityService.IsAuthorized;

    this.authSubscription = this.securityService.authenticationChallenge$.subscribe(response => {
      this.authenticated = response;
    });
  }

  private loadData() {
    this.getBrands();
    this.getTypes();
    this.getCatalog(5, 0, this.brandSelected, this.typeSelected);
  }

  private getTypes() {
    this.catalogService.getTypes().subscribe(types => {
      this.types = types;
      let allTypes = { id: -1, type: 'All' };
      this.types.unshift(allTypes);
    });
  }

  private getBrands() {
    this.catalogService.getBrands().subscribe(brands => {
      this.brands = brands;
      let allBrands = { id: -1, brand: 'All' };
      this.brands.unshift(allBrands);
    });
  }

  onFilterApplied(event: Event) {
    event.preventDefault();

    this.paginationInfo.actualPage = 0;
    this.getCatalog(this.paginationInfo.itemsPage, this.paginationInfo.actualPage, this.brandSelected, this.typeSelected);
  }

  onBrandFilterChanged(event: Event) {
    event.preventDefault();

    const target = event.target as HTMLSelectElement;
    this.brandSelected = target.value !== null ? Number.parseInt(target.value) : -1;
  }

  onTypeFilterChanged(event: Event) {
    event.preventDefault();

    const target = event.target as HTMLSelectElement;
    this.typeSelected = target.value !== null ? Number.parseInt(target.value) : -1;
  }

  onPageChanged(event: any) {
    this.paginationInfo.actualPage = event;
    this.getCatalog(this.paginationInfo.itemsPage, event, this.brandSelected, this.typeSelected);
  }

  addToCart(item: ICatalogItem) {
    if (!this.authenticated || !this.configurationService.isReady) {
      return;
    }
    this.basketService.addItemToBasket(item);
  }

  getCatalog(pageSize: number, pageIndex: number, brand: number, type: number) {
    this.errorReceived = false;
    this.catalogService.getCatalog(brand, type)
      .pipe(catchError((err) => this.handleError(err)))
      .subscribe(items => {
        const start: number = pageIndex * pageSize;
        const end: number = (pageIndex + 1) * pageSize;

        this.catalog.data = items;
        this.catalog.count = items.length;
        this.catalog.pageIndex = pageIndex;
        this.catalog.pageSize = pageSize

        this.catalog.displayData = this.catalog.data.slice(start, end);

        this.paginationInfo = {
          actualPage: this.catalog.pageIndex,
          itemsPage: this.catalog.pageSize,
          totalItems: this.catalog.count,
          totalPages: Math.ceil(this.catalog.count / this.catalog.pageSize),
          items: this.catalog.pageSize,
        };
      });
  }

  private handleError(error: any) {
    this.errorReceived = true;
    return throwError(() => error);
  }
}

