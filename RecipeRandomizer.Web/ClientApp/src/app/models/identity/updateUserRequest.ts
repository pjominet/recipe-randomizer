import {User} from './user';

export class UpdateUserRequest {
    username: string;
    email: string;
    isLockedOut: boolean;
    lockedOutBy: User;

    constructor(init?:Partial<UpdateUserRequest>) {
        Object.assign(this, init);
    }
}
