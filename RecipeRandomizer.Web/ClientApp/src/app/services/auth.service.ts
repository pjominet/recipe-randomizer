import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {environment} from '@env/environment';
import {map} from 'rxjs/operators';
import {User} from '@app/models/identity/user';
import {AuthRequest} from '@app/models/identity/authRequest';
import {ResetPasswordRequest} from '@app/models/identity/resetPasswordRequest';
import {ForgotPasswordRequest} from '@app/models/identity/forgotPasswordRequest';
import {UpdateUserRequest} from '@app/models/identity/updateUserRequest';
import {RegisterRequest} from '@app/models/identity/registerRequest';

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

    public register(registerRequest: RegisterRequest): Observable<any> {
        return this.http.post<User>(`${environment.apiUrl}/users/register`, registerRequest).pipe(response => response);
    }

    public login(authRequest: AuthRequest): Observable<User> {
        return this.http.post<User>(`${environment.apiUrl}/users/authenticate`, authRequest, {withCredentials: true})
            .pipe(map(user => {
                this.userSubject.next(user);
                this.startRefreshTokenTimer();
                return user;
            }));
    }

    public forgotPassword(forgotPasswordRequest: ForgotPasswordRequest): Observable<any> {
        return this.http.post<any>(`${environment.apiUrl}/users/forgot-password`, forgotPasswordRequest).pipe(response => response);
    }

    public resetPassword(resetPasswordRequest: ResetPasswordRequest): Observable<any> {
        return this.http.post<any>(`${environment.apiUrl}/users/reset-password`, resetPasswordRequest).pipe(response => response);
    }

    public logout(): void {
        this.http.post<any>(`${environment.apiUrl}/users/revoke-token`, {}, {withCredentials: true}).subscribe();
        this.stopRefreshTokenTimer();
        this.userSubject.next(null);
        this.router.navigate(['/']);
    }

    public refreshToken(): Observable<User> {
        return this.http.post<User>(`${environment.apiUrl}/users/refresh-token`, {}, {withCredentials: true})
            .pipe(map((user) => {
                this.userSubject.next(user);
                this.startRefreshTokenTimer();
                return user;
            }));
    }

    public updateUser(id: number, updateUserRequest: UpdateUserRequest) : Observable<User> {
        return this.http.put<User>(`${environment.apiUrl}/users/${id}`, updateUserRequest)
            .pipe(map(updatedUser => {
                // update current user if the logged in user updated their own record
                if (id == this.user.id) {
                    // publish updated user to subscribers
                    const user = {...this.user, ...updateUserRequest};
                    this.userSubject.next(user);
                }
                return updatedUser;
            }));
    }

    public deleteUser(id: number): void {
        this.http.delete(`${environment.apiUrl}/users/${id}`)
            .pipe(map(() => {
                // auto logout if the logged in user deleted their own record
                if (id == this.user.id) {
                    this.logout();
                }
            }));
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
