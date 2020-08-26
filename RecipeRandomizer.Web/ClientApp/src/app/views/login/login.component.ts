import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '@app/services/auth.service';
import {AuthRequest} from '@app/models/identity/authRequest';
import {first} from 'rxjs/operators';
import {NgbActiveModal, NgbModal} from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

    public loginForm: FormGroup;
    public isLoading: boolean = false;
    public isSubmitted: boolean = false;
    public error: string = '';

    constructor(private formBuilder: FormBuilder,
                private route: ActivatedRoute,
                private router: Router,
                private authenticationService: AuthService,
                public activeModal: NgbActiveModal) {
        // redirect to user dashboard if already logged in
        if (this.authenticationService.user) {
            this.router.navigate(['/dashboard']);
        }
    }

    public ngOnInit() {
        this.loginForm = this.formBuilder.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', Validators.required]
        });
    }

    // convenience getter for easy access to form fields
    get f() {
        return this.loginForm.controls;
    }

    public onSubmit() {
        this.isSubmitted = true;

        // stop here if form is invalid
        if (this.loginForm.invalid) {
            return;
        }

        this.isLoading = true;
        this.authenticationService.login(new AuthRequest(this.f.email.value, this.f.password.value))
            .pipe(first())
            .subscribe({
                next: () => {
                    this.activeModal.dismiss('login-success');
                },
                error: error => {
                    this.error = error;
                    this.isLoading = false;
                }
            });
    }

    public gotoForgotPassword(): void {
        this.activeModal.dismiss('forgot-password-redirect');
    }
}
