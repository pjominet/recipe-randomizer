import {Component, OnDestroy, OnInit} from '@angular/core';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {ActivatedRoute, NavigationStart, Router} from '@angular/router';
import {Subject} from 'rxjs';
import {filter, takeUntil} from 'rxjs/operators';
import {LoginComponent} from './login.component';

@Component({
    selector: 'app-modal-container',
    template: '<!-- Login view be rendered here -->'
})
export class LoginModalComponent implements OnInit, OnDestroy {

    private modalSubject = new Subject<any>();
    private modalRef: NgbModalRef = null;

    constructor(private modalService: NgbModal,
                private route: ActivatedRoute,
                private router: Router) {
    }

    ngOnInit(): void {
        this.route.params.pipe(takeUntil(this.modalSubject)).subscribe(params => {
            // create the actual modal
            this.modalRef = this.modalService.open(LoginComponent, {centered: true});

            // go back to parent page after the modal is closed/dismissed
            this.modalRef.result.then(
                result => { // on close
                    if (result === 'forgot-password-redirect') {
                        this.router.navigateByUrl('/forgot-password');
                    } else if (result === 'login-success') {
                        this.router.navigateByUrl('/dashboard');
                    } else {
                        this.router.navigate(['..'], {relativeTo: this.route});
                    }
                }, reason => { // on dismiss
                    if (reason === 'forgot-password-redirect') {
                        this.router.navigateByUrl('/forgot-password');
                    } else if (reason === 'login-success') {
                        this.router.navigateByUrl('/dashboard');
                    } else {
                        this.router.navigate(['..'], {relativeTo: this.route});
                    }
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
