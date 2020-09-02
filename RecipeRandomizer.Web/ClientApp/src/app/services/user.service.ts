import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {environment} from '@env/environment';
import {finalize, map} from 'rxjs/operators';
import {User} from '@app/models/identity/user';
import {UpdateUserRequest} from '@app/models/identity/updateUserRequest';
import {AuthService} from './auth.service';

const apiUrl = `${environment.apiUrl}/users`;

@Injectable({
    providedIn: 'root'
})
export class UserService {

    constructor(private router: Router,
                private http: HttpClient,
                private authService: AuthService) {
    }

    public getUsers(): Observable<User[]>{
        return this.http.get<User[]>(apiUrl).pipe(map(response => response));
    }

    public getUser(id: number): Observable<User> {
        return this.http.get<User>(`${apiUrl}/${id}`).pipe(map(response => response));
    }

    public updateUser(id: number, updateUserRequest: UpdateUserRequest): Observable<User> {
        return this.http.put<User>(`${apiUrl}/${id}`, updateUserRequest)
            .pipe(map((updatedUser: User) => {
                // update current user if the logged in user updated their own record
                if (id == this.authService.user.id) {
                    // publish updated user to subscribers
                    this.authService.user = {...this.authService.user, ...updateUserRequest};
                }
                return updatedUser;
            }));
    }

    public deleteUser(id: number): void {
        this.http.delete(`${apiUrl}/${id}`)
            .pipe(finalize(() => {
                // auto logout if the logged in user deleted their own record
                if (id === this.authService.user.id) {
                    this.authService.logout();
                }
            }));
    }
}
