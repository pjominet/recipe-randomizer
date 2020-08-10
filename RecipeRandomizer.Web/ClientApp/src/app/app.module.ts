import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {AppRouting} from './app.routing';


// views
import {AppComponent} from './app.component';
import {HomeComponent} from './home/home.component';

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
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule {
}
