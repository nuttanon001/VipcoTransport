import { Component, OnInit, ViewContainerRef, ViewEncapsulation } from "@angular/core";
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs/Observable";
import "rxjs/Rx";
// classes
import {
    ITransportData, TransportData,
    ITransportReq, LazyLoad
} from "../../classes/index";
import { LazyLoadEvent } from "primeng/primeng";
// service
import { TransportDataTruckCommunicateService } from "../../communicates/transport-data-communicate-truck.service";
import {
    TransportService, DialogsService,
    CarDataService, EmployeeDataService,
    CompanyService, AuthenticationService
} from "../../services/index";

@Component({
    selector: "transport-data-truck-master",
    templateUrl: "./transport-data-truck-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        TransportService,
        TransportDataTruckCommunicateService,
        CarDataService,
        EmployeeDataService,
        CompanyService,
        DialogsService
    ],
    //encapsulation: ViewEncapsulation.None,
})

export class TransportDataTruckMasterComponent implements OnInit {
    searchField: FormControl;
    transportDataTruckForm: FormGroup;
    transportDataTrucks: Array<ITransportData>;
    transportDataTruck: ITransportData;
    editTransportData: ITransportData;

    transportReqID: number = 0;
    totalRow: number;
    scrollHeight: string;
    _showEdit: boolean;
    canSave: boolean;
    hideleft: boolean;

    companyID: number;

    constructor(
        private transportDataTruckService: TransportService,
        private transportDataTruckComService: TransportDataTruckCommunicateService,
        private authService: AuthenticationService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private route: ActivatedRoute,
        private fb: FormBuilder,
    ) { }

    // property
    get TransportDataNull(): boolean {
        return this.transportDataTruck === undefined;
    }

    get showEdit(): boolean {
        if (this._showEdit != null) {
            return this._showEdit;
        }
    }

    set showEdit(showEdit: boolean) {
        if (showEdit !== this._showEdit) {
            this.hideleft = !showEdit;
            this._showEdit = showEdit;
        }
    }

