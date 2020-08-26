import {Component, OnDestroy, OnInit} from '@angular/core';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {ActivatedRoute, NavigationStart, Router} from '@angular/router';
import {Subject} from 'rxjs';
import {filter, takeUntil} from 'rxjs/operators';
import {ForgotPasswordComponent} from './forgot-password.component';

@Component({
    selector: 'app-modal-container',
    template: '<!-- Forgot password view will be rendered here -->'
})
export class ForgotPasswordModalComponent implements OnInit, OnDestroy {

    private modalSubject = new Subject<any>();
    private modalRef: NgbModalRef = null;

    constructor(private modalService: NgbModal,
                private route: ActivatedRoute,
                private router: Router) {
    }

    ngOnInit(): void {
        this.route.params.pipe(takeUntil(this.modalSubject)).subscribe(params => {
            // create the actual modal
            this.modalRef = this.modalService.open(ForgotPasswordComponent, {centered: true});

            // go back to parent page after the modal is closed
            this.modalRef.result.then(
                result => {
                    this.router.navigate(['..'], {relativeTo: this.route});
                }, reason => {
                    this.router.navigate(['..'], {relativeTo: this.route});
                });
        });

        // close modal properly when route is change, by back button or by manual change
        this.router.events.pipe(filter((event: any) => event instanceof NavigationStart))
            .subscribe(() => {
                this.modalRef.close();
            });
    }

    ngOnDestroy(): void {
        this.modalSubject.next();
    }
}
