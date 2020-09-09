import {Injectable} from '@angular/core';
import {Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot} from '@angular/router';

import {AuthService} from '@app/services/auth.service';
import {AlertService} from '@app/components/alert/alert.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {
    constructor(private router: Router,
                private authService: AuthService,
                private alertService: AlertService) {
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        this.alertService.clear();
        const user = this.authService.user;
        if (user) {
            // check if route is restricted by role
            if (route.data.roles && !route.data.roles.includes(user.role)) {
                // role not authorized so redirect to home page
                this.alertService.error("You are not authorized to access this resource!", {keepAfterRouteChange: true});
                this.router.navigate(['/']);
                return false;
            }

            // authorized so return true
            return true;
        }

        this.alertService.info("Your session has expired!", {keepAfterRouteChange: true, autoCloseTimeOut: 2000});
        // not logged in so redirect to login page with the return url
        this.router.navigate(['/auth/login'], {queryParams: {returnUrl: state.url}});
        return false;
    }
}
