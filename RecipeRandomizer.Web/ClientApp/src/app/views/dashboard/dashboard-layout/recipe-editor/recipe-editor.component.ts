import {Component, OnInit} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {Recipe} from '@app/models/recipe';
import {User} from '@app/models/identity/user';
import {AuthService} from '@app/services/auth.service';
import {Cost} from '@app/models/nomenclature/cost';
import {Difficulty} from '@app/models/nomenclature/difficulty';
import {QuantityUnit} from '@app/models/nomenclature/quantityUnit';
import {RecipeService} from '@app/services/recipe.service';
import {QuantityService} from '@app/services/quantity.service';
import {TagService} from '@app/services/tag.service';
import {TagCategory} from '@app/models/nomenclature/tagCategory';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-recipe-editor',
    templateUrl: './recipe-editor.component.html',
    styleUrls: ['./recipe-editor.component.scss']
})
export class RecipeEditorComponent implements OnInit {

    public recipeForm: FormGroup;
    public costs: typeof Cost = Cost;
    public difficulties: typeof Difficulty = Difficulty;
    public quantityUnits: QuantityUnit[] = [];
    public isLoading: boolean = false;
    public isSubmitted: boolean = false;
    public isEditMode: boolean = false;

    public user: User;
    public recipe: Recipe;
    public tagCategories: TagCategory[] = [];

    constructor(private route: ActivatedRoute,
                private formBuilder: FormBuilder,
                private authService: AuthService,
                private recipeService: RecipeService,
                private quantityService: QuantityService,
                private tagService: TagService) {
        this.user = this.authService.user;
    }

    public get f() {
        return this.recipeForm.controls;
    }

    public get i() {
        return this.f.ingredients as FormArray;
    }

    public get ic() {
        return this.i.controls as FormGroup[];
    }

    public ngOnInit(): void {

        forkJoin([
            this.quantityService.getQuantityUnits(),
            this.tagService.getTagCategories()
        ]).subscribe(([quantityUnits, tagCategories]) => {
            this.quantityUnits = quantityUnits;
            this.tagCategories = tagCategories;

            this.recipeForm = this.formBuilder.group({
                name: ['', Validators.required],
                description: ['', Validators.required],
                numberOfPeople: [1, Validators.required],
                cost: [Cost.Cheap],
                difficulty: [Difficulty.Easy],
                prepTime: [1, [Validators.required, Validators.min(1)]],
                cookTime: [1, [Validators.required, Validators.min(1)]],
                preparation: ['', Validators.required],
                ingredients: this.formBuilder.array([
                    this.addIngredientGroup()
                ]),
                tags: ['']
            });
        });

        const recipeId = this.route.snapshot.params['rid'];
        if (recipeId) {
            this.isEditMode = true;
            this.recipeService.getRecipe(recipeId).subscribe(
                recipe => {
                    this.recipe = recipe;
                    this.recipeForm.patchValue({
                        name: recipe.name,
                        description: recipe.description,
                        numberOfPeople: recipe.numberOfPeople,
                        cost: recipe.cost,
                        difficulty: recipe.difficulty,
                        prepTime: recipe.prepTime,
                        cookTime: recipe.cookTime,
                        preparation: recipe.preparation,
                        ingredients: recipe.ingredients.forEach((ingredient, index) => {
                            this.i.push(this.addIngredientGroup())
                            this.i.at(index).patchValue({
                                name: ingredient.name,
                                quantity: ingredient.quantity,
                                quantityUnit: ingredient.quantityUnitId
                            });
                        }),
                        tags: recipe.tags
                    }, {emitEvent: true});
                });
        }
    }

    public onIngredientAdd(): void {
        this.i.push(this.addIngredientGroup());

        this.i.markAsDirty();
    }

    public onIngredientRemove(index: number): void {
        this.i.removeAt(index);
    }

    public onSubmit(): void {
        this.isSubmitted = true;

        // stop here if form is invalid
        if (this.recipeForm.invalid) {
            return;
        }

        //TODO: implement service call
    }

    public resetFrom(): void {
        this.isSubmitted = false;
        this.i.clear();
        this.recipeForm.reset();
        // re-add starting elements
        this.onIngredientAdd();
    }

    private addIngredientGroup(): FormGroup {
        return this.formBuilder.group({
            name: ['', Validators.required],
            quantity: [0, [Validators.required, Validators.min(0)]],
            quantityUnit: [this.quantityUnits[0].id]
        });
    }
}
