import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AuthGuard} from '@app/helpers/auth.guard';

/* Routable views */
import {DashboardComponent} from '@app/views/dashboard/dashboard.component';

const routes: Routes = [
    {path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard]},
    {path: '', redirectTo: '/', pathMatch: 'full'},
    {path: '**', redirectTo: '/404', pathMatch: 'full'}
];

@NgModule({
    // imports: [RouterModule.forRoot(routes, {enableTracing: !environment.production})],
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRouting {
}
