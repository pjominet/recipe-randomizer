import {Component, OnInit} from '@angular/core';
import {AuthService} from '@app/services/auth.service';
import {User} from '@app/models/identity/user';
import {RecipeService} from '@app/services/recipe.service';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {AlertService} from '@app/components/alert/alert.service';
import {forkJoin} from 'rxjs';
import {UserService} from '@app/services/user.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {environment} from '@env/environment';

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
                private alertService: AlertService,
                private userService: UserService,
                private modalService: NgbModal) {
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

    public get userAvatar(): string {
        return this.user.profileImageUri
            ? `${environment.staticFileUrl}/${this.user.profileImageUri}`
            : 'assets/img/avatar_placeholder.png'
    }

    public ngOnInit(): void {
        forkJoin([
            this.recipeService.getCreatedRecipesForUser(this.user.id),
            this.recipeService.getLikedRecipesForUser(this.user.id)
        ]).subscribe(
            ([createdRecipes, likedRecipes]) => {
                this.user.recipes = createdRecipes;
                this.user.likedRecipes = likedRecipes;
            });

        this.editForm = this.formBuilder.group({
            username: [this.user.username, Validators.required],
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

        // TODO: implement correct service use
        this.alertService.success(`Edited values: ${this.f.values}`);
    }

    public openDeleteConfirmation(modalContent: any): void {
        this.modalService.open(modalContent)
    }

    public deleteAccount(): void {
        this.modalService.dismissAll();
        this.userService.deleteUser(this.user.id).subscribe();
    }
}
