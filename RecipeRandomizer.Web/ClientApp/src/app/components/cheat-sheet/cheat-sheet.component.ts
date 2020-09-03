import {Component, OnInit} from '@angular/core';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-cheat-sheet',
    templateUrl: './cheat-sheet.component.html',
    styleUrls: []
})
export class CheatSheetComponent implements OnInit {

    constructor(public modal: NgbActiveModal) {
    }

    ngOnInit(): void {
    }

}
