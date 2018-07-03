import { NgModule, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Logger, Options as LoggerOptions, Level as LoggerLevel } from 'angular2-logger/core';
import { MatButtonModule, MatCheckboxModule, MatTreeModule, MatIconModule, MatProgressBarModule } from '@angular/material';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { DashboardScreen } from './components/screen/dashboard/dashboard.screen';
import { CategoryListScreen } from './components/screen/category-list/category-list.screen';
import { PreviewScreen } from './components/screen/preview/preview.screen';

import 'hammerjs';
import { ViewModel } from './viewmodel';
import { MessagingService } from './service/messaging.service';
import { CourierService } from './service/courier.service';
import { DeliveryService } from './service/delivery.service';
import { NaviService } from './service/navi.service';
import { CategoryTreeFragment } from './components/fragment/category-tree/category-tree.fragment';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        DashboardScreen,
        CategoryListScreen,
        PreviewScreen,
        CategoryTreeFragment
    ],
    imports: [
        BrowserAnimationsModule,
        MatButtonModule, MatCheckboxModule, MatTreeModule, MatIconModule, MatProgressBarModule, 
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'Dashboard', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'Dashboard', component: DashboardScreen },
            { path: 'CategoryList', component: CategoryListScreen },
            { path: 'Preview', component: PreviewScreen },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [
        { provide: LoggerOptions, useValue: { level: LoggerLevel.DEBUG } },
        Logger,
        ViewModel,
        MessagingService,
        CourierService,
        DeliveryService,
        NaviService
    ]
})
export class AppModuleShared {
    constructor(
        private logger: Logger,
        private ngZone: NgZone,
        private messaging: MessagingService,
        private courier: CourierService,
        private delivery: DeliveryService,
        private navi: NaviService
    ) {
        logger.info("アプリケーションの初期化 v0.0.1#6");
        console.info("アプリケーションの初期化 v0.0.1#6");

        let w: any = window;
        w['angularComponentRef'] = {
            component: this,
            zone: ngZone
        };

        var parent: any = window.parent; // JSのWindowオブジェクト
        console.info("parent", parent);

        if (parent.getFirstLoad == null) {
            logger.error("getFirstLoadの定義を見つけることができません");
        } else {
            let flag = parent.getFirstLoad();
            if (flag == false) {
                logger.info("[App] App初期読み込み判定");
                parent.setFirstLoad();
                messaging.initialize(parent.getIpc(), true, logger); // IPCオブジェクト取得
                courier.initialize();
                delivery.initialize();
                navi.initialize();
            } else {
                logger.info("[App] App初期化済み判定");
                messaging.initialize(parent.getIpc(), false, logger); // IPCオブジェクト取得
                courier.initialize();
                delivery.initialize();
                navi.initialize();
            }
        }
    }
}
