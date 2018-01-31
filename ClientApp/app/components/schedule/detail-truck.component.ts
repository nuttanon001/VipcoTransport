import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { ITransportData, ITransportReq } from "../../classes/index";
// service
import { TransportService } from "../../services/index";
import { TransportDataTruckCommunicateService } from "../../communicates/index"

import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "detail-truck",
    templateUrl: "./detail-truck.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})
export class DetailTruckComponent implements OnInit, OnDestroy {
    transportData: ITransportData;
    subscription: Subscription;
    transportReqs: Array<ITransportReq>;

    constructor(
        private transportComService: TransportDataTruckCommunicateService,
        private transportService: TransportService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.transportComService.ToChild$.subscribe(
            (transportData: ITransportData) => {
                this.transportService.getTransportDataByKey(transportData.TransportId)
                    .subscribe(dbTransportData => {
                        this.transportData = dbTransportData;
                        this.getTransportReq(dbTransportData.TransportId);
                        // debug here
                        //console.log("truck detail " + JSON.stringify(this.transportData));
                    }, error => {
                        console.log("this detail truck error");
                        console.error(error);
                    });
            });
    }

    // ngif show table of transportrequested
    get showTransportRequest(): boolean {
        if (this.transportReqs)
            return this.transportReqs.length > 0;
        else
            return false;
    }

    // get transport request filter by transport data id
    getTransportReq(transportId: number): void {
        if (transportId) {
            this.transportService.getTransportRequestByTransportDataID(transportId)
                .subscribe(dbTransportReq => {
                    this.transportReqs = dbTransportReq;
                }, error => {
                    this.transportReqs = undefined;
                });
        }
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}
