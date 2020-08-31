import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '@app/services/auth.service';
import {AlertService} from '@app/components/alert/alert.service';
import {first} from 'rxjs/operators';

enum EmailStatus {
    Verifying,
    Failed
}

@Component({
    selector: 'app-verify-email',
    templateUrl: './verify-email.component.html',
    styleUrls: []
})
export class VerifyEmailComponent implements OnInit {
    public emailStatus: typeof EmailStatus = EmailStatus;
    public currentStatus: EmailStatus = EmailStatus.Verifying;

    constructor(private route: ActivatedRoute,
                private router: Router,
                private authService: AuthService,
                private alertService: AlertService) {
    }

    public ngOnInit(): void {
        const token = this.route.snapshot.queryParams['token'];

        // remove token from url to prevent http referer leakage
        this.router.navigate([], {relativeTo: this.route, replaceUrl: true});

        this.authService.verifyEmail(token)
            .pipe(first())
            .subscribe({
                next: () => {
                    this.alertService.success('Verification successful, you can now login', {keepAfterRouteChange: true});
                    this.router.navigate(['../login'], {relativeTo: this.route});
                },
                error: () => {
                    this.currentStatus = EmailStatus.Failed;
                }
            });
    }

}
