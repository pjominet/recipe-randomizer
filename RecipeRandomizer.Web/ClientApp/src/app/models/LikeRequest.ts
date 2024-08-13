export class LikeRequest {
    like: boolean;
    likedById: number;

    constructor(like: boolean, likedById: number) {
        this.like = like;
        this.likedById = likedById;
    }
}
