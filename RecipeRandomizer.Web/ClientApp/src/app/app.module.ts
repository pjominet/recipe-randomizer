import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {AppRouting} from './app.routing';

//components
import {NavbarComponent} from './components/navbar/navbar.component';

// views
import {AppComponent} from './app.component';
import {HomeComponent} from './views/home/home.component';
import { TopbarComponent } from './components/topbar/topbar.component';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        NavbarComponent,
        TopbarComponent
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
