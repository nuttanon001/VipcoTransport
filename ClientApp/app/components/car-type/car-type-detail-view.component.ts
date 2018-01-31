import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { ICarType } from "../../classes/car-type.class";
// service
import { CarDataService } from "../../services/car.service";
import { CarTypeCommunicateService } from "../../communicates/car-type-communicate.service"

import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "car-type-detail-view",
    templateUrl: "./car-type-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})

export class CarTypeDetailViewComponent implements OnInit, OnDestroy {
    carType: ICarType;
    subscription: Subscription;

    constructor(
        private typeComService: CarTypeCommunicateService,
        private typeService: CarDataService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.typeComService.ToChild$.subscribe(
            (carType: ICarType) => {
                // debug here
                // console.log(JSON.stringify(carType));
                this.typeService.getCarTypeByKey(carType.CarTypeId)
                    .subscribe(dbCarType => this.carType = dbCarType, error => console.error(error))
            });
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}
