export class RegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
    hasAcceptedTerms: boolean;

    constructor(firstName: string, lastName: string, email: string, password: string, confirmPassword: string, hasAcceptedTerms: boolean) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.password = password;
        this.confirmPassword = confirmPassword;
        this.hasAcceptedTerms = hasAcceptedTerms;
    }
}
