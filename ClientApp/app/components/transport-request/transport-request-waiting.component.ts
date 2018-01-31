import { Component, OnInit, OnDestroy, ViewContainerRef, ViewEncapsulation } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import "rxjs/Rx";
import { Observable } from "rxjs/Rx";
import { Subscription } from 'rxjs/Subscription';
// classes
import {
    IScheduleWeekly, ScheduleWeekly,
    ITransportData, ICondition
} from "../../classes/index";
// services
import {
    TransportReqCommunicateService, TransportDataCommunicateService,
    TransportDataTruckCommunicateService
} from "../../communicates/index";
import {
    TransportService, DialogsService,
    TransportRequestionService, AuthenticationService
} from "../../services/index";

@Component({
    selector: "transport-request-waiting",
    templateUrl: "./transport-request-waiting.component.html",
    styleUrls: ["../../styles/style-of-schedule.component.scss"],
    providers: [
        TransportService,
        TransportRequestionService,
        TransportReqCommunicateService,
        TransportDataCommunicateService,
        TransportDataTruckCommunicateService,
        DialogsService
    ],
    //encapsulation: ViewEncapsulation.None,
})

export class TransportRequestWaitingComponent implements OnInit, OnDestroy {
    schedules: Array<any>;
    columnNames: Array<any>;
    transportDatas: Array<ITransportData>;
    inCondition: ICondition;

    scrollHeight: string;

    message: number = 0;
    count: number = 0;
    time: number = 300;
    transportReq: number = 0;
    companyID: number;

    subscription: Subscription;
    condition: boolean;
    display: boolean;
    showListTransport: boolean;

    constructor(
        private waitingService: TransportService,
        private transportReqService: TransportRequestionService,
        private transportReqComService: TransportReqCommunicateService,
        private authService: AuthenticationService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private route: ActivatedRoute,
        private router: Router,
    ) { }

    // component inti
    ngOnInit(): void {
        if (window.innerWidth >= 1600) {
            this.scrollHeight = 75 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 68 + "vh";
        } else {
            this.scrollHeight = 65 + "vh";
        }

        this.transportDatas = new Array;
        this.condition = false;
        this.route.params.subscribe((params: any) => {
            let condition: number = params["condition"];
            if (condition) {
                this.companyID = -1;
            }
        });
        this.companyID = this.companyID != -1 ? this.authService.getUser.CompanyId : this.companyID;
        this.onGetData();
    }

    // get request data
    onGetData(): void {

        this.inCondition = {
            CompanyID: this.companyID,
            ConditionBool: this.condition,
            ConditionString: ""
        };

        this.transportReqService.getRequestWaiting(this.inCondition)
            .subscribe(result => {
                this.columnNames = new Array<any>();

                for (let name of result.Columns) {
                    if (name.indexOf("Employee") >= 0) {
                        this.columnNames.push({ field: name, header: name, style: { "width": "100px", "text-align":"center" }, styleclass: "time-col" })
                    }
                    else if (name.indexOf("Type") >= 0) {
                        this.columnNames.push({ field: name, header: name, style: { "width": "100px", "text-align": "center" }, styleclass: "type-col" })
                    }
                    else {
                        this.columnNames.push({ field: name, header: name, style: { "width": "250px" }, styleclass: "singleLine", isButton: true })
                    }
                }
                this.schedules = result.Data;
                this.reloadData()
            }, error => {
                this.columnNames = new Array<any>();
                this.schedules = new Array<any>();
                this.reloadData();
            });
    }

