import { Component, OnInit, OnDestroy, ViewContainerRef } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
// rxjs
import { Observable } from "rxjs/Rx";
import { Subscription } from 'rxjs/Subscription';
// classes
import { ITransportData } from "../../classes/index";
// service
import { TransportDataCommunicateService, TransportDataTruckCommunicateService } from "../../communicates/index";
import { TransportService } from "../../services/index";

@Component({
    selector: "report-master",
    templateUrl: "./report-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers:
    [
        TransportService,
        TransportDataCommunicateService,
        TransportDataTruckCommunicateService,
    ]
})


export class ReportMasterComponent implements OnInit {

    typeTransport: boolean;
    transportData: ITransportData;

    constructor(
        private reportService: TransportService,
        private carCommunicateService: TransportDataCommunicateService,
        private trailerCommunicateService: TransportDataTruckCommunicateService,
        private route: ActivatedRoute,
    ) { }

    // component inti
    ngOnInit(): void {
        this.route.params.subscribe((params: any) => {
            let key: number = params["key"];
            if (key) {
                this.reportService.getTransportDataByKey(key).subscribe(result => {
                    this.transportData = result;
                },error => console.error(error),() => this.displayData());
            }
        }, error => console.error(error));
    }

    // display data
    displayData(): void {
        if (this.transportData) {
            if (this.transportData.CarId && this.transportData.CarId > 0) {
                this.typeTransport = true;

                setTimeout(() => {
                    this.carCommunicateService.toChild(this.transportData);
                }, 1000);

            } else if (this.transportData.TrailerId && this.transportData.TrailerId > 0) {
                this.typeTransport = false;

                setTimeout(() => {
                    this.trailerCommunicateService.toChild(this.transportData);
                }, 1000);
            }
        }
    }
}