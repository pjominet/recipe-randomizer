import {Component, OnInit} from '@angular/core';
import {first} from 'rxjs/operators';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '@app/services/auth.service';
import {AlertService} from '@app/components/alert/alert.service';
import {MustMatchValidator} from '@app/helpers/must-match.validator';
import {ResetPasswordRequest} from '@app/models/identity/resetPasswordRequest';

enum TokenStatus {
    Validating,
    Valid,
    Invalid
}

@Component({
    selector: 'app-reset-password',
    templateUrl: './reset-password.component.html',
    styleUrls: []
})
export class ResetPasswordComponent implements OnInit {

    public tokenStatus: typeof TokenStatus = TokenStatus;
    public currentStatus: TokenStatus = TokenStatus.Validating;
    public token: string = null;
    public passwordResetForm: FormGroup;
    public isLoading: boolean = false;
    public isSubmitted: boolean = false;

    constructor(private formBuilder: FormBuilder,
                private route: ActivatedRoute,
                private router: Router,
                private authService: AuthService,
                private alertService: AlertService) {
    }

    // convenience getter for easy access to form fields
    get f() {
        return this.passwordResetForm.controls;
    }

    public ngOnInit(): void {
        this.passwordResetForm = this.formBuilder.group({
            password: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', Validators.required],
        }, {
            validator: MustMatchValidator('password', 'confirmPassword')
        });

        const token = this.route.snapshot.queryParams['token'];

        // remove token from url to prevent http referer leakage
        this.router.navigate([], {relativeTo: this.route, replaceUrl: true});

        this.authService.validateResetToken(token)
            .pipe(first())
            .subscribe({
                next: () => {
                    this.token = token;
                    this.currentStatus = TokenStatus.Valid;
                },
                error: () => {
                    this.currentStatus = TokenStatus.Invalid;
                }
            });
    }

    public onSubmit(): void {
        this.isSubmitted = true;

        // reset alerts on submit
        this.alertService.clear();

        // stop here if form is invalid
        if (this.passwordResetForm.invalid) {
            return;
        }

        this.isLoading = true;
        this.authService.resetPassword(new ResetPasswordRequest(this.token, this.f.password.value, this.f.confirmPassword.value))
            .pipe(first())
            .subscribe({
                next: () => {
                    this.alertService.success('Password reset successful, you can now login', {keepAfterRouteChange: true});
                    this.router.navigate(['../login'], {relativeTo: this.route});
                },
                error: error => {
                    this.alertService.error(error);
                    this.isLoading = false;
                }
            });
    }
}
