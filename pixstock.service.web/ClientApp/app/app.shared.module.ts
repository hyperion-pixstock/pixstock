import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { DashboardScreen } from './components/screen/dashboard/dashboard.screen';
import { CategoryListScreen } from './components/screen/category-list/category-list.screen';
import { PreviewScreen } from './components/screen/preview/preview.screen';

import 'hammerjs';
import { MatButtonModule, MatCheckboxModule } from '@angular/material';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        DashboardScreen,
        CategoryListScreen,
        PreviewScreen
    ],
    imports: [
        MatButtonModule, MatCheckboxModule,
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'dashboard', component: DashboardScreen },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}
