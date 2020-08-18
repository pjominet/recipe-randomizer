import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";


@Injectable({
    providedIn: 'root'
})
// @ts-ignore
export class RecipeService {

    constructor(private http: HttpClient) {
    }
}
