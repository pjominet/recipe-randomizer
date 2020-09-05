import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {AuthService} from '@app/services/auth.service';
import {Role, User} from '@app/models/identity/user';
import {RecipeService} from '@app/services/recipe.service';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {AlertService} from '@app/components/alert/alert.service';
import {forkJoin} from 'rxjs';
import {UserService} from '@app/services/user.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {environment} from '@env/environment';
import {UpdateRequest} from '@app/models/identity/updateRequest';
import {UploadService} from '@app/services/upload.service';
import {FileUploadRequest} from '@app/models/fileUploadRequest';
import {HttpEventType, HttpResponse} from '@angular/common/http';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
    public user: User;
    public roles: typeof Role = Role;

    public isEditMode: boolean = false;
    public editForm: FormGroup;

    public isSubmitted: boolean = false;
    public isLoading: boolean = false;
    public isUploading: boolean = false;

    @ViewChild('avatarInput', {static: true}) public avatarInput: ElementRef;
    public hasAvatarPreview: boolean;
    public avatarUploadProgress: number = 0;
    private fileUploadRequest: FileUploadRequest;

    constructor(private authService: AuthService,
                private recipeService: RecipeService,
                private formBuilder: FormBuilder,
                private alertService: AlertService,
                private userService: UserService,
                private modalService: NgbModal,
                private uploadService: UploadService) {
        this.user = this.authService.user;
        this._originalAvatar = this.user?.profileImageUri
            ? `${environment.staticFileUrl}/${this.user.profileImageUri}`
            : 'assets/img/avatar_placeholder.png';
        this._userAvatar = this._originalAvatar;
        this.fileUploadRequest = new FileUploadRequest(`${environment.apiUrl}/users/image-upload`);
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

    private _userAvatar: string;
    private readonly _originalAvatar: string;

    public get userAvatar(): string {
        return this._userAvatar;
    }

    public set userAvatar(base64Img: string) {
        this._userAvatar = base64Img;
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
            email: [this.user.email, [Validators.required, Validators.email]],
            role: [{value: this.user.role, disabled: this.user.role !== Role.admin}, Validators.required]
        });
    }

    public onSubmit(): void {
        this.isSubmitted = true;
        this.alertService.clear();

        if (this.editForm.invalid) {
            return;
        }

        this.isLoading = true;

        this.userService.updateUser(this.user.id, new UpdateRequest({
            username: this.f.username.value,
            email: this.f.email.value,
            role: this.f.role.value
        })).subscribe(
            () => {
                this.isLoading = false;
                this.isEditMode = false;
                this.alertService.success('Successfully updated profile');
            }, () => {
                this.isLoading = false;
                this.alertService.success('Profile update failed');
            });
    }

    public openDeleteConfirmation(modalContent: any): void {
        this.modalService.open(modalContent);
    }

    public deleteAccount(): void {
        this.modalService.dismissAll();
        this.userService.deleteUser(this.user.id).subscribe();
    }

    public stageAvatar(): void {
        const image = this.avatarInput.nativeElement.files[0];
        this.AsBase64(image)
            .then(preview => {
                this.userAvatar = preview;
                this.hasAvatarPreview = true;
                this.fileUploadRequest.file = image;
            }).catch(error => {
            this.alertService.error(error, {autoCloseTimeOut: 5000});
            this.resetAvatar();
        });
    }

    public uploadAvatar(): void {
        this.isUploading = true;
        this.alertService.clear();

        this.fileUploadRequest.entityId = this.user.id;
        this.uploadService.uploadFile(this.fileUploadRequest).subscribe(
            event => {
                if (event.type === HttpEventType.UploadProgress) {
                    this.avatarUploadProgress= Math.round(100 * event.loaded / event.total);
                } else if (event instanceof HttpResponse) {
                    this.userService.getUser(this.user.id).subscribe(user => {
                        this.authService.user = user;
                        this.isUploading = false;
                        this.resetFileInput();
                        this.alertService.success("Successfully updated avatar");
                    });
                }
            },
            () => {
                this.isUploading = false;
                this.resetAvatar();
                this.alertService.error("Avatar update failed");
            });
    }

    public resetAvatar(): void {
        this.resetFileInput();
        this.userAvatar = this._originalAvatar;
    }

    private resetFileInput(): void {
        this.hasAvatarPreview = false;
        this.fileUploadRequest.file = null;
        this.avatarInput.nativeElement.value = '';
    }

    private AsBase64(file: File): Promise<any> {
        const reader = new FileReader();
        return new Promise((resolve, reject) => {
            reader.addEventListener('load',
                () => {
                    const result = reader.result as string;
                    if (!result) {
                        reject('Cannot read file');
                    }
                    // Note: 2**21 = 2*2**20 is how many bytes there are in 2MB
                    if (result.length * 2 > 2 ** 21) {
                        reject('File exceeds the maximum allowed size');
                    }
                    resolve(reader.result);
                });

            reader.addEventListener('error', event => reject(event));

            reader.readAsDataURL(file);
        });
    }
}
