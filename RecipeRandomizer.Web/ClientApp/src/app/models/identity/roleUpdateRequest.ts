import {Role} from './user';

export class RoleUpdateRequest {
    role: Role;

    constructor(role: Role) {
        this.role = role;
    }
}
