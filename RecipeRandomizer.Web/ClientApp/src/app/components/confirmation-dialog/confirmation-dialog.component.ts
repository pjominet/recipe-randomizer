import {Component} from '@angular/core';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-confirmation-dialog',
    templateUrl: './confirmation-dialog.component.html',
    styleUrls: []
})
export class ConfirmationDialogComponent {
    constructor(private activeModal: NgbActiveModal) {
    }

    public confirmNavigation(confirmation: boolean) {
        this.activeModal.close(confirmation);
    }
}
