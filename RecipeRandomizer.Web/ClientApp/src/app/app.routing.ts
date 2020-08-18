import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';

/* Routable views */
import {HomeComponent} from './views/home/home.component';

const routes: Routes = [
    {path: 'home', component: HomeComponent},
    {path: '', redirectTo: '/home', pathMatch: 'full'},
    {path: '**', redirectTo: '/404', pathMatch: 'full'}
];

@NgModule({
    // imports: [RouterModule.forRoot(routes, {enableTracing: !environment.production})],
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRouting {
}
