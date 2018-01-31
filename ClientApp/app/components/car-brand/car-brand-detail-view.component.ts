import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { ICarBrand } from "../../classes/car-brand.class";
// service
import { CarDataService } from "../../services/car.service";
import { CarBrandCommunicateService } from "../../communicates/car-brand-communicate.service"

import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "car-brand-detail-view",
    templateUrl: "./car-brand-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})

export class CarBrandDetailViewComponent implements OnInit, OnDestroy {
    carBrand: ICarBrand;
    subscription: Subscription;

    constructor(
        private brandComService: CarBrandCommunicateService,
        private brandService: CarDataService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.brandComService.ToChild$.subscribe(
            (carBrand: ICarBrand) => {
                // debug here
                // console.log(JSON.stringify(carBrand));
                this.brandService.getCarBrandByKey(carBrand.BrandId)
                    .subscribe(dbCarBrand => this.carBrand = dbCarBrand, error => console.error(error))});
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}
