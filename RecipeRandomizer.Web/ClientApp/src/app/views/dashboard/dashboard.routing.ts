import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';

import {DashboardLayoutComponent} from './dashboard-layout/dashboard-layout.component';
import {ProfileComponent} from './dashboard-layout/profile/profile.component';
import {UserRecipesComponent} from './dashboard-layout/user-recipes/user-recipes.component';
import {RecipeEditorComponent} from './dashboard-layout/recipe-editor/recipe-editor.component';
import {RandomRecipeComponent} from '@app/views/recipes/random-recipe/random-recipe.component';
import {RecipesComponent} from '@app/views/recipes/recipes.component';
import {RecipeModal} from '@app/views/recipes/recipe/recipe-modal';
import {ChangePasswordModal} from './change-password/change-password-modal';

const routes: Routes = [
    {
        path: '', component: DashboardLayoutComponent, children: [
            {
                path: '', component: ProfileComponent, children: [
                    {path: 'change-password', component: ChangePasswordModal}
                ]
            },
            {path: 'profile', redirectTo: '', pathMatch: 'full'},
            {path: 'my-recipes', component: UserRecipesComponent},
            {path: 'recipe-editor', component: RecipeEditorComponent},
            {path: 'random-recipe', component: RandomRecipeComponent},
            {
                path: 'recipes', component: RecipesComponent, children: [
                    {path: ':rid', component: RecipeModal}
                ], data: {showBackButton: false}
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DashboardRoutingModule {
}
