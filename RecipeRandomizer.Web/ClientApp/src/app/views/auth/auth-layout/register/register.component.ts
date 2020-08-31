import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '@app/services/auth.service';
import {first} from 'rxjs/operators';
import {RegisterRequest} from '@app/models/identity/registerRequest';
import {MustMatchValidator} from '@app/helpers/must-match.validator';
import {AlertService} from '@app/components/alert/alert.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: []
})
export class RegisterComponent implements OnInit {

    public registerForm: FormGroup;
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
        return this.registerForm.controls;
    }

    public ngOnInit(): void {
        this.registerForm = this.formBuilder.group({
            userName: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', Validators.required],
            acceptTerms: [false, Validators.requiredTrue]
        }, {
            validators: MustMatchValidator('password', 'confirmPassword')
        });
    }

    public onSubmit(): void {
        this.isSubmitted = true;

        // reset alerts on submit
        this.alertService.clear();

        // stop here if form is invalid
        if (this.registerForm.invalid) {
            return;
        }

        this.isLoading = true;
        this.authService.register(new RegisterRequest(
            this.f.userName.value,
            this.f.email.value,
            this.f.password.value,
            this.f.confirmPassword.value,
            this.f.acceptTerms.value))
            .pipe(first())
            .subscribe({
                next: () => {
                    this.alertService.success('Registration successful, please check your email for verification instructions', {keepAfterRouteChange: true});
                    this.router.navigate(['../login'], {relativeTo: this.route});
                },
                error: error => {
                    this.alertService.error(error);
                    this.isLoading = false;
                }
            });
    }
}
