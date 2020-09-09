import {Component, OnInit} from '@angular/core';
import {CookieService} from 'ngx-cookie-service';
import {RecipeService} from '@app/services/recipe.service';
import {IUser} from '@app/models/identity/user';
import {AuthService} from '@app/services/auth.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
    public showCookiesInfo: boolean = true;
    public recipeCount: number = 0;
    public user: IUser;

    constructor(private cookieService: CookieService,
                private recipeService: RecipeService,
                private authService: AuthService) {
        this.user = this.authService.user;
        this.showCookiesInfo = this.cookieService.get('acceptedCookies') != 'true';
    }

    ngOnInit(): void {
        this.recipeService.getPublishedRecipeCount().subscribe((count) => this.recipeCount = count);
    }

    public acceptCookies(): void {
        this.showCookiesInfo = false;
        this.cookieService.set('acceptedCookies', 'true', 180);
    }
}
