import {Cost} from '@app/models/nomenclature/cost';
import {Difficulty} from '@app/models/nomenclature/difficulty';
import {Ingredient} from '@app/models/ingredient';
import {Tag} from '@app/models/nomenclature/tag';

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
    preparation: string;
    createdOn: Date;
    updatedOn: Date;
    deletedOn: Date;
    isDeleted: boolean;

    ingredients: Ingredient[];
    tags: Tag[]
    createdBy: string;
    likes: number;
}
