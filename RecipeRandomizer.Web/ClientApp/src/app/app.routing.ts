import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AuthGuard} from '@app/helpers/auth.guard';

/* Routable views */
import {HomeComponent} from '@app/views/home/home.component';
import {NotFoundComponent} from '@app/views/not-found/not-found.component';
import {RecipesComponent} from '@app/views/recipes/recipes.component';
import {RecipeModalComponent} from '@app/views/recipes/recipe/recipe-modal.component';

const routes: Routes = [
    {path: '', component: HomeComponent},
    {
        path: 'recipes', component: RecipesComponent, children: [
            {path: ':rid', component: RecipeModalComponent},
        ]
    },
    {path: 'auth', loadChildren: () => import('@app/views/auth/auth.module').then(x => x.AuthModule)},
    {path: 'dashboard', loadChildren: () => import('@app/views/dashboard/dashboard.module').then(x => x.DashboardModule), canActivate: [AuthGuard]},
    {path: '404', component: NotFoundComponent},
    {path: '**', redirectTo: '/404', pathMatch: 'full'},
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRouting {
}
