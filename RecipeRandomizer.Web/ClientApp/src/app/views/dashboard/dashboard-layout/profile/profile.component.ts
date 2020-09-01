import {Component, OnInit} from '@angular/core';
import {AuthService} from '@app/services/auth.service';
import {User} from '@app/models/identity/user';
import {RecipeService} from '@app/services/recipe.service';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {AlertService} from '@app/components/alert/alert.service';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
    public user: User;
    public isEditMode: boolean = false;
    public editForm: FormGroup;
    public isSubmitted: boolean = false;
    public isLoading: boolean = false;

    constructor(private authService: AuthService,
                private recipeService: RecipeService,
                private formBuilder: FormBuilder,
                private alertService: AlertService) {
        this.user = this.authService.user;
    }

    // convenience getter for easy access to form fields
    public get f() {
        return this.editForm.controls;
    }

    public get createdRecipeCount(): number {
        return this.user.recipes?.filter(r => !r.isDeleted).length ?? 0;
    }

    public get deletedRecipeCount(): number {
        return this.user.recipes?.filter(r => r.isDeleted).length ?? 0;
    }

    public get likedRecipeCount(): number {
        return this.user.likedRecipes?.length ?? 0;
    }

    public ngOnInit(): void {
        this.recipeService.getRecipes([], this.user.id).subscribe(
            recipes => {
                this.user.recipes = recipes;
            });

        this.recipeService.getLikedRecipes(this.user.id).subscribe(
            recipes => {
                this.user.likedRecipes = recipes;
            });

        this.editForm = this.formBuilder.group({
            username: [this.user.userName, Validators.required],
            email: [this.user.email, [Validators.required, Validators.email]]
        });
    }

    public onSubmit(): void {
        this.isSubmitted = true;

        // reset alerts on submit
        this.alertService.clear();

        // stop here if form is invalid
        if (this.editForm.invalid) {
            return;
        }

        this.isLoading = true;

        this.alertService.success(`Edited values: ${this.f.values}`);
    }
}
