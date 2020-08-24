import {BrowserModule} from '@angular/platform-browser';
import {APP_INITIALIZER, NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {AppRouting} from './app.routing';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {NgSelectModule} from '@ng-select/ng-select';

// helpers
import {appInitializer} from './helpers/app.initializer';
import {JwtInterceptor} from './helpers/jwt.interceptor';
import {ErrorInterceptor} from './helpers/error.interceptor';
import {AuthService} from './services/auth.service';

// components
import {NavbarComponent} from './components/navbar/navbar.component';

// views
import {AppComponent} from './app.component';
import {TopbarComponent} from './components/topbar/topbar.component';
import {LoginComponent} from './views/login/login.component';
import {RegisterComponent} from './views/register/register.component';
import {DashboardComponent} from './views/dashboard/dashboard.component';
import {SpinnerComponent} from './components/spinner/spinner.component';
import {AlertComponent} from './components/alert/alert.component';
import {ForgotPasswordComponent} from './views/forgot-password/forgot-password.component';
import {NotFoundComponent} from './views/not-found/not-found.component';

@NgModule({
    declarations: [
        AppComponent,
        NavbarComponent,
        TopbarComponent,
        LoginComponent,
        RegisterComponent,
        DashboardComponent,
        SpinnerComponent,
        AlertComponent,
        ForgotPasswordComponent,
        NotFoundComponent,
    ],
    imports: [
        BrowserModule,
        HttpClientModule,
        FormsModule,
        AppRouting,
        NgbModule,
        NgSelectModule,
        ReactiveFormsModule
    ],
    entryComponents: [
        LoginComponent,
        RegisterComponent,
        ForgotPasswordComponent
    ],
    providers: [
        {provide: APP_INITIALIZER, useFactory: appInitializer, multi: true, deps: [AuthService]},
        {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
        {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
