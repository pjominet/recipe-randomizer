import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AuthGuard} from '@app/helpers/auth.guard';

/* Routable views */
import {DashboardComponent} from '@app/views/dashboard/dashboard.component';
import {HomeComponent} from '@app/views/home/home.component';
import {NotFoundComponent} from '@app/views/not-found/not-found.component';

const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'home', component: HomeComponent},
    {path: '404', component: NotFoundComponent},
    {
        path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard], children: []
    },
    {path: '**', redirectTo: '/404', pathMatch: 'full'}
];

@NgModule({
    // imports: [RouterModule.forRoot(routes, {enableTracing: !environment.production})],
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRouting {
}
