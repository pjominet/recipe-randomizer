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
import {SpinnerComponent} from '@app/components/spinner/spinner.component';
import {AlertComponent} from '@app/components/alert/alert.component';
import {ToastComponent} from '@app/components/toast/toast.component';

// views
import {AppComponent} from '@app/app.component';
import {NotFoundComponent} from '@app/views/not-found/not-found.component';
import {HomeComponent} from '@app/views/home/home.component';
import {RecipesComponent} from '@app/views/recipes/recipes.component';
import {RecipeComponent} from '@app/views/recipes/recipe/recipe.component';
import {RecipeModal} from '@app/views/recipes/recipe/recipe-modal';
import {RandomRecipeComponent} from './views/recipes/random-recipe/random-recipe.component';

@NgModule({
    declarations: [
        AppComponent,
        SpinnerComponent,
        AlertComponent,
        NotFoundComponent,
        HomeComponent,
        ToastComponent,
        RecipesComponent,
        RecipeComponent,
        RecipeModal,
        RandomRecipeComponent
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
