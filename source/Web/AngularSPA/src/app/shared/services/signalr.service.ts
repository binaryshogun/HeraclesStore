import { Injectable } from '@angular/core';
import { SecurityService } from './security.service';
import { ConfigurationService } from './configuration.service';
import { HubConnection, HubConnectionBuilder, LogLevel, HttpTransportType } from '@microsoft/signalr';
import { Subject, Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class SignalrService {
  private hubConnection: HubConnection;
  private signalrUrl: string = '';
  private notificationsHub: string = '/hub/notifications';
  private msgSignalrSource = new Subject<void>();
  msgReceived$ = this.msgSignalrSource.asObservable();
  authSubscription: Subscription;
  authenticated: boolean = false;

  constructor(
    private securityService: SecurityService,
    private configurationService: ConfigurationService,
    private toastr: ToastrService,
  ) {
    this.authSubscription = this.securityService.authenticationChallenge$.subscribe(response => {
      this.authenticated = response;
      if (this.authenticated && this.configurationService.isReady) {
        this.hubConnection.baseUrl = this.signalrUrl + this.notificationsHub;
        this.hubConnection.start()
          .then(() => {
            console.log('Hub connection started')
          })
          .catch(() => {
            console.log('Error while establishing connection')
          });
      }
    });
    if (this.configurationService.isReady) {
      this.signalrUrl = this.configurationService.serverSettings.signalrUrl;
    }
    else {
      this.configurationService.settingsLoaded$.subscribe({
        next: () => {
          this.signalrUrl = this.configurationService.serverSettings.signalrUrl
        }
      });
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.signalrUrl + this.notificationsHub, {
        accessTokenFactory: () => this.securityService.GetToken()
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build()


    this.hubConnection.on('UpdatedOrderState', (msg) => {
      console.log(`Order ${msg.orderId} updated to ${msg.status}`);
      this.toastr.success('Updated to status: ' + msg.status, 'Order Id: ' + msg.orderId);
      this.msgSignalrSource.next();
    });
  }

  public stop() {
    this.hubConnection.stop();
  }
}