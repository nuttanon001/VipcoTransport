import { Component, OnInit, OnDestroy, ViewContainerRef} from "@angular/core";
import "rxjs/Rx";
import { Observable } from "rxjs/Rx";
import { Subscription } from 'rxjs/Subscription';
// service
import { TransportDataCommunicateService, TransportDataTruckCommunicateService } from "../../communicates/index";
import {
    TransportService, DialogsService,
    CompanyService
} from "../../services/index";

@Component({
    selector: "schedule-master",
    templateUrl: "./schedule-master.component.html",
    styleUrls: ["../../styles/master.style.scss"],
    providers:
    [
        TransportDataCommunicateService,
        TransportDataTruckCommunicateService,
        TransportService,
        CompanyService,
        DialogsService
    ]
})

export class ScheduleMasterComponent implements OnInit {
    showDaily: boolean;
    showWeekly: boolean;
    showMonthly: boolean;

    constructor(
        private scheduleService: TransportService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
    ) { }

    // component inti
    ngOnInit(): void {
        this.showDaily = false;
        this.showWeekly = true;
        this.showMonthly = false;
    }

    onShowDaily(): void {
        this.showDaily = true;
        this.showWeekly = false;
        this.showMonthly = false;
    }

    onShowWeekly(): void {
        this.showDaily = false;
        this.showWeekly = true;
        this.showMonthly = false;
    }

    onShowMonthly(): void {
        this.showDaily = false;
        this.showWeekly = false;
        this.showMonthly = true;
    }
}