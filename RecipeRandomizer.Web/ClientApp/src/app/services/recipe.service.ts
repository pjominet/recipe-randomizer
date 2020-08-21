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

    public getRandomRecipe(tagIds: number[]): Observable<Recipe> {
        let url = `${environment.apiUrl}/recipes/random`;
        if (tagIds.length > 0) {
            for (let i = 0; i < tagIds.length; i++) {
                if (i !== 0)
                    url += `&tag=${tagIds[i]}`;
                else
                    url += `?tag=${tagIds[i]}`;
            }
        }

        return this.http.get<Recipe>(url)
            .pipe(map(response => response));
    }
}
