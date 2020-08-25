export class Toast {
    type: ToastType;
    header: string;
    message: string;
    autohide: boolean;
    delay: number;
}

export enum ToastType {
    success,
    error,
    info,
    warning
}
