import {Component} from '@angular/core';
import {CookieService} from 'ngx-cookie-service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
})
export class HomeComponent {

    public showCookiesInfo: boolean = true;

    constructor(private cookieService: CookieService) {
        this.showCookiesInfo = this.cookieService.get('acceptedCookies') != 'true';
    }

    public acceptCookies(): void {
        this.showCookiesInfo = false;
        this.cookieService.set('acceptedCookies', 'true', 180);
    }
}
