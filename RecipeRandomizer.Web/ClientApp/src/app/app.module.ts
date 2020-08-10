import {BrowserModule} from '@angular/platform-browser';
import {LOCALE_ID, NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {AppRouting} from './app.routing';

// localization
import {registerLocaleData} from '@angular/common';
import localEn from '@angular/common/locales/en-DE'

// views
import {AppComponent} from './app.component';
import {HomeComponent} from './home/home.component';

registerLocaleData(localEn);

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent
    ],
    imports: [
        BrowserModule,
        HttpClientModule,
        FormsModule,
        AppRouting
    ],
    providers: [
        {provide: LOCALE_ID, useValue: 'en-DE'},
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
