import {Injectable} from '@angular/core';
import {Observable, Subject} from "rxjs";
import {Toast, ToastType} from "./toast";

@Injectable({
    providedIn: 'root'
})
export class ToastService {

    private toastSubject = new Subject<Toast>();

    constructor() {
    }

    public getToast(): Observable<Toast> {
        return this.toastSubject.asObservable();
    }

    public toast(type: ToastType, header: string, message: string, autohide: boolean, delay: number): void {
        this.toastSubject.next(<Toast>{type: type, message: message, header: header, autohide: autohide, delay: delay});
    }

    public toastSuccess(message: string, header = '', delay = 5000, autohide = true): void {
        this.toast(ToastType.success, header, message, autohide, delay);
    }

    public toastError(message: string, header = '', delay = 5000, autohide = true): void {
        this.toast(ToastType.error, header, message, autohide, delay);
    }

    public toastInfo(message: string, header = '', delay = 5000, autohide = true): void {
        this.toast(ToastType.info, header, message, autohide, delay);
    }

    public toastWarn(message: string, header = '', delay = 5000, autohide = true): void {
        this.toast(ToastType.warning, header, message, autohide, delay);
    }
}
