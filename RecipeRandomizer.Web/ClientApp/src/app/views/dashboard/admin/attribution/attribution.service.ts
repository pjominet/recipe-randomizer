import {Injectable} from '@angular/core';
import {Observable, Subject} from 'rxjs';


@Injectable({
    providedIn: 'root'
})
export class AttributionService {
    private attributionSubject: Subject<number> = new Subject<number>();

    public onAttribution(): Observable<number> {
        return this.attributionSubject.asObservable();
    }

    public attributionSuccess(recipeId: number) {
        this.attributionSubject.next(recipeId);
    }
}
