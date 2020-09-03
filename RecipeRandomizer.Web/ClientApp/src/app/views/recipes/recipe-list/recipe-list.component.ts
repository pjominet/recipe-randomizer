import {Component, OnInit} from '@angular/core';
import {Recipe} from '@app/models/recipe';
import {TagCategory} from '@app/models/nomenclature/tagCategory';
import {Tag} from '@app/models/nomenclature/tag';
import {RecipeService} from '@app/services/recipe.service';
import {TagService} from '@app/services/tag.service';
import {ActivatedRoute, Router} from '@angular/router';
import {Difficulty} from '@app/models/nomenclature/difficulty';

@Component({
    selector: 'app-recipe-list',
    templateUrl: './recipe-list.component.html',
    styleUrls: []
})
export class RecipeListComponent implements OnInit {

    public recipes: Recipe[] = [];
    public difficulties: typeof Difficulty = Difficulty;

    public tagCategories: TagCategory[] = [];
    public selectedTags: Tag[] = [];

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
