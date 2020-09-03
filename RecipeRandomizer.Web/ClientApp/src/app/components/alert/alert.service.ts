import {Injectable} from '@angular/core';
import {Observable, Subject} from 'rxjs';
import {Alert, AlertType} from '@app/components/alert/alert';
import {filter} from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class AlertService {
    private alertSubject: Subject<Alert> = new Subject<Alert>();

    public onAlert(): Observable<Alert> {
        return this.alertSubject.asObservable().pipe(filter(x => !!x));
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
        alert.autoClose = (alert.autoClose === undefined ? true : alert.autoClose);
        alert.autoCloseTimeOut = (alert.autoCloseTimeOut === undefined ? 3000 : alert.autoCloseTimeOut);
        this.alertSubject.next(alert);
    }

    public clear() {
        this.alertSubject.next();
    }
}
