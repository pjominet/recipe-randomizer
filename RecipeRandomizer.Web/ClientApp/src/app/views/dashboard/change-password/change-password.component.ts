import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {AuthService} from '@app/services/auth.service';
import {AlertService} from '@app/components/alert/alert.service';
import {MustMatchValidator} from '@app/helpers/must-match.validator';
import {ChangePasswordRequest} from '../../../models/identity/changePasswordRequest';
import {first} from 'rxjs/operators';

@Component({
    selector: 'app-change-password',
    templateUrl: './change-password.component.html',
    styleUrls: []
})
export class ChangePasswordComponent implements OnInit {

    public changePasswordForm: FormGroup;
    public isLoading: boolean = false;
    public isSubmitted: boolean = false;
    public error: string;

    constructor(private formBuilder: FormBuilder,
                private router: Router,
                private authService: AuthService,
                private alertService: AlertService) {
    }

    // convenience getter for easy access to form fields
    get f() {
        return this.changePasswordForm.controls;
    }

    public ngOnInit(): void {
        this.changePasswordForm = this.formBuilder.group({
            password: ['', Validators.required],
            newPassword: ['', [Validators.required, Validators.minLength(6)]],
            confirmNewPassword: ['', Validators.required],
        }, {
            validator: MustMatchValidator('newPassword', 'confirmNewPassword')
        });
    }


    public onSubmit(): void {
        this.isSubmitted = true;

        // reset alerts on submit
        this.alertService.clear();

        // stop here if form is invalid
        if (this.changePasswordForm.invalid) {
            return;
        }

        this.isLoading = true;
        this.authService.changePassword(new ChangePasswordRequest(
            this.f.password.value,
            this.f.newPassword.value,
            this.f.confirmNewPassword.value))
            .pipe(first())
            .subscribe({
                next: (response) => {
                    this.isLoading = false;
                    this.alertService.success(response.message, {keepAfterRouteChange: true});
                    this.router.navigateByUrl('..');
                },
                error: () => {
                    this.isLoading = false;
                    this.error = 'Something went wrong, password was not changed!';
                }
            });
    }
}
