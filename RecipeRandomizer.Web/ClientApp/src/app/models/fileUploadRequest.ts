export class FileUploadRequest {
    apiUrl: string;
    entityId: number;
    file: File;

    constructor(apiUrl: string, entityId?: number, file?: File) {
        this.apiUrl = apiUrl;
        this.entityId = entityId;
        this.file = file;
    }
}
