import {Recipe} from "../recipe";

export interface IUser {
    id: number;
    username: string;
    email: string;
    profileImageUri: string;
    role: Role;
    createdOn: Date;
    updatedOn: Date;
    isVerified: boolean;
    jwtToken?: string;
    isLocked: boolean;
    lockedBy?: User;
    lockedOn: Date;

    recipes: Recipe[];
    likedRecipes: Recipe[];
}

export class User implements IUser {
    id: number;
    username: string;
    email: string;
    profileImageUri: string;
    role: Role;
    createdOn: Date;
    updatedOn: Date;
    isVerified: boolean;
    jwtToken?: string;
    isLocked: boolean;
    lockedBy?: User;
    lockedOn: Date;

    recipes: Recipe[];
    likedRecipes: Recipe[];
}

export enum Role {
    admin = 1,
    user = 2
}
