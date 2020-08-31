export class RegisterRequest {
    userName: string;
    email: string;
    password: string;
    confirmPassword: string;
    hasAcceptedTerms: boolean;

    constructor(userName: string, email: string, password: string, confirmPassword: string, hasAcceptedTerms: boolean) {
        this.userName = userName;
        this.email = email;
        this.password = password;
        this.confirmPassword = confirmPassword;
        this.hasAcceptedTerms = hasAcceptedTerms;
    }
}
