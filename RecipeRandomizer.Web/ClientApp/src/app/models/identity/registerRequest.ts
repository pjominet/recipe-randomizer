export class RegisterRequest {
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
    hasAcceptedTerms: boolean;

    constructor(username: string, email: string, password: string, confirmPassword: string, hasAcceptedTerms: boolean) {
        this.username = username;
        this.email = email;
        this.password = password;
        this.confirmPassword = confirmPassword;
        this.hasAcceptedTerms = hasAcceptedTerms;
    }
}
