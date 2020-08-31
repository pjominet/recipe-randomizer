import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';

import {AuthLayoutComponent} from './auth-layout/auth-layout.component';
import {LoginComponent} from './auth-layout/login/login.component';
import {RegisterComponent} from './auth-layout/register/register.component';
import {VerifyEmailComponent} from './auth-layout/verify-email/verify-email.component';
import {ForgotPasswordComponent} from './auth-layout/forgot-password/forgot-password.component';

const routes: Routes = [
    {
        path: '', component: AuthLayoutComponent,
        children: [
            {path: 'login', component: LoginComponent},
            {path: 'register', component: RegisterComponent},
            {path: 'verify-email', component: VerifyEmailComponent},
            {path: 'forgot-password', component: ForgotPasswordComponent}
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AuthRoutingModule {
}
