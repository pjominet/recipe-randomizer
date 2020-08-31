import {Injectable} from '@angular/core';
import {Observable, Subject} from 'rxjs';
import {Alert, AlertType} from '@app/components/alert/alert';
import {filter} from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class AlertService {
    private subject = new Subject<Alert>();
    private defaultId = 'default-alert';

    public onAlert(id = this.defaultId): Observable<Alert> {
        return this.subject.asObservable().pipe(filter(x => x && x.id === id));
    }

    public success(message: string, options?: any) {
        this.alert(new Alert({...options, type: AlertType.Success, message}));
    }

    public error(message: string, options?: any) {
        this.alert(new Alert({...options, type: AlertType.Error, message}));
    }

    public info(message: string, options?: any) {
        this.alert(new Alert({...options, type: AlertType.Info, message}));
    }

    public warn(message: string, options?: any) {
        this.alert(new Alert({...options, type: AlertType.Warning, message}));
    }

    public alert(alert: Alert) {
        alert.id = alert.id || this.defaultId;
        alert.autoClose = (alert.autoClose === undefined ? true : alert.autoClose);
        alert.autoCloseTimeOut = (alert.autoCloseTimeOut === undefined ? 3000 : alert.autoCloseTimeOut);
        this.subject.next(alert);
    }

    public clear(id = this.defaultId) {
        this.subject.next(new Alert({id}));
    }
}
