import {Component, OnInit} from '@angular/core';
import {UserService} from '@app/services/user.service';
import {first} from 'rxjs/operators';
import {IUser, Role} from '@app/models/identity/user';
import {AlertService} from '@app/components/alert/alert.service';
import {AuthService} from '@app/services/auth.service';
import {LockRequest} from '@app/models/identity/LockRequest';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {ChangeRoleComponent} from '../change-role/change-role.component';
import {ChangeRoleService} from '../change-role/change-role.service';

@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: []
})
export class UserListComponent implements OnInit {

    public currentUser: IUser;
    public users: IUser[] = [];
    public roles: typeof Role = Role;

    constructor(private userService: UserService,
                private authService: AuthService,
                private alertService: AlertService,
                private modalService: NgbModal,
                private changeRoleService: ChangeRoleService) {
        this.currentUser = this.authService.user;
    }

    public ngOnInit(): void {
        this.userService.getUsers().subscribe(
            users => {
                this.users = users;
            });

        this.changeRoleService.onChange().subscribe(
            user => {
                this.users.find(u => u.id == user.id).role = user.role;
            });
    }

    public deleteUser(id: number): void {
        this.alertService.clear();
        this.userService.deleteUser(id)
            .pipe(first())
            .subscribe(() => {
                this.users = this.users.filter(x => x.id !== id);
                this.alertService.success(`Successfully deleted user: ${id}`);
            }, error => {
                this.alertService.error('User could not be deleted');
            });
    }

    public toggleUserLock(user: IUser): void {
        this.alertService.clear();

        this.userService.toggleUserLock(user.id, new LockRequest(!user.lockedOn, !user.lockedOn ? this.currentUser.id : null)).subscribe(
            () => {
                let user = this.users.find(u => u.id === user.id);
                user.lockedBy = this.currentUser;
                this.alertService.success(`Successfully locked out user: ${user.username}`);
            }, error => {
                this.alertService.error(`${user.username}'s role could not be locked out`);
            }
        );
    }

    public showChangeRoleDialog(user: IUser): void {
        const modalRef = this.modalService.open(ChangeRoleComponent, {centered: true, backdrop: 'static'});
        modalRef.componentInstance.user = user;
    }
}
