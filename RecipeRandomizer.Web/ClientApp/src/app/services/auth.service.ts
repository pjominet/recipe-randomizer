import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {environment} from '@env/environment';
import {map} from 'rxjs/operators';
import {User} from '@app/models/identity/user';
import {AuthRequest} from '@app/models/identity/authRequest';

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

    public register(user: User): Observable<User> {
        return this.http.post<User>(`${environment.apiUrl}/users/register`, user)
            .pipe(map(registeredUser => {
                this.userSubject.next(registeredUser);
                this.startRefreshTokenTimer();
                return registeredUser;
            }));
    }

    public login(authRequest: AuthRequest): Observable<User> {
        return this.http.post<User>(`${environment.apiUrl}/users/authenticate`, authRequest, {withCredentials: true})
            .pipe(map(user => {
                this.userSubject.next(user);
                this.startRefreshTokenTimer();
                return user;
            }));
    }

    public logout(): void {
        this.http.post<any>(`${environment.apiUrl}/users/revoke-token`, {}, {withCredentials: true}).subscribe();
        this.stopRefreshTokenTimer();
        this.userSubject.next(null);
        this.router.navigate(['/login']);
    }

    public refreshToken(): Observable<User> {
        return this.http.post<User>(`${environment.apiUrl}/users/refresh-token`, {}, {withCredentials: true})
            .pipe(map((user) => {
                this.userSubject.next(user);
                this.startRefreshTokenTimer();
                return user;
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
