export class FileUploadRequest {
    url: string;
    id: number;
    file: File;

    constructor(url: string, id?: number, file?: File) {
        this.url = url;
        this.id = id;
        this.file = file;
    }
}
