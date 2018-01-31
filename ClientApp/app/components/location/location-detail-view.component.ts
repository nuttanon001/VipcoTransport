import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { ILocation } from "../../classes/location.class";
// service
import { EmployeeDataService } from "../../services/employee.service";
import { LocationCommunicateService } from "../../communicates/location-communicate.service"

import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "location-detail-view",
    templateUrl: "./location-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})

export class LocationDetailViewComponent implements OnInit, OnDestroy {
    location: ILocation;
    subscription: Subscription;

    constructor(
        private locationComService: LocationCommunicateService,
        private locationService: EmployeeDataService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.locationComService.ToChild$.subscribe(
            (location: ILocation) => {
                // debug here
                // console.log(JSON.stringify(location));
                this.locationService.getLocationByKey(location.LocationId)
                    .subscribe(dbLocation => this.location = dbLocation, error => console.error(error));
            });
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}
