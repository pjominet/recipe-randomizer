import {Component, OnInit} from '@angular/core';
import {TagCategory} from '@app/models/nomenclature/tagCategory';
import {Tag} from '@app/models/nomenclature/tag';
import {RecipeService} from '@app/services/recipe.service';
import {TagService} from '@app/services/tag.service';
import {AuthService} from '@app/services/auth.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {RecipeComponent} from '@app/views/recipes/recipe/recipe.component';
import {AlertService} from '@app/components/alert/alert.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

    public tagCategories: TagCategory[] = [];
    public selectedTags: Tag[] = [];

    public isLoading: boolean = false;

    get isLoggedIn(): boolean {
        return this.authService.user !== null;
    }

    constructor(private recipeService: RecipeService,
                private tagService: TagService,
                private authService: AuthService,
                private modalService: NgbModal,
                private alertService: AlertService) {
    }

    public ngOnInit(): void {
        this.tagService.getTagCategories().subscribe(categories => {
            this.tagCategories = categories;
        });
    }

    public getRandomRecipe(): void {
        this.isLoading = true;
        this.alertService.clear();
        this.recipeService.getRandomRecipe(this.selectedTags.map(t => t.id)).subscribe(recipeId => {
            this.loadTimeOut(() => {
                let modalRef = this.modalService.open(RecipeComponent, {size: 'lg', scrollable: true});
                modalRef.componentInstance.id = recipeId;
            });
        }, error => {
            this.loadTimeOut(() => {
                this.alertService.info("No recipe matches the current filter selection :(", {autoCloseTimeOut: 5000});
            });
        });
    }

    public logout(): void {
        this.authService.logout();
    }

    private loadTimeOut(callback?: Function): void {
        setTimeout(() => {
            this.isLoading = false
            if (callback)
                callback();
        }, 1000);
    }
}
