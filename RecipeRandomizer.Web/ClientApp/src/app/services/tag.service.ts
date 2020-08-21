import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {TagCategory} from '@app/models/nomenclature/tagCategory';
import {environment} from '@env/environment';
import {map} from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class TagService {

    constructor(private http: HttpClient) {
    }

    public getTagCategories(): Observable<TagCategory[]> {
        return this.http.get<TagCategory[]>(`${environment.apiUrl}/tags/categories`)
            .pipe(map(response => response));
    }
}
