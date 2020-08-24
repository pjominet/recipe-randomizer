import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '@app/services/auth.service';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';
import {first} from 'rxjs/operators';
import {ForgotPasswordRequest} from '@app/models/identity/forgotPasswordRequest';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {

    public forgotPasswordForm: FormGroup;
    public isLoading: boolean = false;
    public isSubmitted: boolean = false;
    public returnUrl: string;
    public error: string = '';

    constructor(private formBuilder: FormBuilder,
                private route: ActivatedRoute,
                private router: Router,
                private authenticationService: AuthService,
                public activeModal: NgbActiveModal) {
    }

    public ngOnInit() {
        this.forgotPasswordForm = this.formBuilder.group({
            email: ['', [Validators.required, Validators.email]]
        });

        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    // convenience getter for easy access to form fields
    get f() {
        return this.forgotPasswordForm.controls;
    }

    public onSubmit() {
        this.isSubmitted = true;

        // stop here if form is invalid
        if (this.forgotPasswordForm.invalid) {
            return;
        }

        this.isLoading = true;
        this.authenticationService.forgotPassword(new ForgotPasswordRequest(this.f.email.value))
            .pipe(first())
            .subscribe({
                next: () => {
                    this.router.navigate([this.returnUrl]);
                },
                error: error => {
                    this.error = error;
                    this.isLoading = false;
                }
            });
    }
}
