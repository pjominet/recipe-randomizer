import {Component, OnInit} from '@angular/core';
import {AuthService} from '@app/services/auth.service';

@Component({
    selector: 'app-topbar',
    templateUrl: './topbar.component.html',
    styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent implements OnInit {

    constructor(private authService: AuthService) {
    }

    ngOnInit(): void {
    }

    public logout(): void {
        this.authService.logout();
    }
}
