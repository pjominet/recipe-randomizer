import {Component, OnInit} from '@angular/core';
import {AuthService} from '@app/services/auth.service';
import {User} from '../../models/identity/user';

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

    ngOnInit(): void {
    }

    public logout(): void {
        this.authService.logout();
    }
}
