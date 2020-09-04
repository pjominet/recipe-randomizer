import {Injectable} from '@angular/core';
import {Observable, Subject} from 'rxjs';


@Injectable({
    providedIn: 'root'
})
export class FileUploadService {
    private attributionSubject: Subject<boolean> = new Subject<boolean>();

    public onFileUpload(): Observable<boolean> {
        return this.attributionSubject.asObservable();
    }

    public setFileUploadSuccess() {
        this.attributionSubject.next(true);
    }
}
