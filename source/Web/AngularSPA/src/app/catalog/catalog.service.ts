import { Injectable } from '@angular/core';

import { DataService } from '../shared/services/data.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { ICatalogBrand } from '../shared/models/catalogBrand.model';
import { ICatalogType } from '../shared/models/catalogType.model';

import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ICatalogItem } from '../shared/models/catalogItem.model';


@Injectable()
export class CatalogService {
  public baseUrl: string = '';
  public catalogUrl: string = '';
  private brandUrl: string = '';
  private typesUrl: string = '';

  constructor(private service: DataService, private configurationService: ConfigurationService) {
    if (configurationService.isReady) {
      this.initializeUrls();
    }

    this.configurationService.settingsLoaded$.subscribe({
      next: () => this.initializeUrls(),
    });
  }

  initializeUrls() {
    this.baseUrl = this.configurationService.serverSettings.catalogUrl;
    this.catalogUrl = this.baseUrl + '/api/catalog/items';
    this.brandUrl = this.baseUrl + '/api/catalog/brands';
    this.typesUrl = this.baseUrl + '/api/catalog/types';
  }

  getCatalog(brand: number, type: number): Observable<ICatalogItem[]> {
    let url = this.catalogUrl;

    if (type !== -1) {
      url = this.catalogUrl + '/type/' + type.toString() + ((brand !== -1) ? ('/brand/' + brand.toString()) : '');
    }
    else if (brand !== -1) {
      url = this.catalogUrl + ((brand !== -1) ? ('/type/all/brand/' + brand.toString()) : '');
    }

    return this.service.get(url).pipe<ICatalogItem[]>((response: any) => response);
  }

  getBrands(): Observable<ICatalogBrand[]> {
    return this.service.get(this.brandUrl).pipe<ICatalogBrand[]>(tap((response: any) => {
      return response;
    }));
  }

  getTypes(): Observable<ICatalogType[]> {
    return this.service.get(this.typesUrl).pipe<ICatalogType[]>(tap((response: any) => {
      return response;
    }));
  };
}
