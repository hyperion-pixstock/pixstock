import { BrowserModule } from '@angular/platform-browser';
import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
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
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
