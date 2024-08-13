export class LockRequest {
    userLock: boolean;
    lockedById?: number;

    constructor(userLock: boolean, lockedById?: number) {
        this.userLock = userLock;
        this.lockedById = lockedById;
    }
}
