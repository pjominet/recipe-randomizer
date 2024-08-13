import {Component, Input} from '@angular/core';
import {IUser, Role} from '@app/models/identity/user';
import {UserService} from '@app/services/user.service';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';
import {ChangeRoleService} from './change-role.service';
import {AlertService} from '@app/components/alert/alert.service';
import {RoleUpdateRequest} from '@app/models/identity/roleUpdateRequest';

@Component({
    selector: 'app-change-role',
    templateUrl: './change-role.component.html',
    styleUrls: []
})
export class ChangeRoleComponent {

    @Input() public user: IUser
    public roles: typeof Role = Role;
    public isLoading: boolean = false;

    constructor(private userService: UserService,
                public activeModal: NgbActiveModal,
                private changeRoleService: ChangeRoleService,
                private alertService: AlertService,) {
    }

    public onSubmit(): void {
        this.isLoading = true;
        this.alertService.clear();

        this.userService.updateUserRole(this.user.id, new RoleUpdateRequest(this.user.role)).subscribe(
            () => {
                this.isLoading = false;
                this.activeModal.dismiss();
                this.changeRoleService.changeSuccess(this.user);
                this.alertService.success(`Successfully changed ${this.user.username}'s role.`);
            }, error => {
                this.isLoading = false;
                this.activeModal.dismiss();
                this.alertService.error(error);
            }
        );
    }
}
