﻿import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {NgSelectModule} from '@ng-select/ng-select';
import {MarkdownModule} from 'ngx-markdown';

import {DashboardRoutingModule} from './dashboard.routing';

// helpers
import {UnsavedChangesGuard} from '@app/helpers/unsaved-changes.guard';
import {EnumListPipe} from '@app/helpers/enumlist.pipe';

// components
import {NavbarComponent} from '@app/components/navbar/navbar.component';
import {TopbarComponent} from '@app/components/topbar/topbar.component';
import {FileUploadComponent} from '@app/components/file-upload/file-upload.component';
import {CheatSheetComponent} from '@app/components/cheat-sheet/cheat-sheet.component';

// views
import {DashboardLayoutComponent} from './dashboard-layout/dashboard-layout.component';
import {ProfileComponent} from './dashboard-layout/profile/profile.component';
import {UserRecipesComponent} from './dashboard-layout/user-recipes/user-recipes.component';
import {RecipeEditorComponent} from './dashboard-layout/recipe-editor/recipe-editor.component';
import {ChangePasswordComponent} from './change-password/change-password.component';
import {ChangePasswordModal} from './change-password/change-password-modal';
import {UserListComponent} from './admin/user-list/user-list.component';
import {OrphanRecipesComponent} from './admin/orphan-recipes/orphan-recipes.component';

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        DashboardRoutingModule,
        NgbModule,
        NgSelectModule,
        MarkdownModule.forChild(),
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
        EnumListPipe,
        FileUploadComponent,
        CheatSheetComponent,
        UserListComponent,
        OrphanRecipesComponent
    ],
    entryComponents: [
        ChangePasswordComponent,
        CheatSheetComponent
    ],
    providers: [
        UnsavedChangesGuard
    ]
})
export class DashboardModule {
}
