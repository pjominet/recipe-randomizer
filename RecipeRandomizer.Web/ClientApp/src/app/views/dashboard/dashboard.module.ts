﻿import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {NgSelectModule} from '@ng-select/ng-select';

import {DashboardRoutingModule} from './dashboard.routing';

// helpers
import {UnsavedChangesGuard} from '@app/helpers/unsaved-changes.guard';
import {EnumListPipe} from '@app/helpers/enumlist.pipe';

// components
import {NavbarComponent} from '@app/components/navbar/navbar.component';
import {TopbarComponent} from '@app/components/topbar/topbar.component';

// views
import {DashboardLayoutComponent} from './dashboard-layout/dashboard-layout.component';
import {ProfileComponent} from './dashboard-layout/profile/profile.component';
import {UserRecipesComponent} from './dashboard-layout/user-recipes/user-recipes.component';
import {RecipeEditorComponent} from './dashboard-layout/recipe-editor/recipe-editor.component';
import {ChangePasswordComponent} from './change-password/change-password.component';
import {ChangePasswordModal} from './change-password/change-password-modal';

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        DashboardRoutingModule,
        NgbModule,
        NgSelectModule,
    ],
    declarations: [
        DashboardLayoutComponent,
        NavbarComponent,
        TopbarComponent,
        ProfileComponent,
        UserRecipesComponent,
        RecipeEditorComponent,
        ChangePasswordComponent,
        ChangePasswordModal,
        EnumListPipe
    ],
    entryComponents: [
        ChangePasswordComponent
    ],
    providers: [
        UnsavedChangesGuard
    ]
})
export class DashboardModule {
}