    // component inti
    ngOnInit(): void {
        this.searchField = new FormControl();
        this.bulidForm();
        this.showEdit = false;
        this.canSave = false;
        //this.hideleft = true;

        this.transportDataTruckComService.ToParent$.subscribe(
            (transportDataTruckCom: [ITransportData, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editTransportData = transportDataTruckCom[0];
                this.canSave = transportDataTruckCom[1];
                // debug here
                //console.log("can save " + this.canSave);
                //console.log(JSON.stringify(this.editTransportData));
            });

        this.route.params.subscribe((params: any) => {
            let key: number = params["key"];
            // debug here
            // console.log("has key : " + key);
            if (key) {
                this.transportDataTruckService.getTransportReqByKey(key).subscribe(result => {

                    this.transportReqID = key;
                    let data: ITransportData = new TransportData();
                    data.TransportId = 0;
                    data.CarTypeId = result.CarTypeId;
                    data.TransportTypeId = result.TransportTypeId;
                    data.ContactSource = result.ContactSource;
                    data.ContactDestination = result.ContactDestination;
                    data.EmployeeRequestCode = result.EmployeeRequestCode;
                    data.TransDate = result.TransportDate;
                    data.TransTime = result.TransportTime;
                    data.TransportInformation = result.TransportInformation;
                    data.JobInfo = result.JobInfo;
                    data.Passenger = result.Passenger;
                    data.WeightLoad = result.WeightLoad;
                    data.Width = result.Width;
                    data.Length = result.Length;
                    data.Remark = ((!result.ProblemName || result.ProblemName == "null") ? "" : "Contact: " + result.ProblemName) + "" + (!result.ProblemPhone || result.ProblemPhone == "null" ? "" : " Phone: " + result.ProblemPhone);

                    this.onDetailEdit(data);
                });
            }
        }, error => console.error(error));

        this.route.params.subscribe((params: any) => {

            let condition: number = params["condition"];
            if (condition) {
                this.companyID = -1;
            }
        });

        if (this.authService.getUser) {
            this.companyID = this.companyID != -1 ? this.authService.getUser.CompanyId : this.companyID;
        } else {
            this.companyID = -1;
        }
    }

    // bulid form
    bulidForm(): void {
        this.transportDataTruckForm = this.fb.group({
            "search": this.searchField
        });

        if (window.innerWidth >= 1600) {
            this.scrollHeight = 78 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 75 + "vh";
        } else {
            this.scrollHeight = 70 + "vh";
        }
    }

    // load data for datatable
    loadData(event: LazyLoadEvent) {
        //in a real application, make a remote request to load data using state metadata from event
        //event.first = First row offset
        //event.rows = Number of rows per page
        //event.sortField = Field name to sort with
        //event.sortOrder = Sort order as number, 1 for asc and -1 for dec
        //filters: FilterMetadata object having field as key and filter value, filter matchMode as value
        if (!event) {
            event = {
                first: 0,
                rows: 100,
                sortField: "",
                sortOrder: 1,
            }
        }

        let lazydata:LazyLoad = {
            first: event.first,
            rows: event.rows,
            sortField: event.sortField,
            sortOrder: event.sortOrder,
            filter: this.searchField.value,
            option: this.companyID
        };

        this.transportDataTruckService.getTransportDataTruckByLazyLoad(lazydata).subscribe(result => {
            this.transportDataTrucks = result.Data;
            this.totalRow = result.TotalRecordCount;
            //debug here
            //console.log(JSON.stringify(this.transportDataTrucks));
            //console.log(JSON.stringify(this.tools));
        }, error => console.error(error));
    }

    // on detail view
    onDetailView(): void {
        if (this.transportDataTruck) {
            setTimeout(() => {
                // debug here
                // console.log("truck" + JSON.stringify(this.transportDataTruck));
                this.transportDataTruckComService.toChild(this.transportDataTruck);
            }, 1000);
        }
    }

    // on detail edit
    onDetailEdit(transportDataTruck?: ITransportData): void {
        if (!transportDataTruck) {
            transportDataTruck = new TransportData();
            transportDataTruck.TransportId = 0;
        }

        this.transportDataTruck = transportDataTruck;
        this.showEdit = true;

        //console.log(JSON.stringify(this.transportDataTruck));

        setTimeout(() => this.transportDataTruckComService.toChildEdit(this.transportDataTruck), 1000);
    }

    // on cancel edit
    onCancelEdit(): void {
        this.editTransportData = undefined;
        this.transportDataTruck = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }

    // on submit
    onSubmit(): void {
        this.canSave = false;
        if (this.editTransportData.Creator) {
            this.updateToDataBase(this.editTransportData);
        } else {
            this.insertToDataBase(this.editTransportData);
        }
    }

    // on insert data
    insertToDataBase(transportDataTruck: ITransportData): void {
        // debug here
        // console.log(JSON.stringify(transportDataTruck));

        this.transportDataTruckService.postTransportData(this.transportReqID,transportDataTruck).subscribe(
            (complete: any) => {
                this.transportDataTruck = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.editTransportData.Creator = undefined;
                this.canSave = true;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }

    // on update data
    updateToDataBase(transportDataTruck: ITransportData): void {
        // debug here
        // console.log(JSON.stringify(transportDataTruck));

        this.transportDataTruckService.putTransportData(transportDataTruck.TransportId, transportDataTruck).subscribe(
            (complete: any) => {
                this.transportDataTruck = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.canSave = true;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }

    // on save complete
    saveComplete(): void {
        this.dialogsService
            .context("System message", "Save completed.", this.viewContainerRef)
            .subscribe(result => {
                this.canSave = false;
                this.showEdit = false;
                this.transportReqID = 0;
                this.onDetailView();
                this.editTransportData = undefined;
                this.loadData(undefined);
            });
    }

    reportExcel(transportData?: ITransportData) {
        if (transportData) {
            this.transportDataTruckService.getTransportationReport(transportData.TransportId)
                .subscribe(data => {
                    let link = document.createElement('a');
                    link.href = window.URL.createObjectURL(data);
                    link.download = transportData.TransportNo + ".xlsx";
                    link.click();
                },
                error => console.log("Error downloading the file."),
                () => console.log('Completed file download.'));
        }
    }

    reportPdf(transportData?: ITransportData) {
        if (transportData) {
            this.transportDataTruckService.getTransportationPdfReport(transportData.TransportId)
                .subscribe(data => {
                    let link = document.createElement('a');
                    link.href = window.URL.createObjectURL(data);
                    link.download = transportData.TransportNo + ".pdf";
                    link.click();
                },
                error => console.log("Error downloading the file."),
                () => console.log('Completed file download.'));
        }
    }
}