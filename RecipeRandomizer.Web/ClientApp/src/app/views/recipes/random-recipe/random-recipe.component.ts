import {Component, OnInit} from '@angular/core';
import {TagCategory} from '@app/models/nomenclature/tagCategory';
import {Tag} from '@app/models/nomenclature/tag';
import {RecipeService} from '@app/services/recipe.service';
import {TagService} from '@app/services/tag.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {AlertService} from '@app/components/alert/alert.service';
import {RecipeComponent} from '@app/views/recipes/recipe/recipe.component';

@Component({
    selector: 'app-random-recipe',
    templateUrl: './random-recipe.component.html',
    styleUrls: ['./random-recipe.component.scss']
})
export class RandomRecipeComponent implements OnInit {

    public tagCategories: TagCategory[] = [];
    public selectedTags: Tag[] = [];

    public isFetching: boolean = false;

    constructor(private recipeService: RecipeService,
                private tagService: TagService,
                private modalService: NgbModal,
                private alertService: AlertService) {
    }

    public ngOnInit(): void {
        this.tagService.getTagCategories().subscribe(categories => {
            this.tagCategories = categories;
        });
    }

    public getRandomRecipe(): void {
        this.isFetching = true;
        this.alertService.clear();
        this.recipeService.getRandomRecipe(this.selectedTags.map(t => t.id)).subscribe(recipeId => {
            this.loadTimeOut(() => {
                let modalRef = this.modalService.open(RecipeComponent, {size: 'lg', scrollable: true});
                modalRef.componentInstance.id = recipeId;
            });
        }, () => {
            this.loadTimeOut(() => {
                this.alertService.info("No recipe matches the current filter selection :(", {autoCloseTimeOut: 5000});
            });
        });
    }

    private loadTimeOut(callback?: Function): void {
        setTimeout(() => {
            this.isFetching = false
            if (callback)
                callback();
        }, 1000);
    }

}
