import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ReactiveFormsModule} from '@angular/forms';
import {AuthRoutingModule} from './auth.routing';

import {AuthLayoutComponent} from './auth-layout/auth-layout.component';
import {VerifyEmailComponent} from './auth-layout/verify-email/verify-email.component';
import {LoginComponent} from './auth-layout/login/login.component';
import {RegisterComponent} from './auth-layout/register/register.component';
import {ForgotPasswordComponent} from './auth-layout/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './auth-layout/reset-password/reset-password.component';


@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        AuthRoutingModule
    ],
    declarations: [
        AuthLayoutComponent,
        VerifyEmailComponent,
        LoginComponent,
        RegisterComponent,
        ForgotPasswordComponent,
        ResetPasswordComponent,
    ],
})
export class AuthModule {
}
