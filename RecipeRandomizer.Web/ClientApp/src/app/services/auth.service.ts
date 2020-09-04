import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {environment} from '@env/environment';
import {map} from 'rxjs/operators';
import {IUser} from '@app/models/identity/user';
import {AuthRequest} from '@app/models/identity/authRequest';
import {ResetPasswordRequest} from '@app/models/identity/resetPasswordRequest';
import {VerificationRequest} from '@app/models/identity/verificationRequest';
import {RegisterRequest} from '@app/models/identity/registerRequest';
import {ValidationRequest} from '@app/models/identity/validationRequest';
import {ChangePasswordRequest} from '@app/models/identity/changePasswordRequest';

const authApi = `${environment.apiUrl}/auth`;

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private userSubject: BehaviorSubject<IUser>;
    public $user: Observable<IUser>;
    private refreshTokenTimeout;

    constructor(private router: Router,
                private http: HttpClient) {
        this.userSubject = new BehaviorSubject<IUser>(null);
        this.$user = this.userSubject.asObservable();
    }

    public get user(): IUser {
        return this.userSubject.value;
    }

    public set user(user: IUser) {
        this.userSubject.next(user);
    }

    public register(registerRequest: RegisterRequest): Observable<any> {
        return this.http.post<any>(`${authApi}/register`, registerRequest);
    }

    public login(authRequest: AuthRequest): Observable<IUser> {
        return this.http.post<IUser>(`${authApi}/authenticate`, authRequest, {withCredentials: true})
            .pipe(map((user: IUser) => {
                this.userSubject.next(user);
                this.startRefreshTokenTimer();
                return user;
            }));
    }

    public refreshToken(): Observable<IUser> {
        return this.http.post<IUser>(`${authApi}/refresh-token`, {}, {withCredentials: true})
            .pipe(map((user: IUser) => {
                if (user) {
                    this.userSubject.next(user);
                    this.startRefreshTokenTimer();
                }
                return user;
            }));
    }

    public logout(): void {
        this.http.post<any>(`${authApi}/revoke-token`, {}, {withCredentials: true}).subscribe(
            () => {
                this.stopRefreshTokenTimer();
                this.userSubject.next(null);
                this.router.navigate(['/']);
            }, error => {
                console.log(error);
            }
        );
    }

    public verifyEmail(validationRequest: ValidationRequest): Observable<any> {
        return this.http.post(`${authApi}/verify-email`, validationRequest);
    }

    public resendEmailVerificationCode(verificationRequest: VerificationRequest): Observable<any> {
        return this.http.post(`${authApi}/resend-email-verification`, verificationRequest);
    }

    public validateResetToken(validationRequest: ValidationRequest): Observable<any> {
        return this.http.post(`${authApi}/validate-reset-token`, validationRequest);
    }

    public forgotPassword(verificationRequest: VerificationRequest): Observable<any> {
        return this.http.post<any>(`${authApi}/forgot-password`, verificationRequest);
    }

    public resetPassword(resetPasswordRequest: ResetPasswordRequest): Observable<any> {
        return this.http.post<any>(`${authApi}/reset-password`, resetPasswordRequest);
    }

    public changePassword(changePasswordRequest: ChangePasswordRequest): Observable<any> {
        return this.http.post<any>(`${authApi}/change-password`, changePasswordRequest);
    }

    private startRefreshTokenTimer() {
        // parse json object from base64 encoded jwt token
        const jwtToken = JSON.parse(atob(this.user.jwtToken.split('.')[1]));

        // set a timeout to refresh the token a minute before it expires
        const expires = new Date(jwtToken.exp * 1000);
        const timeout = expires.getTime() - Date.now() - (60 * 1000);
        this.refreshTokenTimeout = setTimeout(() => this.refreshToken().subscribe(), timeout);
    }

    private stopRefreshTokenTimer() {
        clearTimeout(this.refreshTokenTimeout);
    }
}
