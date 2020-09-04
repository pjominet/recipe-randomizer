import {Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild} from '@angular/core';
import {FileUploadService} from './file-upload.service';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-file-upload',
    templateUrl: './file-upload.component.html',
    styleUrls: []
})
export class FileUploadComponent implements OnInit, OnDestroy {

    @ViewChild('fileInput', {static: true}) public fileInput: ElementRef;
    @Input() public allowedMimeTypes: string = 'text/plain';
    @Output() public onFileStaged: EventEmitter<File> = new EventEmitter<File>();
    public file: File;
    public resultSub: Subscription;

    constructor(private fileUploadService: FileUploadService) {
    }

    public stageFile(): void {
        this.file = this.fileInput.nativeElement.files[0];
        this.onFileStaged.emit(this.file);
    }

    public ngOnInit(): void {
        this.resultSub = this.fileUploadService.onFileUpload().subscribe(
            success => {
                if (success === true) {
                    this.file = null;
                    this.fileInput.nativeElement.value = '';
                }
            }
        );
    }

    public ngOnDestroy(): void {
        this.resultSub.unsubscribe();
    }
}
