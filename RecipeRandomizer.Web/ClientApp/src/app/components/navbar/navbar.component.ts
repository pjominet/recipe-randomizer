import {Component} from '@angular/core';
import {AuthService} from '../../services/auth.service';
import {IUser, Role} from '../../models/identity/user';

@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {

    public user: IUser;
    public roles: typeof Role = Role;

    constructor(private authService: AuthService) {
        this.user = this.authService.user;
    }
}
