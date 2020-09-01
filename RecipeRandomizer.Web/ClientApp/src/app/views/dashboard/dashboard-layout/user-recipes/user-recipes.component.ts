import {Component, OnInit} from '@angular/core';
import {Recipe} from '@app/models/recipe';
import {RecipeService} from '@app/services/recipe.service';
import {User} from '@app/models/identity/user';
import {AuthService} from '@app/services/auth.service';

@Component({
    selector: 'app-user-recipes',
    templateUrl: './user-recipes.component.html',
    styleUrls: ['./user-recipes.component.scss']
})
export class UserRecipesComponent implements OnInit {

    public recipes: Recipe[] = [];
    public user: User;

    constructor(private recipeService: RecipeService,
                private authService: AuthService) {
        this.user = this.authService.user;
    }

    public ngOnInit(): void {
        this.recipeService.getRecipes([], this.user.id).subscribe(
            recipes => {
                this.recipes = recipes;
            }, error => {
                console.log(error);
            }
        );
    }

}
