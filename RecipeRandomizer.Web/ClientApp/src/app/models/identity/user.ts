import {Recipe} from "../recipe";

export class User {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    jwtToken?: string;

    recipes: Recipe[];
}
