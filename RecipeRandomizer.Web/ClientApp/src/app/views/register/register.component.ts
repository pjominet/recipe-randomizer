import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '@app/services/auth.service';
import {first} from 'rxjs/operators';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';
import {RegisterRequest} from '@app/models/identity/registerRequest';
import {MustMatchValidator} from '@app/helpers/must-match.validator';
import {ToastService} from '@app/components/toast/toast.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

    public registerForm: FormGroup;
    public isLoading: boolean = false;
    public isSubmitted: boolean = false;
    public error: string = '';

    constructor(private formBuilder: FormBuilder,
                private route: ActivatedRoute,
                private router: Router,
                private authService: AuthService,
                public activeModal: NgbActiveModal,
                private toastService: ToastService) {
    }

    ngOnInit() {
        this.registerForm = this.formBuilder.group({
            firstName: ['', Validators.required],
            lastName: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', Validators.required],
            acceptTerms: [false, Validators.requiredTrue]
        }, {
            validators: MustMatchValidator('password', 'confirmPassword')
        });
    }

    // convenience getter for easy access to form fields
    get f() {
        return this.registerForm.controls;
    }

    onSubmit() {
        this.isSubmitted = true;

        // stop here if form is invalid
        if (this.registerForm.invalid)
            return;

        this.isLoading = true;
        this.authService.register(new RegisterRequest(
            this.f.firstName.value,
            this.f.lastName.value,
            this.f.email.value,
            this.f.password.value,
            this.f.confirmPassword.value,
            this.f.acceptTerms.value))
            .pipe(first())
            .subscribe(
                response => {
                    this.activeModal.dismiss()
                    this.toastService.toastSuccess(response.message);
                },
                error => {
                    this.error = error;
                    this.isLoading = false;
                });
    }

}
