import {Component, OnInit} from '@angular/core';
import {AuthService} from '@app/services/auth.service';
import {User} from '../../models/identity/user';
import {environment} from '../../../environments/environment';

@Component({
    selector: 'app-topbar',
    templateUrl: './topbar.component.html',
    styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent implements OnInit {

    public user: User;

    constructor(private authService: AuthService) {
        this.user = this.authService.user;
    }

    public get userAvatar(): string {
        return this.user.profileImageUri
            ? `${environment.staticFileUrl}/${this.user.profileImageUri}`
            : 'assets/img/avatar_placeholder.png'
    }

    ngOnInit(): void {
    }

    public logout(): void {
        this.authService.logout();
    }
}
