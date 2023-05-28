import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { IConfiguration } from '../models/configuration.model';
import { StorageService } from './storage.service';
import { Subject, firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {
  serverSettings: IConfiguration = {
    identityUrl: '',
    catalogUrl: '',
    purchaseUrl: '',
    signalrUrl: '',
    basketUrl: '',
  };

  private settingsLoadedSource = new Subject<void>();
  settingsLoaded$ = this.settingsLoadedSource.asObservable();
  isReady: boolean = false;

  constructor(private http: HttpClient, private storageService: StorageService) {
    this.serverSettings = {
      identityUrl: this.storageService.retrieve('identityUrl'),
      catalogUrl: this.storageService.retrieve('catalogUrl'),
      purchaseUrl: this.storageService.retrieve('purchaseUrl'),
      signalrUrl: this.storageService.retrieve('signalrUrl'),
      basketUrl: this.storageService.retrieve('basketUrl'),
    }
    this.settingsLoadedSource.next();
  }

  load() {
    this.http.get<IConfiguration>('../../../assets/app.config.json').subscribe((response) => {
      this.serverSettings = response;
      this.storageService.store('catalogUrl', this.serverSettings.catalogUrl)
      this.storageService.store('identityUrl', this.serverSettings.identityUrl);
      this.storageService.store('purchaseUrl', this.serverSettings.purchaseUrl);
      this.storageService.store('signalrUrl', this.serverSettings.signalrUrl);
      this.storageService.store('basketUrl', this.serverSettings.basketUrl);
      this.isReady = true;
      this.settingsLoadedSource.next();
    });
  }
}
