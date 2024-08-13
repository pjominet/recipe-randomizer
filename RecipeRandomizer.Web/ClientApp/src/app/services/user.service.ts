import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {environment} from '@env/environment';
import {finalize, map} from 'rxjs/operators';
import {IUser} from '@app/models/identity/user';
import {AuthService} from './auth.service';
import {LockRequest} from '@app/models/identity/LockRequest';
import {UserUpdateRequest} from '@app/models/identity/userUpdateRequest';
import {RoleUpdateRequest} from '../models/identity/roleUpdateRequest';

const usersApi = `${environment.apiUrl}/users`;

@Injectable({
    providedIn: 'root'
})
export class UserService {

    constructor(private router: Router,
                private http: HttpClient,
                private authService: AuthService) {
    }

    public getUsers(): Observable<IUser[]>{
        return this.http.get<IUser[]>(usersApi).pipe(map(response => response));
    }

    public getUser(id: number): Observable<IUser> {
        return this.http.get<IUser>(`${usersApi}/${id}`).pipe(map(response => response));
    }

    public updateUser(id: number, userUpdateRequest: UserUpdateRequest): Observable<IUser> {
        return this.http.put<IUser>(`${usersApi}/${id}`, userUpdateRequest)
            .pipe(map((updatedUser: IUser) => {
                // update current user if the logged in user updated their own record
                if (id == this.authService.user.id) {
                    // publish updated user to subscribers
                    this.authService.user = {...this.authService.user, ...userUpdateRequest};
                }
                return updatedUser;
            }));
    }

    public updateUserRole(id: number, roleUpdateRequest: RoleUpdateRequest): Observable<IUser> {
        return this.http.put<any>(`${usersApi}/${id}/role`, roleUpdateRequest)
            .pipe(map((updatedUser: IUser) => {
                // update current user role if the logged in user updated their own record
                if (id == this.authService.user.id) {
                    // publish updated user to subscribers
                    this.authService.user = {...this.authService.user, ...roleUpdateRequest};
                }
                return updatedUser;
            }));
    }

    public deleteUser(id: number): Observable<any> {
        return this.http.delete(`${usersApi}/${id}`)
            .pipe(finalize(() => {
                // auto logout if the logged in user deleted their own record
                if (id === this.authService.user.id) {
                    this.authService.logout();
                }
            }));
    }

    public toggleUserLock(id: number, lockRequest: LockRequest): Observable<any> {
        return this.http.post(`${usersApi}/lock/${id}`, lockRequest, {observe: 'response'});
    }
}
