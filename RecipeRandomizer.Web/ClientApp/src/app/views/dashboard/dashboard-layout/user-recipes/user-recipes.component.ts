import {Component, OnInit} from '@angular/core';
import {Recipe} from '@app/models/recipe';
import {RecipeService} from '@app/services/recipe.service';
import {User} from '@app/models/identity/user';
import {AuthService} from '@app/services/auth.service';
import {ActivatedRoute, Router} from '@angular/router';
import {forkJoin} from 'rxjs';
import {AlertService} from '@app/components/alert/alert.service';

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
                private router: Router,
                private alertService: AlertService) {
        this.user = this.authService.user;
    }

    public get createdRecipes(): Recipe[] {
        return this.user.recipes?.filter(r => !r.isDeleted).sort((a, b) => {
            return new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime();
        }) ?? [];
    }

    public get deletedRecipes(): Recipe[] {
        return this.user.recipes?.filter(r => r.isDeleted).sort((a, b) => {
            return new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime();
        }) ?? [];
    }

    public get likedRecipes(): Recipe[] {
        return this.user.likedRecipes.sort((a, b) => {
            return new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime();
        }) ?? [];
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

    public showRecipe(id: number, event: any): void {
        if (event.target.tagName.toLowerCase() === 'td') {
            const queryParams = this.route.snapshot.queryParams;
            this.router.navigate([id], {relativeTo: this.route, queryParams: queryParams});
        }
    }

    public markAsDeleted(id: number): void {
        this.alertService.clear();
        this.recipeService.deleteRecipe(id).subscribe(
            () => {
                this.user.recipes[this.user.recipes.findIndex(r => r.id === id)].isDeleted = true;
                this.alertService.success('Successfully marked recipe for deletion.');
            }, error => {
                this.alertService.error('Could not mark recipe for deletion.');
            });
    }

    public restore(id: number): void {
        this.alertService.clear();
        this.recipeService.restoreRecipe(id).subscribe(
            recipe => {
                this.user.recipes[this.user.recipes.findIndex(r => r.id === id)].isDeleted = false;
                this.alertService.success('Successfully restored recipe.');
            }, error => {
                this.alertService.error('Could not restore recipe.');
            });
    }

    public hardDelete(id: number): void {
        this.alertService.clear();
        this.recipeService.deleteRecipe(id, true).subscribe(
            () => {
                this.user.recipes = this.user.recipes.filter(r => r.id === id);
                this.alertService.success('Successfully deleted recipe. It is gone for good!');
            }, error => {
                this.alertService.error('Could not delete recipe.');
            });
    }
}
