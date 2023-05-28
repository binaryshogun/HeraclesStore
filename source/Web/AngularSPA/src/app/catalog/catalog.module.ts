import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { SharedModule } from '../shared/shared.module';
import { CatalogComponent } from './catalog.component';
import { CatalogService } from './catalog.service';

@NgModule({
  imports: [BrowserModule, SharedModule, CommonModule],
  declarations: [CatalogComponent],
  providers: [CatalogService]
})
export class CatalogModule { }
