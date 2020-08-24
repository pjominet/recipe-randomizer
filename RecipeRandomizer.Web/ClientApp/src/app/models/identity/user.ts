import {Recipe} from "../recipe";

export class User {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    role: Role;
    createdOn: Date;
    updatedOn: Date;
    isVerified: boolean;
    jwtToken?: string;

    recipes: Recipe[];
}

export enum Role {
    admin = 1,
    user = 2
}
