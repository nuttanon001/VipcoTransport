import { Component, OnInit, Input, ViewEncapsulation, ViewContainerRef } from "@angular/core";
// rxjs
import "rxjs/Rx";
import { Observable } from "rxjs/Observable";
// service
import { TransportService } from "../../services/transport.service";
import { DialogsService } from "../../services/dialogs.service";
import { TransportDataCommunicateService } from "../../communicates/transport-data-communicate.service";
import { TransportDataTruckCommunicateService } from "../../communicates/transport-data-communicate-truck.service";
// classes
import { ITransportData } from "../../classes/transport-data.class";

@Component({
    selector: "schedule-chart-bar",
    templateUrl: "./schedule-chart-bar.component.html",
    styleUrls: ["../../styles/style-of-schedule.component.scss"],
    //encapsulation: ViewEncapsulation.None,
})

export class ScheduleChartBar implements OnInit {
    @Input("stringData") stringData: string;
    columns: Array<string>;
    data: Array<any>;
    display: boolean;
    typeTransport: boolean;
    transport: ITransportData;

    constructor(
        private scheduleService: TransportService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private transportDataComService: TransportDataCommunicateService,
        private transportDataTruckComService: TransportDataTruckCommunicateService,
    ) { }

    // component inti
    ngOnInit(): void {
        this.display = false;
        this.typeTransport = false;
        this.getData(this.stringData);
    }

    // get data
    getData(stringData: string) {
        this.scheduleService.getTransportScheduleWithTrasnsportData(stringData).
            subscribe(result => {

                // debug here
                //console.log("Schedule Chart");
                //console.log(JSON.stringify(result));

                this.columns = result.Columns;
                this.data = result.Data;
            }, error => {
                this.columns = new Array<string>();
                this.data = new Array<any>();
            });
    }

    // on click
    onClick(data: string): void {
        // this.dialogsService.context("Message", "Text message.", this.viewContainerRef);

        let splitArray: Array<string> = data.split("#");
        if (splitArray.length > 0) {
            let requestKey = splitArray[1];
            this.scheduleService.getTransportDataByKey(Number(requestKey)).subscribe(data => {
                this.transport = data;
                this.displayData();
            }, error => {
                console.error(error);
                }
            );
        }
    }

    displayData(): void {
        if (this.transport) {
                this.display = true;
                if (this.transport.CarId && this.transport.CarId > 0) {
                    this.typeTransport = true;

                    setTimeout(() => {
                        this.transportDataComService.toChild(this.transport);
                    }, 1000);

                } else if (this.transport.TrailerId && this.transport.TrailerId > 0) {
                    this.typeTransport = false;

                    setTimeout(() => {
                        this.transportDataTruckComService.toChild(this.transport);
                    }, 1000);
                }
        } else {
            this.display = false;
        }
    }
}