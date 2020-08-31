import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {DashboardLayoutComponent} from './dashboard-layout/dashboard-layout.component';
import {ProfileComponent} from './dashboard-layout/profile/profile.component';

const routes: Routes = [
    {
        path: '', component: DashboardLayoutComponent, children: [
            {path: '', component: ProfileComponent}
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DashboardRoutingModule { }
