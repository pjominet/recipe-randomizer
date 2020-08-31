import {NgModule} from '@angular/core';
import {ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';

import {DashboardRoutingModule} from './dashboard.routing';

// components
import {NavbarComponent} from '@app/components/navbar/navbar.component';
import {TopbarComponent} from '@app/components/topbar/topbar.component';

// views
import {DashboardLayoutComponent} from './dashboard-layout/dashboard-layout.component';
import {ProfileComponent} from './dashboard-layout/profile/profile.component';

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        DashboardRoutingModule
    ],
    declarations: [
        DashboardLayoutComponent,
        NavbarComponent,
        TopbarComponent,
        ProfileComponent
    ]
})
export class DashboardModule {
}
