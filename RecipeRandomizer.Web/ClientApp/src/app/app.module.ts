import {BrowserModule} from '@angular/platform-browser';
import {APP_INITIALIZER, NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {AppRouting} from '@app/app.routing';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {NgSelectModule} from '@ng-select/ng-select';

// helpers
import {appInitializer} from '@app/helpers/app.initializer';
import {JwtInterceptor} from '@app/helpers/jwt.interceptor';
import {ErrorInterceptor} from '@app/helpers/error.interceptor';
import {AuthService} from '@app/services/auth.service';

// components
import {NavbarComponent} from '@app/components/navbar/navbar.component';
import {TopbarComponent} from '@app/components/topbar/topbar.component';
import {SpinnerComponent} from '@app/components/spinner/spinner.component';
import {AlertComponent} from '@app/components/alert/alert.component';
import {ToastComponent} from '@app/components/toast/toast.component';

// views
import {AppComponent} from '@app/app.component';
import {LoginComponent} from '@app/views/login/login.component';
import {LoginModalComponent} from '@app/views/login/login-modal.component';
import {RegisterComponent} from '@app/views/register/register.component';
import {RegisterModalComponent} from '@app/views/register/register-modal.component';
import {DashboardComponent} from '@app/views/dashboard/dashboard.component';
import {ForgotPasswordComponent} from '@app/views/forgot-password/forgot-password.component';
import {ForgotPasswordModalComponent} from '@app/views/forgot-password/forgot-password-modal.component';
import {NotFoundComponent} from '@app/views/not-found/not-found.component';
import {HomeComponent} from '@app/views/home/home.component';
import {RecipesComponent} from '@app/views/recipes/recipes.component';
import {RecipeComponent} from '@app/views/recipes/recipe/recipe.component';
import {RecipeModalComponent} from '@app/views/recipes/recipe/recipe-modal.component';

@NgModule({
    declarations: [
        AppComponent,
        NavbarComponent,
        TopbarComponent,
        LoginComponent,
        LoginModalComponent,
        RegisterComponent,
        RegisterModalComponent,
        DashboardComponent,
        SpinnerComponent,
        AlertComponent,
        ForgotPasswordComponent,
        ForgotPasswordModalComponent,
        NotFoundComponent,
        HomeComponent,
        ToastComponent,
        RecipesComponent,
        RecipeComponent,
        RecipeModalComponent
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
        ForgotPasswordComponent,
        RecipeComponent
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
