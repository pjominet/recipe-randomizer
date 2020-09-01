import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '@env/environment';
import {map} from 'rxjs/operators';
import {QuantityUnit} from '@app/models/nomenclature/quantityUnit';

@Injectable({
    providedIn: 'root'
})
export class QuantityService {

    constructor(private http: HttpClient) {
    }

    public getQuantityUnits(): Observable<QuantityUnit[]> {
        return this.http.get<QuantityUnit[]>(`${environment.apiUrl}/quantities/units`)
            .pipe(map(response => response));
    }
}
