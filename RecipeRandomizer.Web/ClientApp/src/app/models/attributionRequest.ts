export class AttributionRequest {
    userId: number;
    recipeId: number;

    constructor(userId: number, recipeId: number) {
        this.userId = userId;
        this.recipeId = recipeId;
    }
}
