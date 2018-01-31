import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { ITrailer } from "../../classes/trailer.class";
import { ICarBrand } from "../../classes/car-brand.class";
import { ICarType } from "../../classes/car-type.class";
// service
import { CarDataService } from "../../services/car.service";
import { TrailerCommunicateService } from "../../communicates/trailer-communicate.service"

import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "trailer-detail-view",
    templateUrl: "./trailer-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})

export class TrailerDetailViewComponent implements OnInit, OnDestroy {
    trailer: ITrailer;
    carBrand: ICarBrand;
    carType: ICarType;
    subscription: Subscription;

    constructor(
        private traiComService: TrailerCommunicateService,
        private carService: CarDataService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.traiComService.ToChild$.subscribe(
            (trailer: ITrailer) => {
                // debug here
                // console.log(JSON.stringify(trailer));
                this.carService.getTrailerByKey(trailer.TrailerId)
                    .subscribe(dbTrailer => this.trailer = dbTrailer, error => console.error(error),
                    () => {
                        this.carService.getCarBrandByKey(this.trailer.TrailerBrand).subscribe(dbCarBrand => this.carBrand = dbCarBrand);
                        this.carService.getCarTypeByKey(this.trailer.CarTypeId).subscribe(dbCarType => this.carType = dbCarType);
                    });
            });
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}
