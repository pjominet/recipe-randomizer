import {Component, OnDestroy, OnInit} from '@angular/core';
import {RecipeService} from '@app/services/recipe.service';
import {Recipe} from '@app/models/recipe';
import {RecipeComponent} from '@app/views/recipes/recipe/recipe.component';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {AttributionComponent} from '../attribution/attribution.component';
import {AttributionService} from '../attribution/attribution.service';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-orphan-recipes',
    templateUrl: './orphan-recipes.component.html',
    styleUrls: []
})
export class OrphanRecipesComponent implements OnInit, OnDestroy {

    public orphans: Recipe[] = [];
    public sub: Subscription;

    constructor(private recipeService: RecipeService,
                private modalService: NgbModal,
                private attributionService: AttributionService) {
    }

    public ngOnInit(): void {
        this.recipeService.getOrphanRecipes().subscribe(
            orphans => {
                this.orphans = orphans;
            });

        this.sub = this.attributionService.onAttribution().subscribe(
            (recipeId) => {
                if (recipeId) {
                    this.orphans = this.orphans.filter(o => o.id !== recipeId);
                }
            }
        );
    }

    public showRecipe(id: number, event: any): void {
        if (event.target.tagName.toLowerCase() === 'td') {
            let modalRef = this.modalService.open(RecipeComponent, {size: 'xl', scrollable: true, backdrop: 'static'});
            modalRef.componentInstance.id = id;
        }
    }

    public showAttributeToUserDialog(id: number): void {
        let modalRef = this.modalService.open(AttributionComponent, {centered: true, size: 'lg'});
        modalRef.componentInstance.recipeId = id;
    }

    public ngOnDestroy(): void {
        this.sub.unsubscribe();
    }
}
