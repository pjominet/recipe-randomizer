import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {AuthService} from '@app/services/auth.service';
import {first} from 'rxjs/operators';
import {User} from '@app/models/identity/user';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

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
                public activeModal: NgbActiveModal) {
    }

    ngOnInit() {
        this.registerForm = this.formBuilder.group({
            firstName: ['', Validators.required],
            lastName: ['', Validators.required],
            email: ['', Validators.required],
            password: ['', [Validators.required, Validators.minLength(6)]]
        });
    }

    // convenience getter for easy access to form fields
    get f() {
        return this.registerForm.controls;
    }

    onSubmit() {
        this.isSubmitted = true;

        // stop here if form is invalid
        if (this.registerForm.invalid) {
            return;
        }

        this.isLoading = true;
        this.authService.register(new User(this.f.firstName.value, this.f.lastName.value, this.f.email.value, this.f.password.value))
            .pipe(first())
            .subscribe(
                () => {
                    this.router.navigate(['/dashboard']);
                },
                error => {
                    this.error = error;
                    this.isLoading = false;
                });
    }

}
