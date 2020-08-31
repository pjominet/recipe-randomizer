import {Component, OnInit} from '@angular/core';
import {TagCategory} from '@app/models/nomenclature/tagCategory';
import {Tag} from '@app/models/nomenclature/tag';
import {Recipe} from '@app/models/recipe';
import {RecipeService} from '@app/services/recipe.service';
import {TagService} from '@app/services/tag.service';
import {AuthService} from '@app/services/auth.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

    public tagCategories: TagCategory[] = [];
    public selectedTags: Tag[] = [];
    public recipe: Recipe;

    public isLoading: boolean = false;

    get isLoggedIn(): boolean {
        return this.authService.user !== null;
    }

    constructor(private recipeService: RecipeService,
                private tagService: TagService,
                private authService: AuthService) {
    }

    public ngOnInit(): void {
        this.tagService.getTagCategories().subscribe(categories => {
            this.tagCategories = categories;
        });
    }

    public getRandomRecipe(): void {
        this.isLoading = true;
        this.recipeService.getRandomRecipe(this.selectedTags.map(t => t.id)).subscribe(recipe => {
            this.recipe = recipe;
            this.loadTimeOut();
        }, error => {
            console.log(error);
            this.loadTimeOut();
        });
    }

    private loadTimeOut(): void {
        setTimeout(() => this.isLoading = false, 1000);
    }
}
