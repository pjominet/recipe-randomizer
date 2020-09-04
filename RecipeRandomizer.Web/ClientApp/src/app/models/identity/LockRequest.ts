export class LockRequest {
    isLocked: boolean;
    lockedById: number;

    constructor(isLocked: boolean, lockedById: number) {
        this.isLocked = isLocked;
        this.lockedById = lockedById;
    }
}
