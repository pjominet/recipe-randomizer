import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '@app/services/auth.service';
import {AlertService} from '@app/components/alert/alert.service';
import {first} from 'rxjs/operators';
import {VerificationRequest} from '../../../../models/identity/verificationRequest';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ValidationRequest} from '../../../../models/identity/validationRequest';

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
    public resendCodeForm: FormGroup;
    public isLoading: boolean = false;
    public isSubmitted: boolean = false;

    constructor(private route: ActivatedRoute,
                private router: Router,
                private authService: AuthService,
                private alertService: AlertService,
                private formBuilder: FormBuilder) {
    }

    // convenience getter for easy access to form fields
    get f() {
        return this.resendCodeForm.controls;
    }

    public ngOnInit(): void {
        const token = this.route.snapshot.queryParams['token'];

        // remove token from url to prevent http referer leakage
        this.router.navigate([], {relativeTo: this.route, replaceUrl: true});

        this.authService.verifyEmail(new ValidationRequest(token))
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

        this.resendCodeForm = this.formBuilder.group({
            email: ['', [Validators.required, Validators.email]]
        });
    }

    public onSubmit(): void {
        this.isSubmitted = true;

        // reset alerts on submit
        this.alertService.clear();

        // stop here if form is invalid
        if (this.resendCodeForm.invalid) {
            return;
        }

        this.isLoading = true;
        this.authService.resendEmailVerificationCode(new VerificationRequest(this.f.email.value))
            .pipe(first())
            .subscribe({
                next: (response) => {
                    this.alertService.success(response.message);
                },
                error: (error) => {
                    this.alertService.success(error.message);
                }
            })
    }
}
