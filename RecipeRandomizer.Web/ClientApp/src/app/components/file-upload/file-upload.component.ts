import {Component, ElementRef, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild} from '@angular/core';

@Component({
    selector: 'app-file-upload',
    templateUrl: './file-upload.component.html',
    styleUrls: ['./file-upload.component.scss']
})
export class FileUploadComponent implements OnInit {

    @ViewChild('fileInput', {static: true}) public fileInput: ElementRef;
    @Input() public onUploadSuccess: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Output() public onFileStaged: EventEmitter<File> = new EventEmitter<File>();
    public file: File;

    public stageFile(): void {
        this.file = this.fileInput.nativeElement.files[0];
        this.onFileStaged.emit(this.file);
    }

    public ngOnInit(): void {
        this.onUploadSuccess.subscribe(
            success => {
                if (success === true) {
                    this.file = null;
                    this.fileInput.nativeElement.value = '';
                }
            });
    }
}
