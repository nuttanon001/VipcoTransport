import {
    Component, OnInit, OnDestroy,
    ViewContainerRef, ViewEncapsulation
} from "@angular/core";

import "rxjs/Rx";
import { Observable } from "rxjs/Rx";
import { Subscription } from 'rxjs/Subscription';
// classes
import {
    IScheduleWeekly, ScheduleWeekly,
    ITransportData, ICompany
} from "../../classes/index";
// services
import {
    TransportService, CompanyService,
    DialogsService , AuthenticationService
} from "../../services/index";
// communicate
import {
    TransportDataCommunicateService,
    TransportDataTruckCommunicateService
} from "../../communicates/index";

@Component({
    selector: "schedule-weekly",
    templateUrl: "./schedule-weekly.component.html",
    styleUrls: ["../../styles/style-of-schedule.component.scss"],
})

export class ScheduleWeeklyComponent implements OnInit, OnDestroy {
    scheduleWeekly: IScheduleWeekly;
    company: ICompany;
    companys: Array<ICompany>;
    schedules: Array<any>;
    columnNames: Array<any>;

    arrayNumber: number;
    message: number = 0;
    count: number = 0;
    time: number = 300;

    scrollHeight: string;

    display: boolean;
    typeTransport: boolean;

    subscription: Subscription;

    constructor(
        private companyService: CompanyService,
        private scheduleService: TransportService,
        private authService: AuthenticationService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
    ) { }

    // component inti
    ngOnInit(): void {
        this.arrayNumber = 0;
        this.onGetCompanyArrayData();
        this.scheduleWeekly = new ScheduleWeekly();
        this.scheduleWeekly.StartDate = new Date();
        this.scheduleWeekly.EndDate = new Date();
        this.scheduleWeekly.EndDate.setDate(this.scheduleWeekly.EndDate.getDate() + 7);

        if (window.innerWidth >= 1600) {
            this.scrollHeight = 75 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 68 + "vh";
        } else {
            this.scrollHeight = 65 + "vh";
        }
    }
    // on angular hook
    ngOnDestroy(): void {
        if (this.subscription) {
            // prevent memory leak when component destroyed
            this.subscription.unsubscribe();
        }
    }
    // on get company
    onGetCompanyArrayData(): void {
        // Here
        console.log("Test");

        this.companyService.getAll()
            .subscribe(dbCompany => {
                this.companys = dbCompany;
                // add all company
                let allCompany: ICompany = {
                    Address1: "",
                    Address2: "",
                    CompanyCode: "",
                    CompanyId: -1,
                    CompanyName: "All Branch",
                    CreateDate: null,
                    Creator: "",
                    ModifyDate: null,
                    Modifyer: "",
                    Telephone: "",
                };

                this.companys.push(allCompany);
            }, Error => console.error(Error), () => {
                let CompanyId: number = this.authService.getUser ? this.authService.getUser.CompanyId : 2;
                this.company = this.companys.find(value => value.CompanyId === CompanyId);
                this.onDateChange();
            });
    }
    // on get date
    onDateChange(): void {
        if (this.scheduleWeekly) {
            this.scheduleWeekly.CompanyID = this.company.CompanyId;

            this.scheduleService.getTransportWeeklySchedule(this.scheduleWeekly)
                .subscribe(result => {
                    this.columnNames = new Array<any>();

                    for (let name of result.Columns) {
                        if (name.indexOf("Car") >= 0) {
                            this.columnNames.push({ field: name, header: name, style: { "width": "150px" }, styleclass: "time-col" })
                        } else {
                            this.columnNames.push({ field: name, header: name, style: { "width": "450px", "margin-top": "5px " }, styleclass: "singleLine", isButton: true })
                        }
                    }

                    // debug here
                    //console.log("Schedule Weeks");
                    //console.log(JSON.stringify(result));

                    this.schedules = result.Data;
                    this.reloadData()
                }, error => {
                    this.columnNames = new Array<any>();
                    this.schedules = new Array<any>();
                    this.reloadData()
                });
        }
    }
    // on date change
    onStartDateChange(): void {
        if (this.scheduleWeekly.StartDate > this.scheduleWeekly.EndDate) {
            this.scheduleWeekly.EndDate = this.scheduleWeekly.StartDate;
        }
        this.onDateChange();
    }
    // on date change
    onEndDateChange(): void {
        if (this.scheduleWeekly.EndDate < this.scheduleWeekly.StartDate) {
            this.scheduleWeekly.StartDate = this.scheduleWeekly.EndDate;
        }
        this.onDateChange();
    }
    // count down to get data
    reloadData(): void {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
        this.subscription = Observable.interval(1000)
            .take(this.time).map((x) => x + 1)
            .subscribe((x) => {
                this.message = this.time - x;
                this.count = (x / this.time) * 100;
                if (x == this.time) {
                    this.onDateChange();
                }
            });
    }
    // radio change
    onChangeRadio(company: ICompany): void {
        if (company)
            this.onDateChange();
    }

}