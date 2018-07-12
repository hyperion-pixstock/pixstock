import { BrowserModule } from '@angular/platform-browser';
import { NgModule, NO_ERRORS_SCHEMA, NgZone } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { NgxMapboxGLModule } from 'ngx-mapbox-gl';

import { AppComponent } from './app.component';
import { DisplayMapComponent } from './components/displayMap/displayMap.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
  MatButtonModule, MatCheckboxModule, MatTreeModule, MatIconModule,
  MatProgressBarModule, MatProgressSpinnerModule,
  MatSidenavModule
} from '@angular/material';

import { MessagingService } from './service/messaging.service';
import { DeliveryService } from './service/delivery.service';
import { ViewModel } from './viewmodel';

@NgModule({
  declarations: [
    AppComponent,
    DisplayMapComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    MatButtonModule, MatCheckboxModule,
    MatTreeModule, MatIconModule,
    MatProgressBarModule, MatProgressSpinnerModule,
    MatSidenavModule,
    ReactiveFormsModule,
    MDBBootstrapModule.forRoot(),
    NgxMapboxGLModule.forRoot({
      accessToken: 'YOUR ACCESS TOKEN GOES HERE', // Can also be set per map (accessToken input of mgl-map)
      geocoderAccessToken: 'TOKEN' // Optionnal, specify if different from the map access token, can also be set per mgl-geocoder (accessToken input of mgl-geocoder)
    })
  ],
  providers: [
    MessagingService,
    DeliveryService,
    ViewModel
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(
    private ngZone: NgZone,
    private messaging: MessagingService,
    private delivery: DeliveryService
  ) {
    let w: any = window;
    w['angularComponentRef'] = {
      component: this,
      zone: ngZone
    };

    var parent: any = window.parent; // JSのWindowオブジェクト
    console.info("parent", parent);

    if (parent.getFirstLoad == null) {
      console.error("getFirstLoadの定義を見つけることができません");
    } else {
      let flag = parent.getFirstLoad();
      if (flag == false) {
        console.info("[App] App初期読み込み判定");
        parent.setFirstLoad();
        console.info("IPCオブジェクト取得", parent.getIpc());
        messaging.initialize(parent.getIpc(), true); // IPCオブジェクト取得
        //courier.initialize();
        delivery.initialize();
        //navi.initialize();
      } else {
        console.info("[App] App初期化済み判定");
        console.info("IPCオブジェクト取得", parent.getIpc());
        messaging.initialize(parent.getIpc(), false); // IPCオブジェクト取得
        //courier.initialize();
        delivery.initialize();
        //navi.initialize();
      }
    }
  }
}
