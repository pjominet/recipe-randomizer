import {Recipe} from "../recipe";

export class User {
    id: number;
    userName: string;
    email: string;
    profileImageUri: string;
    role: Role;
    createdOn: Date;
    updatedOn: Date;
    isVerified: boolean;
    jwtToken?: string;

    recipes: Recipe[];
    likedRecipes: Recipe[];
}

export enum Role {
    admin = 1,
    user = 2
}
