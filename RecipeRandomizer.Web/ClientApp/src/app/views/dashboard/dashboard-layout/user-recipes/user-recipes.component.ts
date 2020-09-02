import {Component, OnInit} from '@angular/core';
import {Recipe} from '@app/models/recipe';
import {RecipeService} from '@app/services/recipe.service';
import {User} from '@app/models/identity/user';
import {AuthService} from '@app/services/auth.service';
import {ActivatedRoute, Router} from '@angular/router';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-user-recipes',
    templateUrl: './user-recipes.component.html',
    styleUrls: ['./user-recipes.component.scss']
})
export class UserRecipesComponent implements OnInit {

    public user: User;
    public activeTab: number = 1;

    constructor(private recipeService: RecipeService,
                private authService: AuthService,
                private route: ActivatedRoute,
                private router: Router) {
        this.user = this.authService.user;
    }

    public get createdRecipes(): Recipe[] {
        return this.user.recipes?.filter(r => !r.isDeleted) ?? [];
    }

    public get deletedRecipes(): Recipe[] {
        return this.user.recipes?.filter(r => r.isDeleted) ?? [];
    }

    public get likedRecipes(): Recipe[] {
        return this.user.likedRecipes ?? [];
    }

    public ngOnInit(): void {
        if (this.route.snapshot.queryParams['liked']) {
            this.activeTab = 3;
        } else if (this.route.snapshot.queryParams['deleted']) {
            this.activeTab = 2;
        } else {
            this.activeTab = 1;
        }

        forkJoin([
            this.recipeService.getCreatedRecipesForUser(this.user.id),
            this.recipeService.getLikedRecipesForUser(this.user.id)
        ]).subscribe(
            ([createdRecipes, likedRecipes]) => {
                this.user.recipes = createdRecipes;
                this.user.likedRecipes = likedRecipes;
            });
    }

    public onTabChange(activeTab: number): void {
        let queryParams: {};
        switch (activeTab) {
            case 1:
                queryParams = {};
                break;
            case 2:
                queryParams = {deleted: true};
                break;
            case 3:
                queryParams = {liked: true};
                break;
            default:
                queryParams = {};
        }
        this.router.navigate([], {relativeTo: this.route, replaceUrl: true, queryParams: queryParams});
    }

    public markAsDeleted(id: number): void {

    }

    public restore(id: number): void {

    }

    public hardDelete(id: number): void {

    }
}
