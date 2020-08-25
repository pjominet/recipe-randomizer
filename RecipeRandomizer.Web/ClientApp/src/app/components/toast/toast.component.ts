import {Component, OnInit} from '@angular/core';
import {Toast, ToastType} from "./toast";
import {ToastService} from "./toast.service";

@Component({
    selector: 'app-toast',
    templateUrl: './toast.component.html',
    styleUrls: ['./toast.component.scss'],
    host: {'[class.ngb-toasts]': 'true'}
})
export class ToastComponent implements OnInit {

    public toasts: Toast[] = [];

    constructor(private toastService: ToastService) {
    }

    ngOnInit() {
        this.toastService.getToast().subscribe((toast: Toast) => {
            this.toasts.push(toast);
            // launch auto removal if autohide is set true
            if (toast.autohide)
                setTimeout(() => this.removeToast(toast), toast.delay);
        });
    }

    public removeToast(toast: Toast) {
        this.toasts = this.toasts.filter(t => t !== toast);
    }

    public getStateColor(toast: Toast): string {
        if (!toast) return;

        switch (toast.type) {
            case ToastType.success:
                return 'bg-success';
            case ToastType.error:
                return 'bg-danger';
            case ToastType.info:
                return 'bg-info';
            case ToastType.warning:
                return 'bg-warning';
        }
    }

    public getStateIcon(toast: Toast): string {
        if (!toast) return;

        switch (toast.type) {
            case ToastType.success:
                return 'fas fa-check-circle text-success';
            case ToastType.error:
                return 'fas fa-times-circle text-danger';
            case ToastType.info:
                return 'fas fa-info-circle text-info';
            case ToastType.warning:
                return 'fas fa-exclamation-triangle text-warning';
        }
    }
}
