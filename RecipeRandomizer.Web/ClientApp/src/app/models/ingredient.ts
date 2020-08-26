import {QuantityUnit} from './nomenclature/quantityUnit';
import {Recipe} from './recipe';

export class Ingredient {
    id: number;
    quantityUnitId: number;
    recipeId: number;
    name: string;
    quantity: number;

    quantityUnit: QuantityUnit;
    recipe: Recipe;
}
