import {Component, OnInit} from '@angular/core';
import {TagCategory} from '@app/models/nomenclature/tagCategory';
import {Tag} from '@app/models/nomenclature/tag';
import {Recipe} from '@app/models/recipe';
import {RecipeService} from '@app/services/recipe.service';
import {TagService} from '@app/services/tag.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {LoginComponent} from '@app/views/login/login.component';
import {RegisterComponent} from '@app/views/register/register.component';

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

    constructor(private recipeService: RecipeService,
                private tagService: TagService,
                private dialogService: NgbModal) {
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

    public openSignInDialog(): void {
        this.dialogService.dismissAll();
        this.dialogService.open(LoginComponent, {centered: true});
    }

    public openSignUpDialog(): void {
        this.dialogService.dismissAll();
        this.dialogService.open(RegisterComponent, {centered: true});
    }

    private loadTimeOut(): void {
        setTimeout(() => this.isLoading = false, 1000);
    }
}
