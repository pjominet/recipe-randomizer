import {Injectable} from '@angular/core';
import {Observable, Subject} from 'rxjs';
import {IUser} from '@app/models/identity/user';


@Injectable({
    providedIn: 'root'
})
export class ChangeRoleService {
    private changeSubject: Subject<IUser> = new Subject<IUser>();

    public onChange(): Observable<IUser> {
        return this.changeSubject.asObservable();
    }

    public changeSuccess(user: IUser) {
        this.changeSubject.next(user);
    }
}
