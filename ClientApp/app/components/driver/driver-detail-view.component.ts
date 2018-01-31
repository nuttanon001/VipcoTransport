import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { IDriver } from "../../classes/driver.class";
// service
import { EmployeeDataService } from "../../services/employee.service";
import { DriverCommunicateService } from "../../communicates/driver-communicate.service"

import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "driver-detail-view",
    templateUrl: "./driver-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})

export class DriverDetailViewComponent implements OnInit, OnDestroy {
    driver: IDriver;
    subscription: Subscription;

    constructor(
        private driverComService: DriverCommunicateService,
        private driverService: EmployeeDataService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.driverComService.ToChild$.subscribe(
            (driver: IDriver) => {
                // debug here
                // console.log(JSON.stringify(driver));
                this.driverService.getDriverByKey(driver.EmployeeDriveId)
                    .subscribe(dbDriver => this.driver = dbDriver, error => console.error(error));
            });
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}
