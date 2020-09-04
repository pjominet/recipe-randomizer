import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {environment} from '@env/environment';
import {finalize, map} from 'rxjs/operators';
import {User} from '@app/models/identity/user';
import {AuthRequest} from '@app/models/identity/authRequest';
import {ResetPasswordRequest} from '@app/models/identity/resetPasswordRequest';
import {VerificationRequest} from '@app/models/identity/verificationRequest';
import {UpdateUserRequest} from '@app/models/identity/updateUserRequest';
import {RegisterRequest} from '@app/models/identity/registerRequest';
import {ValidationRequest} from '@app/models/identity/validationRequest';
import {ChangePasswordRequest} from '@app/models/identity/changePasswordRequest';

const apiUrl = `${environment.apiUrl}/auth`;

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private userSubject: BehaviorSubject<User>;
    public $user: Observable<User>;
    private refreshTokenTimeout;

    constructor(private router: Router,
                private http: HttpClient) {
        this.userSubject = new BehaviorSubject<User>(null);
        this.$user = this.userSubject.asObservable();
    }

    public get user(): User {
        return this.userSubject.value;
    }

    public set user(user: User) {
        this.userSubject.next(user);
    }

    public register(registerRequest: RegisterRequest): Observable<any> {
        return this.http.post<any>(`${apiUrl}/register`, registerRequest);
    }

    public login(authRequest: AuthRequest): Observable<User> {
        return this.http.post<User>(`${apiUrl}/authenticate`, authRequest, {withCredentials: true})
            .pipe(map((user: User) => {
                this.userSubject.next(user);
                this.startRefreshTokenTimer();
                return user;
            }));
    }

    public refreshToken(): Observable<User> {
        return this.http.post<User>(`${apiUrl}/refresh-token`, {}, {withCredentials: true})
            .pipe(map((user: User) => {
                if (user) {
                    this.userSubject.next(user);
                    this.startRefreshTokenTimer();
                }
                return user;
            }));
    }

    public logout(): void {
        this.http.post<any>(`${apiUrl}/revoke-token`, {}, {withCredentials: true}).subscribe();
        this.stopRefreshTokenTimer();
        this.userSubject.next(null);
        this.router.navigate(['/']);
    }

    public verifyEmail(validationRequest: ValidationRequest): Observable<any> {
        return this.http.post(`${apiUrl}/verify-email`, validationRequest);
    }

    public resendEmailVerificationCode(verificationRequest: VerificationRequest): Observable<any> {
        return this.http.post(`${apiUrl}/resend-email-verification`, verificationRequest);
    }

    public validateResetToken(validationRequest: ValidationRequest): Observable<any> {
        return this.http.post(`${apiUrl}/validate-reset-token`, validationRequest);
    }

    public forgotPassword(verificationRequest: VerificationRequest): Observable<any> {
        return this.http.post<any>(`${apiUrl}/forgot-password`, verificationRequest);
    }

    public resetPassword(resetPasswordRequest: ResetPasswordRequest): Observable<any> {
        return this.http.post<any>(`${apiUrl}/reset-password`, resetPasswordRequest);
    }

    public changePassword(changePasswordRequest: ChangePasswordRequest): Observable<any> {
        return this.http.post<any>(`${apiUrl}/change-password`, changePasswordRequest);
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
