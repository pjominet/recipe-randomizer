import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '@env/environment';
import {map} from 'rxjs/operators';
import {Recipe} from '@app/models/recipe';
import {AttributionRequest} from '@app/models/attributionRequest';

const recipeApi = `${environment.apiUrl}/recipes`;

@Injectable({
    providedIn: 'root'
})
export class RecipeService {

    constructor(private http: HttpClient) {
    }

    public getRandomRecipe(tagIds: number[] = []): Observable<number> {
        let url = `${recipeApi}/random`;
        if (tagIds.length > 0) {
            url += this.generateTagQueryParams(tagIds);
        }

        return this.http.get<number>(url).pipe(map(response => response));
    }

    public getRecipes(tagIds: number[] = []): Observable<Recipe[]> {
        let url = `${recipeApi}`;
        if (tagIds.length > 0) {
            url += this.generateTagQueryParams(tagIds);
        }

        return this.http.get<Recipe[]>(url).pipe(map(response => response));
    }

    public getPublishedRecipeCount(): Observable<number> {
        return this.http.get<number>(`${recipeApi}/published-count`).pipe(map(response => response));
    }

    public getCreatedRecipesForUser(userId: number): Observable<Recipe[]> {
        return this.http.get<Recipe[]>(`${recipeApi}/created/${userId}`).pipe(map(response => response));
    }

    public getLikedRecipesForUser(userId: number): Observable<Recipe[]> {
        return this.http.get<Recipe[]>(`${recipeApi}/liked/${userId}`).pipe(map(response => response));
    }

    public getRecipe(id: number): Observable<Recipe> {
        return this.http.get<Recipe>(`${recipeApi}/${id}`).pipe(map(response => response));
    }

    public addRecipe(recipe: Recipe): Observable<any> {
        return this.http.post<any>(`${recipeApi}`, recipe, {observe: 'response'});
    }

    public updateRecipe(recipe: Recipe): Observable<any> {
        return this.http.put<any>(`${recipeApi}`, recipe, {observe: 'response'});
    }

    public deleteRecipe(id: number, hard: boolean = false): Observable<any> {
        let url = `${recipeApi}/${id}`;
        if (hard) {
            url += `&hard=${hard}`;
        }

        return this.http.delete<any>(url, {observe: 'response'});
    }

    public restoreRecipe(id: number): Observable<Recipe> {
        return this.http.get<Recipe>(`${recipeApi}/restore/${id}`).pipe(map(response => response));
    }

    public toggleRecipeLike(recipeId: number, userId: number, like: boolean): Observable<any> {
        return this.http.get<any>(`${recipeApi}/${recipeId}/liked-by/${userId}?like=${like}`, {observe: 'response'});
    }

    public getOrphanRecipes(): Observable<Recipe[]> {
        return this.http.get<Recipe[]>(`${recipeApi}/orphans`).pipe(map(response => response));
    }

    public attributeRecipe(attributionRequest: AttributionRequest): Observable<any> {
        return this.http.post<any>(`${recipeApi}/orphans`, attributionRequest, {observe: 'response'});
    }

    private generateTagQueryParams(tagIds: number []): string {
        let queryParams = `?tag=${tagIds[0]}`;
        for (let i = 1; i < tagIds.length; i++) {
            queryParams += `&tag=${tagIds[i]}`;
        }
        return queryParams;
    }
}
