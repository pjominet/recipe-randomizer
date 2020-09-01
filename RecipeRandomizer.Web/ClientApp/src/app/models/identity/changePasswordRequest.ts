export class ChangePasswordRequest {
    password: string;
    newPassword: string;
    confirmNewPassword: string;

    constructor(password: string, newPassword: string, confirmNewPassword: string) {
        this.password = password;
        this.newPassword = newPassword;
        this.confirmNewPassword = confirmNewPassword;
    }
}
