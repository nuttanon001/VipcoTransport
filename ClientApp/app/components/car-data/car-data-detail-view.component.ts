import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { ICarData } from "../../classes/car-data.class";
import { ICarBrand } from "../../classes/car-brand.class";
import { ICarType } from "../../classes/car-type.class";
// service
import { CarDataService } from "../../services/car.service";
import { CarDataCommunicateService } from "../../communicates/car-communicate.service"

import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "car-data-detail-view",
    templateUrl: "./car-data-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})
export class CarDataDetailViewComponent implements OnInit, OnDestroy {
    carData: ICarData;
    carBrand: ICarBrand;
    carType: ICarType;
    subscription: Subscription;

    constructor(
        private carComService: CarDataCommunicateService,
        private carService: CarDataService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.carComService.ToChild$.subscribe(
            (carData: ICarData) => {
                this.carService.getCarDataByKey(carData.CarId)
                    .subscribe(dbCarData => this.carData = dbCarData, error => console.error(error),
                    () => {
                        this.carService.getCarBrandByKey(this.carData.CarBrand).subscribe(dbCarBrand => this.carBrand = dbCarBrand);
                        this.carService.getCarTypeByKey(this.carData.CarTypeId).subscribe(dbCarType => this.carType = dbCarType);
                    });
            });
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}
