import {Component, OnInit} from '@angular/core';
import {RecipeService} from '@app/services/recipe.service';
import {Recipe} from '@app/models/recipe';
import {TagCategory} from '@app/models/nomenclature/tagCategory';
import {Tag} from '@app/models/nomenclature/tag';
import {TagService} from '@app/services/tag.service';

@Component({
    selector: 'app-recipes',
    templateUrl: './recipes.component.html',
    styleUrls: []
})
export class RecipesComponent implements OnInit {

    public recipes: Recipe[] = [];

    public tagCategories: TagCategory[] = [];
    public selectedTags: Tag[] = [];

    constructor(private recipeService: RecipeService,
                private tagService: TagService,) {
    }

    public ngOnInit(): void {
        this.tagService.getTagCategories().subscribe(categories => {
            this.tagCategories = categories;
        });

        this.recipeService.getRecipes().subscribe(
            recipes => {
                this.recipes = recipes;
            }, error => {
                console.log(error);
            }
        );
    }

    public applyFilters(): void {
        console.log(this.selectedTags);
        this.recipeService.getRecipes(this.selectedTags.map(t => t.id)).subscribe(
            recipes => {
                this.recipes = recipes;
            }, error => {
                console.log(error);
            }
        );
    }
}
