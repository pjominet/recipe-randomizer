import {Injectable} from '@angular/core';
import {HttpClient, HttpEvent} from '@angular/common/http';
import {Observable} from 'rxjs';
import {FileUploadRequest} from '@app/models/fileUploadRequest';

@Injectable({
    providedIn: 'root'
})
export class UploadService {

    constructor(private http: HttpClient) {
    }

    public uploadFile(fileUploadRequest: FileUploadRequest): Observable<HttpEvent<any>> {
        const formData: FormData = new FormData();
        formData.append('id', `${fileUploadRequest.entityId}`);
        formData.append('file', fileUploadRequest.file);

        return this.http.post<any>(fileUploadRequest.apiUrl, formData, {reportProgress: true, responseType: 'json'});
    }
}
