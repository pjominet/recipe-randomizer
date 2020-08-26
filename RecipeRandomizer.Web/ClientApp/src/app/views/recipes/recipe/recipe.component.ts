import {Component, Input, OnInit} from '@angular/core';
import {RecipeService} from '@app/services/recipe.service';
import {Recipe} from '@app/models/recipe';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';
import {Cost} from '@app/models/nomenclature/cost';
import {Difficulty} from '@app/models/nomenclature/difficulty';

@Component({
    selector: 'app-recipe',
    templateUrl: './recipe.component.html',
    styleUrls: ['./recipe.component.scss']
})
export class RecipeComponent implements OnInit {

    @Input() public id: number;
    public recipe: Recipe;
    public cost: typeof Cost = Cost;
    public difficulty: typeof Difficulty = Difficulty;

    constructor(private recipeService: RecipeService,
                public activeModal: NgbActiveModal) {
    }

    ngOnInit(): void {
        this.recipeService.getRecipe(this.id).subscribe(
            recipe => {
                this.recipe = recipe;
            }, error => {
                console.log(error);
            }
        );
    }
}
