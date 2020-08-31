import {Component, OnInit} from '@angular/core';
import {AuthService} from '@app/services/auth.service';
import {User} from '@app/models/identity/user';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
    public user: User;

    constructor(private authService: AuthService) {
        this.user = this.authService.user;
    }

    public ngOnInit(): void {
    }
}
