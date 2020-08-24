import {Role} from './user';

export class UpdateUserRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
    role: Role;
}
