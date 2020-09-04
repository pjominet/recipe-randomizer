﻿import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {environment} from '@env/environment';
import {finalize, map} from 'rxjs/operators';
import {User} from '@app/models/identity/user';
import {UpdateRequest} from '@app/models/identity/updateRequest';
import {AuthService} from './auth.service';
import {LockRequest} from '@app/models/identity/LockRequest';

const usersApi = `${environment.apiUrl}/users`;

@Injectable({
    providedIn: 'root'
})
export class UserService {

    constructor(private router: Router,
                private http: HttpClient,
                private authService: AuthService) {
    }

    public getUsers(): Observable<User[]>{
        return this.http.get<User[]>(usersApi).pipe(map(response => response));
    }

    public getUser(id: number): Observable<User> {
        return this.http.get<User>(`${usersApi}/${id}`).pipe(map(response => response));
    }

    public updateUser(id: number, updateRequest: UpdateRequest): Observable<User> {
        return this.http.put<User>(`${usersApi}/${id}`, updateRequest)
            .pipe(map((updatedUser: User) => {
                // update current user if the logged in user updated their own record
                if (id == this.authService.user.id) {
                    // publish updated user to subscribers
                    this.authService.user = {...this.authService.user, ...updateRequest};
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
        return this.http.post(`${usersApi}/${id}`, lockRequest, {observe: 'response'});
    }
}
