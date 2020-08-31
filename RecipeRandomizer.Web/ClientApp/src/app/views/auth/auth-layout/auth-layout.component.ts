import {Component} from '@angular/core';
import {Router} from '@angular/router';
import {AuthService} from '@app/services/auth.service';

@Component({
    selector: 'app-auth-layout',
    templateUrl: './auth-layout.component.html'
})
export class AuthLayoutComponent {

    constructor(private router: Router,
                private accountService: AuthService
    ) {
        // redirect to home if already logged in
        if (this.accountService.user) {
            this.router.navigate(['/']);
        }
    }
}
