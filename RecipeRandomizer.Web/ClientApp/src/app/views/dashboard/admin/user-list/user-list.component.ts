import {Component, OnInit} from '@angular/core';
import {UserService} from '@app/services/user.service';
import {first} from 'rxjs/operators';
import {IUser, Role} from '@app/models/identity/user';
import {AlertService} from '@app/components/alert/alert.service';
import {AuthService} from '@app/services/auth.service';
import {LockRequest} from '@app/models/identity/LockRequest';

@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: []
})
export class UserListComponent implements OnInit {

    public currentUser: IUser;
    public users: IUser[] = [];
    public roles: typeof Role = Role;

    public isToggling: boolean = false;
    public isDeleting: boolean = false;

    constructor(private userService: UserService,
                private authService: AuthService,
                private alertService: AlertService) {
        this.currentUser = this.authService.user;
    }

    public ngOnInit(): void {
        this.userService.getUsers().subscribe(
            users => {
                this.users = users;
            });
    }

    public deleteUser(id: number): void {
        this.alertService.clear();
        this.isDeleting = true;
        this.userService.deleteUser(id)
            .pipe(first())
            .subscribe(() => {
                this.users = this.users.filter(x => x.id !== id);
                this.isDeleting = false;
                this.alertService.success(`Successfully deleted user: ${id}`);
            }, error => {
                this.isDeleting = false;
                this.alertService.error('User could not be deleted');
            });
    }

    public toggleUserLock(user: IUser): void {
        this.alertService.clear();
        this.isToggling = true;

        this.userService.toggleUserLock(user.id, new LockRequest(!user.lockedOn, !user.lockedOn ? this.currentUser.id : null)).subscribe(
            () => {
                let user = this.users.find(u => u.id === user.id);
                user.isLocked = true;
                user.lockedBy = this.currentUser;
                this.isToggling = false;
                this.alertService.success(`Successfully locked out user: ${user.username}`);
            }, error => {
                this.isToggling = false;
                this.alertService.error('User could not be locked out');
            }
        )
    }

    // TODO: add possibility to change role of other users

}
