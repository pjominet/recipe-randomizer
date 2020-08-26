import {Component, OnInit} from '@angular/core';
import {RecipeService} from '@app/services/recipe.service';
import {Recipe} from '@app/models/recipe';

@Component({
    selector: 'app-recipes',
    templateUrl: './recipes.component.html',
    styleUrls: ['./recipes.component.scss']
})
export class RecipesComponent implements OnInit {

    public recipes: Recipe[] = [];

    constructor(private recipeService: RecipeService) {
    }

    ngOnInit(): void {
        this.recipeService.getRecipes().subscribe(
            recipes => {
                this.recipes = recipes;
            }, error => {
                console.log(error);
            }
        );
    }
}
