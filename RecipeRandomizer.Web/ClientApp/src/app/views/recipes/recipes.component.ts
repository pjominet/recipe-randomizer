import {Component, OnInit} from '@angular/core';
import {RecipeService} from '@app/services/recipe.service';
import {Recipe} from '@app/models/recipe';
import {TagCategory} from '@app/models/nomenclature/tagCategory';
import {Tag} from '@app/models/nomenclature/tag';
import {TagService} from '@app/services/tag.service';
import {ActivatedRoute, Router} from '@angular/router';

@Component({
    selector: 'app-recipes',
    templateUrl: './recipes.component.html',
    styleUrls: []
})
export class RecipesComponent implements OnInit {

    public recipes: Recipe[] = [];

    public tagCategories: TagCategory[] = [];
    public selectedTags: Tag[] = [];

    public get showBackButton(): boolean {
        return this.route.snapshot.data.showBackButton;
    }

    constructor(private recipeService: RecipeService,
                private tagService: TagService,
                private route: ActivatedRoute,
                private router: Router) {
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

    public showRecipe(id: number): void {
        const queryParams = this.route.snapshot.queryParams;
        this.router.navigate([id], {relativeTo: this.route, queryParams: queryParams});
    }
}
