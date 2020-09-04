import {Component, Input, OnInit} from '@angular/core';
import {UserService} from '@app/services/user.service';
import {IUser} from '@app/models/identity/user';
import {AlertService} from '@app/components/alert/alert.service';
import {RecipeService} from '@app/services/recipe.service';
import {AttributionRequest} from '@app/models/attributionRequest';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';
import {AttributionService} from './attribution.service';

@Component({
    templateUrl: './attribution.component.html',
    styleUrls: []
})
export class AttributionComponent implements OnInit {

    @Input() public recipeId: number;
    public existingUsers: IUser[] = [];
    public selectedUserId: number;

    public isLoading: boolean = false;
    public isSubmitted: boolean = false;

    constructor(private userService: UserService,
                private alertService: AlertService,
                private recipeService: RecipeService,
                public activeModal: NgbActiveModal,
                private attributionService: AttributionService) {
    }

    public ngOnInit(): void {
        this.isLoading = true;
        this.userService.getUsers().subscribe(
            users => {
                this.existingUsers = users;
                this.isLoading = false;
            });
    }

    public onSubmit(): void {
        this.isSubmitted = true;
        this.alertService.clear();

        this.recipeService.attributeRecipe(new AttributionRequest(this.selectedUserId, this.recipeId)).subscribe(
            () => {
                this.activeModal.dismiss();
                this.attributionService.attributionSuccess(this.recipeId);
                this.alertService.success("Successfully attribute the recipe");
            }, () => {
                this.activeModal.dismiss();
                this.alertService.error("Attribution failed");
            }
        )
    }
}
