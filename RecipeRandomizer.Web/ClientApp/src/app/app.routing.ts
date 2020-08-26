import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AuthGuard} from '@app/helpers/auth.guard';

/* Routable views */
import {DashboardComponent} from '@app/views/dashboard/dashboard.component';
import {HomeComponent} from '@app/views/home/home.component';
import {NotFoundComponent} from '@app/views/not-found/not-found.component';
import {RecipesComponent} from '@app/views/recipes/recipes.component';
import {RecipeModalComponent} from '@app/views/recipes/recipe/recipe-modal.component';
import {LoginModalComponent} from '@app/views/login/login-modal.component';
import {RegisterModalComponent} from '@app/views/register/register-modal.component';
import {ForgotPasswordModalComponent} from '@app/views/forgot-password/forgot-password-modal.component';

const routes: Routes = [
    {
        path: '', component: HomeComponent, children: [
            {path: 'sign-in', component: LoginModalComponent},
            {path: 'sign-up', component: RegisterModalComponent},
            {path: 'forgot-password', component: ForgotPasswordModalComponent}
        ]
    },
    {path: 'home', redirectTo: '', pathMatch: 'full'},
    {path: '404', component: NotFoundComponent},
    {
        path: 'recipes', component: RecipesComponent, children: [
            {path: ':rid', component: RecipeModalComponent},
        ]
    },
    {
        path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard], children: []
    },
    {path: '**', redirectTo: '/404', pathMatch: 'full'},
];

@NgModule({
    // imports: [RouterModule.forRoot(routes, {enableTracing: !environment.production})],
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRouting {
}
