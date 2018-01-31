import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs/Observable";
import "rxjs/Rx";
// classes
import {
    ITransportReq, TransportReq,
    ITransportData, LazyLoad, IUser
} from "../../classes/index";
import { LazyLoadEvent } from "primeng/primeng";
// communicate
import {
    TransportReqCommunicateService, TransportDataCommunicateService,
    TransportDataTruckCommunicateService,
} from "../../communicates/index";
// service
import {
    TransportService, DialogsService,
    CarDataService, EmployeeDataService,
    CompanyService, AuthenticationService
} from "../../services/index";

@Component({
    selector: "transport-request-master",
    templateUrl: "./transport-request-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        TransportService,
        TransportReqCommunicateService,
        TransportDataCommunicateService,
        TransportDataTruckCommunicateService,
        CarDataService,
        EmployeeDataService,
        CompanyService,
        DialogsService
    ],
    //encapsulation: ViewEncapsulation.None,
})

export class TransportRequestMasterComponent implements OnInit {
    searchField: FormControl;
    transportReqForm: FormGroup;
    transportReqs: Array<ITransportReq>;
    transportReq: ITransportReq;
    editTransportReq: ITransportReq;
    fileUpload: any;

    totalRow: number;
    scrollHeight: string;

    filterUser: boolean;
    _showEdit: boolean;
    canSave: boolean;
    hideleft: boolean;

    companyID: number;
    constructor(
        private transportReqService: TransportService,
        private transportReqComService: TransportReqCommunicateService,
        private transportDataComService: TransportDataCommunicateService,
        private transportDataTruckComService: TransportDataTruckCommunicateService,
        private authService: AuthenticationService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private route: ActivatedRoute,
        private fb: FormBuilder,
    ) { }
    // property
    get TransportReqNull(): boolean {
        return this.transportReq === undefined;
    }

    //get TransportReqStatus(): boolean {
    //    let data:any = this.transportReq;
    //    if (data && !this.showEdit) {
    //        return data.Color === "gold" || data.Color === "red";
    //    }
    //    return true;
    //}

    get getUser(): string {
        if (this.authService.getUser)
            return this.authService.getUser.Username;
        else
            return "";
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
        this.showEdit = false;
        this.canSave = false;
        // this.hideleft = true;
        this.filterUser = true;

        this.searchField = new FormControl();
        this.bulidForm();


        this.transportReqComService.ToParent$.subscribe(
            (transportReqCom: [ITransportReq, any, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editTransportReq = transportReqCom[0];
                this.fileUpload = transportReqCom[1];
                this.canSave = transportReqCom[2];
                // debug here
                //console.log("can save " + this.canSave);
                //console.log(JSON.stringify(this.editTransportReq));
            });
        this.route.params.subscribe((params: any) => {

            let condition: number = params["condition"];
            if (condition) {
                // debug here
                // console.log("route:" + condition);
                this.companyID = -1;
            }
        });
        this.companyID = this.companyID != -1 ? this.authService.getUser.CompanyId : this.companyID;
    }

    // bulid form
    bulidForm(): void {
        this.transportReqForm = this.fb.group({
            search: this.searchField,
            filterUser: this.filterUser
        });

        const controlStatus = this.transportReqForm.get("filterUser");
        controlStatus.valueChanges.subscribe((data: any) => this.onCheckBoxValueChange(data));

        if (window.innerWidth >= 1600) {
            this.scrollHeight = 76 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 74 + "vh";
        } else {
            this.scrollHeight = 70 + "vh";
        }
    }

    onCheckBoxValueChange(data?: any):void {
        if (!this.transportReqForm) { return; }

        this.filterUser = this.transportReqForm.get("filterUser").value;
        this.loadData(undefined);
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

        let lazydata = {
            first: event.first,
            rows: event.rows,
            sortField: event.sortField,
            sortOrder: event.sortOrder,
            filter: this.searchField.value,
            option: this.companyID
        };

        this.transportReqService.getTransportReqByLazyLoad(lazydata).subscribe(result => {
            let data: Array<ITransportReq> = result.Data;
            this.transportReqs = this.filterUser ? data.filter(item => item.Creator === this.getUser) : data;
            this.totalRow = result.TotalRecordCount;
        }, error => console.error(error));
    }

    // on detail view
    onDetailView(): void {
        if (this.transportReq) {
            setTimeout(() => this.transportReqComService.toChild(this.transportReq), 1000);
        }
    }

    // on detail edit
    onDetailEdit(transportReq?: ITransportReq): void {
        if (!transportReq) {
            transportReq = new TransportReq();
            transportReq.TransportRequestId = 0;
        }

        let canEdit: boolean = true;

        if (transportReq.TransportStatus > 1) {
            if (this.authService.getUser.Role < 3)
                canEdit = false;
        } else if (transportReq.TransportStatus === 1) {
            if (this.authService.getUser.Username !== transportReq.Creator) {
                if (this.authService.getUser.Role < 3) {
                    canEdit = false;
                }
            }
        }

        if (canEdit) {
            this.transportReq = transportReq;
            // debug here
            // console.log("onDetailEdit" + JSON.stringify(this.transportReq));
            this.showEdit = true;
            setTimeout(() => this.transportReqComService.toChildEdit(this.transportReq), 1000);
        } else {
            this.dialogsService.error("Access Deny", "You don't have permission to access", this.viewContainerRef);
        }
    }

    // on cancel edit
    onCancelEdit(): void {
        this.editTransportReq = undefined;
        this.transportReq = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }

    // on submit
    onSubmit(): void {
        this.canSave = false;
        if (this.editTransportReq.Creator) {
            this.updateToDataBase(this.editTransportReq);
        } else {
            this.insertToDataBase(this.editTransportReq);
        }
    }

    // on insert data
    insertToDataBase(transportReq: ITransportReq): void {
        // debug here
        // console.log(JSON.stringify(transportReq));

        this.transportReqService.postTransportReq(transportReq).subscribe(
            (complete: any) => {
                this.transportReq = complete;
                if (this.fileUpload) {
                    this.uploadAttachFileForTranspotRequestToDataBase(this.transportReq.TransportRequestId);
                }
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.editTransportReq.Creator = undefined;
                this.canSave = true;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }

    // on update data
    updateToDataBase(transportReq: ITransportReq): void {
        // debug here
        // console.log(JSON.stringify(transportReq));

        this.transportReqService.putTransportReq(transportReq.TransportRequestId, transportReq).subscribe(
            (complete: any) => {
                this.transportReq = complete;
                console.log(this.fileUpload);
                if (this.fileUpload) {
                    this.uploadAttachFileForTranspotRequestToDataBase(this.transportReq.TransportRequestId);
                }
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.canSave = true;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }

    // on update attach file
    uploadAttachFileForTranspotRequestToDataBase(transportReqID: number): void {
        // console.log("upload : " + (this.fileUpload));
        this.transportReqService
            .postAttachFileForTransportRequest(transportReqID, this.fileUpload)
            .subscribe(res => console.log("upload completed"),error => console.error(error));
    }

    // on save complete
    saveComplete(): void {
        this.dialogsService
            .context("System message", "Save completed.", this.viewContainerRef)
            .subscribe(result => {
                this.canSave = false;
                this.showEdit = false;
                this.onDetailView();
                this.editTransportReq = undefined;
                this.fileUpload = undefined;
                this.loadData(undefined);
            });
    }
}