import {Role} from './user';

export class UpdateRequest {
    username: string;
    email: string;
    role: Role;

    constructor(init?:Partial<UpdateRequest>) {
        Object.assign(this, init);
    }
}
