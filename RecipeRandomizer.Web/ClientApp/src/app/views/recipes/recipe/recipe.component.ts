import {Component, Input, OnInit} from '@angular/core';
import {RecipeService} from '@app/services/recipe.service';
import {Recipe} from '@app/models/recipe';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';
import {Cost} from '@app/models/nomenclature/cost';
import {Difficulty} from '@app/models/nomenclature/difficulty';
import {AuthService} from '@app/services/auth.service';
import {User} from '@app/models/identity/user';
import {environment} from '@env/environment';
import {LikeRequest} from "@app/models/LikeRequest";

@Component({
    selector: 'app-recipe',
    templateUrl: './recipe.component.html',
    styleUrls: ['./recipe.component.scss']
})
export class RecipeComponent implements OnInit {

    @Input() public id: number;
    public recipe: Recipe;
    public cost: typeof Cost = Cost;
    public difficulties: typeof Difficulty = Difficulty;

    public user: User;
    public isLiked: boolean = false;

    constructor(private recipeService: RecipeService,
                public activeModal: NgbActiveModal,
                private authService: AuthService) {
        this.user = this.authService.user;
    }

    public get recipeImage(): string {
        return this.recipe.imageUri
            ? `${environment.staticFileUrl}/${this.recipe.imageUri}`
            : 'assets/img/recipe_placeholder.jpg';
    }

    public ngOnInit(): void {
        this.recipeService.getRecipe(this.id).subscribe(
            recipe => {
                this.recipe = recipe;
                this.isLiked = this.recipe.likes.findIndex(l => l === this.user.id) !== -1;
            });
    }

    public like(): void {
        this.isLiked = !this.isLiked;
        this.recipeService.toggleRecipeLike(this.recipe.id, new LikeRequest(this.isLiked, this.user.id)).subscribe(
            () => {
                if (this.isLiked) {
                    this.recipe.likes.push(this.user.id);
                } else {
                    this.recipe.likes = this.recipe.likes.filter(l => l === this.user.id);
                }
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
            color = 'primary-dark';
        }
        return color;
    }
}
