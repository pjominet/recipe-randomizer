import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';

// helpers
import {Role} from '@app/models/identity/user';
import {AuthGuard} from '@app/helpers/auth.guard';

// routable views
import {DashboardLayoutComponent} from './dashboard-layout/dashboard-layout.component';
import {ProfileComponent} from './dashboard-layout/profile/profile.component';
import {UserRecipesComponent} from './dashboard-layout/user-recipes/user-recipes.component';
import {RecipeEditorComponent} from './dashboard-layout/recipe-editor/recipe-editor.component';
import {RandomRecipeComponent} from '@app/views/recipes/random-recipe/random-recipe.component';
import {RecipeModal} from '@app/views/recipes/recipe/recipe-modal';
import {ChangePasswordModal} from './change-password/change-password-modal';
import {RecipeListComponent} from '@app/views/recipes/recipe-list/recipe-list.component';
import {UserListComponent} from './admin/user-list/user-list.component';
import {OrphanRecipesComponent} from './admin/orphan-recipes/orphan-recipes.component';

const routes: Routes = [
    {
        path: '', component: DashboardLayoutComponent, children: [
            {
                path: '', component: ProfileComponent, children: [
                    {path: 'change-password', component: ChangePasswordModal}
                ]
            },
            {path: 'profile', redirectTo: '', pathMatch: 'full'},
            {
                path: 'my-recipes', component: UserRecipesComponent, children: [
                    {path: ':rid', component: RecipeModal}
                ]
            },
            {path: 'recipe-editor', component: RecipeEditorComponent},
            {path: 'recipe-editor/:rid', component: RecipeEditorComponent},
            {path: 'random-recipe', component: RandomRecipeComponent},
            {
                path: 'recipes', component: RecipeListComponent, children: [
                    {path: ':rid', component: RecipeModal}
                ], data: {showBackButton: false}
            },
            {path: 'admin/user-list', component: UserListComponent, canActivate: [AuthGuard], data: {roles: [Role.admin]}},
            {path: 'admin/abandoned-recipes', component: OrphanRecipesComponent, canActivate: [AuthGuard], data: {roles: [Role.admin]}},
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DashboardRoutingModule {
}
