import {Component, OnDestroy, OnInit} from '@angular/core';
import {Recipe} from '@app/models/recipe';
import {TagCategory} from '@app/models/nomenclature/tagCategory';
import {RecipeService} from '@app/services/recipe.service';
import {TagService} from '@app/services/tag.service';
import {ActivatedRoute, Router} from '@angular/router';
import {Difficulty} from '@app/models/nomenclature/difficulty';
import {FormBuilder, FormGroup} from '@angular/forms';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-recipe-list',
    templateUrl: './recipe-list.component.html',
    styleUrls: []
})
export class RecipeListComponent implements OnInit, OnDestroy {

    public recipes: Recipe[] = [];
    public difficulties: typeof Difficulty = Difficulty;

    public tagCategories: TagCategory[] = [];
    public selectionForm: FormGroup;
    private sub: Subscription;

    constructor(private recipeService: RecipeService,
                private tagService: TagService,
                private route: ActivatedRoute,
                private router: Router,
                private formBuilder: FormBuilder) {
    }

    public ngOnInit(): void {
        this.selectionForm = this.formBuilder.group({
            tags: ['']
        });

        this.onSelectionChange();

        this.tagService.getTagCategories().subscribe(categories => {
            this.tagCategories = categories;

            const params = this.route.snapshot.queryParams['tag']
            const paramTagIds = Array.isArray(params) ? params.map(p => +p) : [+params];
            const tags = [].concat.apply([], this.tagCategories.map(tc => tc.tags));

            if(paramTagIds.length > 0){
                this.selectionForm.patchValue({
                    tags: tags.filter(t => paramTagIds.includes(t.id))
                }, {emitEvent: true});
            }
        });
    }

    public onSelectionChange(): void {
        this.sub = this.selectionForm.valueChanges.subscribe(
            (formControls) => {
                this.applyFilters(formControls.tags.map(t => t.id));
            });
    }

    private applyFilters(selectedTagIds: number[]): void {
        this.router.navigate([], {relativeTo: this.route, replaceUrl: true, queryParams: {tag: selectedTagIds}});

        if(selectedTagIds.length > 0) {
            this.recipeService.getRecipes(selectedTagIds).subscribe(
                recipes => {
                    this.recipes = recipes.sort((a, b) => {
                        return new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime();
                    });
                });
        } else this.recipeService.getRecipes().subscribe(recipes => {
            this.recipes = recipes.sort((a, b) => {
                return new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime();
            });
        });
    }

    public showRecipe(id: number): void {
        const queryParams = this.route.snapshot.queryParams;
        this.router.navigate([id], {relativeTo: this.route, queryParams: queryParams});
    }

    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }
}
