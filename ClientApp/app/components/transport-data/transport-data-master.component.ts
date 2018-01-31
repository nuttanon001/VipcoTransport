import { Component, OnInit, ViewContainerRef, ViewEncapsulation} from "@angular/core";
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
import { TransportDataCommunicateService } from "../../communicates/transport-data-communicate.service";
import {
    TransportService, DialogsService,
    CarDataService, EmployeeDataService,
    CompanyService, AuthenticationService
} from "../../services/index";

@Component({
    selector: "transport-data-master",
    templateUrl: "./transport-data-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        TransportService,
        TransportDataCommunicateService,
        CarDataService,
        EmployeeDataService,
        CompanyService,
        DialogsService
    ],
    //encapsulation: ViewEncapsulation.None,
})

export class TransportDataMasterComponent implements OnInit {
    searchField: FormControl;
    transportDataForm: FormGroup;
    transportDatas: Array<ITransportData>;
    transportData: ITransportData;
    editTransportData: ITransportData;

    transportReqID: number = 0;
    totalRow: number;
    scrollHeight: string;
    _showEdit: boolean;
    canSave: boolean;
    hideleft: boolean;

    companyID: number;

    constructor(
        private transportDataService: TransportService,
        private transportDataComService: TransportDataCommunicateService,
        private authService: AuthenticationService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private route: ActivatedRoute,
        private fb: FormBuilder,
    ) { }

    // property
    get TransportDataNull(): boolean {
        return this.transportData === undefined;
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
        this.canSave = false
        //this.hideleft = true;

        this.transportDataComService.ToParent$.subscribe(
            (transportDataCom: [ITransportData, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editTransportData = transportDataCom[0];
                this.canSave = transportDataCom[1];
                // debug here
                //console.log("can save " + this.canSave);
                //console.log(JSON.stringify(this.editTransportData));
            });

        this.route.params.subscribe((params: any) => {
            let key: number = params["key"];
            // debug here
            // console.log("has key : " + key);
            if (key) {
                this.transportDataService.getTransportReqByKey(key).subscribe(result => {

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
                // debug here
                // console.log("route:" + condition);
                this.companyID = -1;
            }
        });
        if (this.authService.getUser) {
            this.companyID = this.companyID != -1 ? this.authService.getUser.CompanyId : this.companyID;
        } else {
            this.companyID = -1;
        }
        // this.companyID = this.companyID != -1 ? this.authService.getUser.CompanyId : this.companyID;
    }

    // bulid form
    bulidForm(): void {
        this.transportDataForm = this.fb.group({
            "search": this.searchField
        });

        if (window.innerWidth >= 1600) {
            this.scrollHeight = 78 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 75 + "vh";
        } else {
            this.scrollHeight = 70 + "vh";
        }

        // console.log("scrollHeight : " + this.scrollHeight);
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

        let lazydata: LazyLoad = {
            first: event.first,
            rows: event.rows,
            sortField: event.sortField,
            sortOrder: event.sortOrder,
            filter: this.searchField.value,
            option: this.companyID
        };

        this.transportDataService.getTransportDataByLazyLoad(lazydata).subscribe(result => {
            this.transportDatas = result.Data;
            this.totalRow = result.TotalRecordCount;
        }, error => console.error(error));
    }

    // on detail view
    onDetailView(): void {
        if (this.transportData) {
            setTimeout(() => {
                this.transportDataComService.toChild(this.transportData);
            }, 1000);
        }
    }

    // on detail edit
    onDetailEdit(transportData?: ITransportData): void {
        if (!transportData) {
            transportData = new TransportData();
            transportData.TransportId = 0;
        }

        this.transportData = transportData;
        this.showEdit = true;

        //console.log(JSON.stringify(this.transportData));

        setTimeout(() => this.transportDataComService.toChildEdit(this.transportData), 1000);
    }

    // on cancel edit
    onCancelEdit(): void {
        this.editTransportData = undefined;
        this.transportData = undefined;
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
    insertToDataBase(transportData: ITransportData): void {
        // debug here
        // console.log(JSON.stringify(transportData));

        this.transportDataService.postTransportData(this.transportReqID,transportData).subscribe(
            (complete: any) => {
                this.transportData = complete;
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
    updateToDataBase(transportData: ITransportData): void {
        // debug here
        // console.log(JSON.stringify(transportData));

        this.transportDataService.putTransportData(transportData.TransportId, transportData).subscribe(
            (complete: any) => {
                this.transportData = complete;
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

    // ... for array
    testMethod(...data) {
        for (let i = 0; i < data.length; i++) {
            console.log("Extra Arg: " + data[i]);
        }

        data.forEach((item, index) => {
            console.log("no." + index + " value : " + item);
        });
    }

    reportExcel(transportData?: ITransportData) {
        if (transportData) {
            this.transportDataService.getTransportationReport(transportData.TransportId)
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
            this.transportDataService.getTransportationPdfReport(transportData.TransportId)
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