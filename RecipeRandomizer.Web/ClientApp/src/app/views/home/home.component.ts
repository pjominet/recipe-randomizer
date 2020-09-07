import {Component, OnInit} from '@angular/core';
import {CookieService} from 'ngx-cookie-service';
import {RecipeService} from '@app/services/recipe.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {

    public showCookiesInfo: boolean = true;
    public recipeCount: number = 0;

    constructor(private cookieService: CookieService,
                private recipeService: RecipeService) {
        this.showCookiesInfo = this.cookieService.get('acceptedCookies') != 'true';
    }

    public ngOnInit(): void {
        this.recipeService.getPublishedRecipeCount().subscribe((count) => this.recipeCount = count);
    }

    public acceptCookies(): void {
        this.showCookiesInfo = false;
        this.cookieService.set('acceptedCookies', 'true', 180);
    }
}
