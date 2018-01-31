import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { ITransportReq, IAttachFile, ITransportData } from "../../classes/index";
// service
import { TransportService } from "../../services/index";
import {
    TransportReqCommunicateService, TransportDataCommunicateService,
    TransportDataTruckCommunicateService
} from "../../communicates/index"
// rxjs
import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "transport-request-detail-view",
    templateUrl: "./transport-request-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})
export class TransportRequestDetailViewComponent implements OnInit, OnDestroy {
    transportReq: ITransportReq;
    transportData: ITransportData;

    attachFiles: Array<IAttachFile> = new Array;
    subscription: Subscription;

    typeTransport: boolean;
    topPanel: number;
    buttomPanel: number;

    constructor(
        private transportComService: TransportReqCommunicateService,
        private transportService: TransportService,
        private transportDataComService: TransportDataCommunicateService,
        private transportDataTruckComService: TransportDataTruckCommunicateService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.transportComService.ToChild$.subscribe(
            (transportReq: ITransportReq) => {
                // debug here
                // console.log(JSON.stringify(transportReq));
                this.transportService.getTransportReqByKey(transportReq.TransportRequestId)
                    .subscribe(dbTransportReq => {
                        this.transportReq = dbTransportReq;
                        // get attach files if have
                        this.transportService.getAttachFileTransportRequest(this.transportReq.TransportRequestId)
                            .subscribe(dbAttachFile => {
                                this.attachFiles = dbAttachFile;
                            }, error => {
                                this.attachFiles = new Array;
                            }, () => {
                                if (this.attachFiles.length > 0) {
                                    this.topPanel = 60;
                                    this.buttomPanel = 40;
                                } else {
                                    this.topPanel = 95;
                                    this.buttomPanel = 5;
                                }
                            });
                        // get transportation if have
                        this.getTransprotDataByTransportRequestedID(this.transportReq);

                    }, error => console.error(error));
            });
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }

    onOpenNewLink(link: string): void {
        if (link)
            window.open(link, '_blank');
    }

    getStyles(status: number) {
        return {
            color: status === 1 ? "goldenrod" : (status === 2 ? "blue" : (status === 3 ? "forestgreen" : "red"))
        };
    }

    // on detail transport-data view
    getTransprotDataByTransportRequestedID(transportReq: ITransportReq): void {
        if (transportReq) {
            this.transportService.getTransportDataByTransportRequest(transportReq.TransportRequestId)
                .subscribe(dbTransportData => {
                    this.transportData = dbTransportData;
                    this.displayData();
                }, error => {
                    this.transportData = undefined;
                });
        }
    }

    // display dialog box transprot-data
    displayData(): void {
        if (this.transportData) {
            if (this.transportData.CarId && this.transportData.CarId > 0) {
                this.typeTransport = true;

                setTimeout(() => {
                    this.transportDataComService.toChild(this.transportData);
                }, 1000);

            } else if (this.transportData.TrailerId && this.transportData.TrailerId > 0) {
                this.typeTransport = false;

                setTimeout(() => {
                    this.transportDataTruckComService.toChild(this.transportData);
                }, 1000);
            }
        }
    }
}
