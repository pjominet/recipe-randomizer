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

    public ngOnInit(): void {
        this.recipeService.getRecipe(this.id).subscribe(
            recipe => {
                this.recipe = recipe;
            }, error => {
                console.log(error);
            }
        );
    }

    public costColor(cost: Cost): string {
        let color: string;
        switch (cost) {
            case Cost.Cheap:
                color = 'success';
                break;
            case Cost.Average:
                color = 'warning';
                break;
            case Cost.Expensive:
                color = 'danger';
                break;
            default:
                color = 'success';
        }

        return color;
    }

    public difficultyColor(difficulty: Difficulty): string {
        let color: string;
        switch (difficulty) {
            case Difficulty.Easy:
                color = 'success';
                break;
            case Difficulty.Average:
                color = 'warning';
                break;
            case Difficulty.Hard:
                color = 'danger';
                break;
            default:
                color = 'success';
        }

        return color;
    }

    public timeColor(time: number): string {
        let color = 'primary-light';
        if (time > 30 && time <= 90) {
            color = 'primary';
        } else if (time > 90) {
            color = 'primary-dark'
        }
        return color;
    }
}
