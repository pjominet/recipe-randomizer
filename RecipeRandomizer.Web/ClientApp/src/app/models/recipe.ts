import {Cost} from '@app/models/nomenclature/cost';
import {Difficulty} from '@app/models/nomenclature/difficulty';
import {Ingredient} from '@app/models/ingredient';
import {Tag} from '@app/models/nomenclature/tag';
import {User} from '@app/models/identity/user';

export class Recipe {
    id: number;
    userId: number;
    name: string;
    description: string;
    imageUri: string;
    numberOfPeople: number;
    cost: Cost;
    difficulty: Difficulty;
    prepTime: number;
    cookTime: number;
    createdOn: Date;
    lastUpdatedOn: Date;
    deletedOn: Date;
    isDeleted: boolean;

    ingredients: Ingredient[];
    tags: Tag[]
    user: User;
}
