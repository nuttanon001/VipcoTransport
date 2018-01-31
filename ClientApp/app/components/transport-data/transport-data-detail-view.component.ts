import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { ITransportData,ITransportReq } from "../../classes/index";
// service
import { TransportService } from "../../services/transport.service";
import { TransportDataCommunicateService } from "../../communicates/transport-data-communicate.service"

import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "transport-data-detail-view",
    templateUrl: "./transport-data-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})
export class TransportDataDetailViewComponent implements OnInit, OnDestroy {
    transportData: ITransportData;
    subscription: Subscription;

    transportReqs: Array<ITransportReq>;

    constructor(
        private transportComService: TransportDataCommunicateService,
        private transportService: TransportService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.transportComService.ToChild$.subscribe(
            (transportData: ITransportData) => {
                // debug here
                // console.log(JSON.stringify(transportData));
                this.transportService.getTransportDataByKey(transportData.TransportId)
                    .subscribe(dbTransportData => {
                        this.transportData = dbTransportData;
                        this.getTransportReq(dbTransportData.TransportId);
                    }, error => console.error(error));
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
