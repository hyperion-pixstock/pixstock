import { NgModule, NgZone } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule, MatCheckboxModule, MatIconModule, MatProgressBarModule, MatProgressSpinnerModule, MatSidenavModule, MatTreeModule, MatInputModule } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { NgxMapboxGLModule } from 'ngx-mapbox-gl';
import { AppComponent } from './app.component';
import { DisplayMapComponent } from './components/displayMap/displayMap.component';
import { CategoryTreeFragment } from './components/fragment/category-tree/category-tree.fragment';
import { ContentPreviewFragment } from './components/fragment/content-preview/content-preview.fragment';
import { ExplorerListFragment } from './components/fragment/explorer-list/explorer-list.fragment';
import { DeliveryService } from './service/delivery.service';
import { MessagingService } from './service/messaging.service';
import { ViewModel } from './viewmodel';
import { CourierService } from './service/courier.service';
import { NaviService } from './service/navi.service';
import { DashboardScreen } from './components/screen/dashboard/dashboard.screen';
import { FinderScreen } from './components/screen/finder/finder.screen';
import { PreviewScreen } from './components/screen/preview/preview.screen';

@NgModule({
  declarations: [
    AppComponent,
    DisplayMapComponent,
    // Fragment
    CategoryTreeFragment,
    ExplorerListFragment,
    ContentPreviewFragment,
    // Screen
    DashboardScreen,
    FinderScreen,
    PreviewScreen,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    MatButtonModule, MatCheckboxModule, MatInputModule,
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
    CourierService,
    DeliveryService,
    NaviService,
    ViewModel
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(
    private ngZone: NgZone,
    private messaging: MessagingService,
    private delivery: DeliveryService,
    private navi: NaviService
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
      } else {
        console.info("[App] App初期化済み判定");
        console.info("IPCオブジェクト取得", parent.getIpc());
        messaging.initialize(parent.getIpc(), false); // IPCオブジェクト取得
      }
    }
  }
}
