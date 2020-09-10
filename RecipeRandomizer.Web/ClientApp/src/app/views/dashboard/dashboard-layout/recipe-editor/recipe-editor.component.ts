import {Component, OnInit, ViewEncapsulation} from '@angular/core';
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
import {forkJoin, Observable, Subject} from 'rxjs';
import {AlertService} from '@app/components/alert/alert.service';
import {FileUploadRequest} from '@app/models/fileUploadRequest';
import {environment} from '@env/environment';
import {UploadService} from '@app/services/upload.service';
import {HttpEventType, HttpResponse} from '@angular/common/http';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {FileUploadService} from '@app/components/file-upload/file-upload.service';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import {ConfirmationDialogComponent} from '@app/components/confirmation-dialog/confirmation-dialog.component';

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
    public recipe: Recipe = new Recipe();
    public tagCategories: TagCategory[] = [];
    public fileUploadRequest: FileUploadRequest;
    public fileUploadSuccess: boolean = false;
    public changeImage: boolean = false;
    public imageUploadProgress: number = 0;

    public editor = ClassicEditor;
    public editorConfig = {
        removePlugin: [
            'CKFinderUploadAdapter',
            'CKFinder',
            'EasyImage',
            'Image',
            'ImageCaption',
            'ImageStyle',
            'ImageToolbar',
            'ImageUpload',
            'Link',
            'MediaEmbed',
        ],
        toolbar: [
            'heading', '|',
            'bold', 'italic', 'bulletedList', 'numberedList', '|',
            'undo', 'redo'
        ],
        heading: {
            options: [
                {model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph'},
                {model: 'heading1', view: 'h4', title: 'Heading 1', class: 'ck-heading_heading1'},
                {model: 'heading2', view: 'h5', title: 'Heading 2', class: 'ck-heading_heading2'}
            ]
        }
    };

    constructor(private route: ActivatedRoute,
                private formBuilder: FormBuilder,
                private authService: AuthService,
                private recipeService: RecipeService,
                private quantityService: QuantityService,
                private tagService: TagService,
                private alertService: AlertService,
                private uploadService: UploadService,
                private modalService: NgbModal,
                private fileUploadService: FileUploadService) {
        this.user = this.authService.user;
        this.fileUploadRequest = new FileUploadRequest(`${environment.apiUrl}/recipes/image-upload`);
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

            this.onFormValueChanged();
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
                        tags: recipe.tags,
                    }, {emitEvent: false});

                    recipe.ingredients.forEach((ingredient, index) => {
                        if (index !== 0) { // first group already exists
                            this.i.push(this.addIngredientGroup());
                        }
                        this.i.at(index).patchValue({
                            name: ingredient.name,
                            quantity: ingredient.quantity,
                            quantityUnitId: ingredient.quantityUnitId
                        }, {emitEvent: false});
                    });

                    this.recipeForm.updateValueAndValidity();
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

    public onFileStaged(file: File): void {
        this.fileUploadRequest.file = file;
    }

    public onSubmit(): void {
        this.isSubmitted = true;

        // stop here if form is invalid
        if (this.recipeForm.invalid) {
            return;
        }

        this.alertService.clear();
        this.isLoading = true;

        this.recipe.userId = this.user.id;
        if (this.isEditMode) {
            this.recipeService.updateRecipe(this.recipe).subscribe(
                response => {
                    this.resetView();
                    this.onEditSuccess(response, 'Successfully updated this recipe!');
                }, error => {
                    this.resetView();
                    this.alertService.error('Recipe could not be updated.');
                });
        } else {
            this.recipeService.addRecipe(this.recipe).subscribe(
                newRecipe => {
                    this.resetFrom();
                    this.resetView();
                    this.onEditSuccess(newRecipe.id, 'Successfully created a new recipe!');
                }, error => {
                    this.resetView();
                    this.alertService.error('Recipe could not be created.');
                });
        }
    }

    public resetFrom(): void {
        this.isSubmitted = false;
        this.i.clear();
        this.recipeForm.reset();
        // re-add starting element
        this.onIngredientAdd();
    }

    public canDeactivate(): Observable<boolean> | Promise<boolean> | boolean {
        if (!this.recipeForm.pristine && !this.isSubmitted) {
            const navigationConfirmation = new Subject<boolean>();

            const modalRef = this.modalService.open(ConfirmationDialogComponent,
                {size: 'sm', keyboard: false, backdrop: 'static'});
            modalRef.result.then(result => {
                navigationConfirmation.next(result);
                navigationConfirmation.complete();
            });

            return navigationConfirmation.asObservable();
        }

        return true;
    }

    private resetView(): void {
        this.isLoading = false;
        window.scrollTo(0, 0);
    }

    private addIngredientGroup(): FormGroup {
        return this.formBuilder.group({
            name: ['', Validators.required],
            quantity: [0, [Validators.required, Validators.min(0)]],
            quantityUnitId: [this.quantityUnits[0].id]
        });
    }

    private onFormValueChanged(): void {
        this.recipeForm.valueChanges.subscribe((values: Partial<Recipe>) => {
            Object.assign(this.recipe, values);
        });
    }

    private onEditSuccess(recipeId: number, successMessage: string): void {
        if (this.fileUploadRequest.file) {
            // complete the file upload request
            this.fileUploadRequest.entityId = recipeId;
            this.uploadService.uploadFile(this.fileUploadRequest).subscribe(
                event => {
                    if (event.type === HttpEventType.UploadProgress) {
                        this.imageUploadProgress = Math.round(100 * event.loaded / event.total);
                    } else if (event instanceof HttpResponse) {
                        this.isLoading = false;
                        this.fileUploadSuccess = true;
                        this.fileUploadService.setFileUploadSuccess();
                        this.alertService.success(successMessage);
                    }
                },
                error => {
                    this.isLoading = false;
                    this.alertService.error(error);
                });
        } else {
            this.alertService.success(successMessage);
            this.isLoading = false;
        }
    }
}
