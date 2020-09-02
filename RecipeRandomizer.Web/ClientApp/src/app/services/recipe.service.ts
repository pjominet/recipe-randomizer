import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '@env/environment';
import {map} from 'rxjs/operators';
import {Recipe} from '@app/models/recipe';

@Injectable({
    providedIn: 'root'
})
export class RecipeService {

    constructor(private http: HttpClient) {
    }

    public getRandomRecipe(tagIds: number[] = []): Observable<number> {
        let url = `${environment.apiUrl}/recipes/random`;
        if (tagIds.length > 0) {
            url += this.generateTagQueryParams(tagIds);
        }

        return this.http.get<number>(url).pipe(map(response => response));
    }

    public getRecipes(tagIds: number[] = []): Observable<Recipe[]> {
        let url = `${environment.apiUrl}/recipes`;
        if (tagIds.length > 0) {
            url += this.generateTagQueryParams(tagIds);
        }

        return this.http.get<Recipe[]>(url).pipe(map(response => response));
    }

    public getCreatedRecipesForUser(userId: number): Observable<Recipe[]> {
        return this.http.get<Recipe[]>(`${environment.apiUrl}/recipes/created/${userId}`).pipe(map(response => response));
    }

    public getLikedRecipesForUser(userId: number): Observable<Recipe[]> {
        return this.http.get<Recipe[]>(`${environment.apiUrl}/recipes/liked/${userId}`).pipe(map(response => response));
    }

    public getRecipe(id: number): Observable<Recipe> {
        return this.http.get<Recipe>(`${environment.apiUrl}/recipes/${id}`).pipe(map(response => response));
    }

    public addRecipe(recipe: Recipe): Observable<any> {
        return this.http.post<any>(`${environment.apiUrl}/recipes`, recipe, {observe: 'response'});
    }

    public updateRecipe(recipe: Recipe): Observable<any> {
        return this.http.put<any>(`${environment.apiUrl}/recipes`, recipe, {observe: 'response'});
    }

    public deleteRecipe(id: number, hard: boolean = false): Observable<any> {
        let url = `${environment.apiUrl}/recipes/${id}`;
        if (hard) {
            url += `&hard=${hard}`;
        }

        return this.http.delete<any>(url, {observe: 'response'});
    }

    public restoreRecipe(id: number): Observable<Recipe> {
        return this.http.get<Recipe>(`${environment.apiUrl}/recipes/restore/${id}`).pipe(map(response => response));
    }

    private generateTagQueryParams(tagIds: number []): string {
        let queryParams = `?tag=${tagIds[0]}`;
        for (let i = 1; i < tagIds.length; i++) {
            queryParams += `&tag=${tagIds[i]}`;
        }
        return queryParams;
    }
}
