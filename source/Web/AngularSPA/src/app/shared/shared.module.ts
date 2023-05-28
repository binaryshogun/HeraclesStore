import { ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { PagerComponent } from './components/pager/pager.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { DataService } from './services/data.service';
import { BasketWrapperService } from './services/basket.wrapper.service';
import { SecurityService } from './services/security.service';
import { ConfigurationService } from './services/configuration.service';
import { StorageService } from './services/storage.service';
import { SignalrService } from './services/signalr.service';
import { UppercasePipe } from './pipes/uppercase.pipe';
import { GlobalErrorComponent } from './components/global-error/global-error.component';
import { ErrorService } from './services/error.service';
import { NotificationService } from './services/notification.service';
import { IdentityComponent } from './components/identity/identity.component';

@NgModule({
  declarations: [
    PagerComponent,
    NotFoundComponent,
    UppercasePipe,
    GlobalErrorComponent,
    IdentityComponent,
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    PagerComponent,
    NotFoundComponent,
    GlobalErrorComponent,
    IdentityComponent,
    UppercasePipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
  ]
})
export class SharedModule {
  static forRoot(): ModuleWithProviders<SharedModule> {
    return {
      ngModule: SharedModule,
      providers: [
        DataService,
        BasketWrapperService,
        SecurityService,
        ConfigurationService,
        StorageService,
        SignalrService,
        ErrorService,
        NotificationService
      ]
    }
  }
}
