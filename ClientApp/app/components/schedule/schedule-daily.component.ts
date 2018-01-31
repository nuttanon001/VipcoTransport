import { Component, OnInit, OnDestroy, ViewContainerRef, ViewEncapsulation } from "@angular/core";
import "rxjs/Rx";
import { Observable } from "rxjs/Rx";
import { Subscription } from 'rxjs/Subscription';
// service
import { TransportService } from "../../services/transport.service";
import { DialogsService } from "../../services/dialogs.service";

@Component({
    selector: "schedule-daily",
    templateUrl: "./schedule-daily.component.html",
    styleUrls: ["../../styles/style-of-schedule.component.scss"],
    encapsulation: ViewEncapsulation.None,
})

export class ScheduleDailyComponent implements OnInit, OnDestroy {
    pickDate: any;
    schedules: Array<any>;
    columnNames: Array<any>;
    scrollHeight: string;
    message: number = 0;
    count: number = 0;
    time: number = 300;
    subscription: Subscription;
    constructor(
        private scheduleService: TransportService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
    ) { }

    // component inti
    ngOnInit(): void {
        this.pickDate = new Date();
        if (window.innerWidth >= 1600) {
            this.scrollHeight = 78 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 75 + "vh";
        } else {
            this.scrollHeight = 70 + "vh";
        }
        this.onPickDateChange();
    }

    onPickDateChange(): void {
        if (this.pickDate) {
            this.scheduleService.getTransportDailySchedule(this.pickDate)
                .subscribe(result => {
                    this.columnNames = new Array<any>();

                    for (let name of result.Columns) {
                        if (name.indexOf("Time") >= 0) {
                            this.columnNames.push({ field: name, header: name, style: { "width": "200px" }, styleclass: "time-col", frozen: true })
                        } else {
                            this.columnNames.push({ field: name, header: name, styleclass: "singleLine" })
                        }
                    }

                    this.schedules = result.Data;
                    if (this.subscription) {
                        this.subscription.unsubscribe();
                    }
                    this.reloadData()
                }, error => {
                    this.columnNames = new Array<any>();
                    this.schedules = new Array<any>();

                    if (this.subscription) {
                        this.subscription.unsubscribe();
                    }
                    this.reloadData()
                    // console.log("error here");
                });
        }
    }

    reloadData() {
        this.subscription = Observable.interval(1000)
            .take(this.time).map((x) => x + 1)
            .subscribe((x) => {
                this.message = this.time - x;
                this.count = (x / this.time) * 100;
                if (x == this.time) {
                    this.onPickDateChange();
                }
            });
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}