    // reload data
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
                    this.onGetData();
                }
            });
    }

    // selected request
    onSelectData(data: any, col: any, row: any): void {
        let splitArray: Array<string> = data.split("#");
        //this.dialogsService.context("Transport Request " + col, "Request by : " + row + " Detail : " + JSON.stringify(splitArray[0]), this.viewContainerRef);
        if (splitArray.length > 0) {
            let requestKey = splitArray[1];
            this.waitingService.getTransportReqByKey(Number(requestKey)).subscribe(data => {
                this.display = true;
                this.transportReqComService.toChild(data);
            }, error => this.dialogsService.error("Error Message", "Can't found key of transport request !!!", this.viewContainerRef));
        } else {
            this.dialogsService.error("Error Message", "Can't found key of transport request !!!", this.viewContainerRef);
        }
    }

    // request to transport
    onRequestToData(data: any, row: any) {

        let splitArray: Array<string> = data.split("#");
        if (splitArray.length > 0) {
            //debug here
            // console.log(JSON.stringify(splitArray));
            let requestKey = splitArray[1];
            this.transportReq = Number(requestKey);
            // selected type of transport car or truck
            this.waitingService.getTransportDataSameTransportRequestByID(this.transportReq)
                .subscribe(dbTransportReqs => {
                    this.showListTransport = true;
                    this.transportDatas = dbTransportReqs;

                    //if (data.CarId && data.CarId > 0)
                    //    this.router.navigate(["transport-data/" + requestKey]);
                    //else if (data.TrailerId && data.TrailerId > 0)
                    //    this.router.navigate(["transport-data-truck/" + requestKey]);
                    //else
                    //    this.router.navigate(["transport-data/" + requestKey]);

                }, error => {
                    this.showListTransport = false;
                    this.transportDatas = new Array;
                    this.newTransportData();
                });
        }
        //this.dialogsService.confirm("Question", "Are you want to build transport data with this request ?", this.viewContainerRef).subscribe(result => {
        //    if (result) {
        //        } else {
        //            this.dialogsService.error("Error Message", "Can't found key of transport request !!!", this.viewContainerRef);
        //        }
        //    }
        //});
    }

    // new transport-data
    newTransportData() {
        if (this.transportReq) {
            this.waitingService.getTransportReqByKey(this.transportReq)
                .subscribe(result => {
                    this.transportReq = 0;
                    this.transportDatas = new Array;

                    if (result.TypeOfVehcile == 1)
                        this.router.navigate(["transport-data/" + result.TransportRequestId]);
                    else
                        this.router.navigate(["transport-data-truck/" + result.TransportRequestId]);
                }, error => this.dialogsService.error("Error Message", "Can't update this transport requested. please try agine later", this.viewContainerRef));
        } else {
            this.dialogsService.error("Error Message", "Can't found key of transport request !!!", this.viewContainerRef);
        }
    }

    // same transport-data
    sameTransportData(transportDataID: number) {
        if (this.transportReq) {
            this.waitingService.putTransportRequestedSameTransportData(transportDataID, this.transportReq)
                .subscribe(result => {
                    this.transportReq = 0;
                    this.showListTransport = false;
                    this.transportDatas = new Array;
                    this.onGetData();
                    this.dialogsService.context("Completed Message", "Update this transport requested has completed.", this.viewContainerRef);
                }, error =>
                {
                    this.showListTransport = false;
                    this.dialogsService.error("Error Message", "Can't update this transport requested. please try agine later", this.viewContainerRef);
                });
        } else {
            this.dialogsService.error("Error Message", "Can't found key of transport request !!!", this.viewContainerRef);
        }
    }

    // cancel data
    onCancelData(data: any, row: any) {
        this.dialogsService.confirm("Question", "Are you want to cancel this request ?", this.viewContainerRef).subscribe(result => {
            if (result) {
                let splitArray: Array<string> = data.split("#");
                if (splitArray.length > 0) {
                    this.waitingService.cancelTransportRequest(splitArray[1]).subscribe(result => {
                        this.onGetData();
                    }, error => {
                        this.dialogsService.error("Error Message", "Can't found key of transport request !!!", this.viewContainerRef);
                    });
                } else {
                    this.dialogsService.error("Error Message", "Can't found key of transport request !!!", this.viewContainerRef);
                }
            }
        });
    }

    // destroy
    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}