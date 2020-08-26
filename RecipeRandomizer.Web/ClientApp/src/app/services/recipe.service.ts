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

    public getRandomRecipe(tagIds: number[] = []): Observable<Recipe> {
        let url = `${environment.apiUrl}/recipes/random`;
        if (tagIds.length > 0) {
            url += this.generateTagQueryParams(tagIds);
        }

        return this.http.get<Recipe>(url).pipe(map(response => response));
    }

    public getRecipes(tagIds: number[] = [], userId?: number): Observable<Recipe[]> {
        let url = `${environment.apiUrl}/recipes`;
        if (tagIds.length > 0) {
            url += this.generateTagQueryParams(tagIds);
        }
        if (!!userId) {
            if (tagIds.length > 0) {
                url += '?';
            } else {
                url += '&';
            }
            url += `user=${userId}`;
        }
        return this.http.get<Recipe[]>(url).pipe(map(response => response));
    }

    public getRecipe(id: number): Observable<Recipe> {
        let url = `${environment.apiUrl}/recipes/${id}`;

        return this.http.get<Recipe>(url).pipe(map(response => response));
    }

    private generateTagQueryParams(tagIds: number []): string {
        let queryParams = `?tag=${tagIds[0]}`;
        for (let i = 1; i < tagIds.length; i++) {
            queryParams += `&tag=${tagIds[i]}`;
        }
        return queryParams;
    }
}
