export class ResetPasswordRequest {
    token: string;
    password: string;
    confirmPassword: string;

    constructor(token: string, password: string, confirmPassword: string) {
        this.token = token;
        this.password = password;
        this.confirmPassword = confirmPassword;
    }
}
