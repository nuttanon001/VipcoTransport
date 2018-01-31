import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs/Observable";
import "rxjs/Rx";
// classes
import { IDriver, Driver, LazyLoad } from "../../classes/index";
import { LazyLoadEvent } from "primeng/primeng";
// service
import { DriverCommunicateService } from "../../communicates/driver-communicate.service";
import {
    EmployeeDataService, CarDataService,
    DialogsService, CompanyService,
    AuthenticationService
} from "../../services/index";

@Component({
    selector: "driver-master",
    templateUrl: "./driver-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        EmployeeDataService,
        CompanyService,
        DriverCommunicateService,
        CarDataService,
        DialogsService
    ]
})

export class DriverMasterComponent implements OnInit {
    searchField: FormControl;
    driverForm: FormGroup;
    drivers: Array<IDriver>;
    driver: IDriver;
    editDriver: IDriver;

    totalRow: number;
    scrollHeight: string;
    showEdit: boolean;
    canSave: boolean;

    companyID: number;
    constructor(
        private driverService: EmployeeDataService,
        private driverComService: DriverCommunicateService,
        private dialogsService: DialogsService,
        private authService: AuthenticationService,
        private viewContainerRef: ViewContainerRef,
        private route: ActivatedRoute,
        private fb: FormBuilder,
    ) { }
    // property
    get DriverNull(): boolean {
        return this.driver === undefined;
    }
    // component inti
    ngOnInit(): void {
        this.searchField = new FormControl();
        this.bulidForm();
        this.showEdit = false;
        this.canSave = false;

        this.driverComService.ToParent$.subscribe(
            (driverCom: [IDriver, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editDriver = driverCom[0];
                this.canSave = driverCom[1];
                // debug here
                // console.log("can save " + this.canSave);
                // console.log(JSON.stringify(this.editDriver));
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
        this.driverForm = this.fb.group({
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

        let lazydata: LazyLoad = {
            first: event.first,
            rows: event.rows,
            sortField: event.sortField,
            sortOrder: event.sortOrder,
            filter: this.searchField.value,
            option: this.companyID
        };

        this.driverService.getDriverAllWithLazyLoad(lazydata).subscribe(result => {
            this.drivers = result.Data;
            this.totalRow = result.TotalRow
            //debug here
            console.log(JSON.stringify(this.drivers));
        }, error => console.error(error));
    }
    // on detail view
    onDetailView(): void {
        if (this.driver) {
            // debug here
            // console.log(JSON.stringify(this.driver));
            setTimeout(() => this.driverComService.toChild(this.driver), 1000);
        }
    }
    // on detail edit
    onDetailEdit(driver?: IDriver): void {
        if (!driver) {
            driver = new Driver();
        }

        this.driver = driver;
        this.showEdit = true;
        setTimeout(() => this.driverComService.toChildEdit(this.driver), 1000);
    }
    // on cancel edit
    onCancelEdit(): void {
        this.editDriver = undefined;
        this.driver = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }
    // on submit
    onSubmit(): void {
        if (this.editDriver.Creator) {
            this.updateToDataBase(this.editDriver);
        } else {
            this.insertToDataBase(this.editDriver);
        }
    }
    // on insert data
    insertToDataBase(driver: IDriver): void {
        this.driverService.postDriver(driver).subscribe(
            (complete: any) => {
                this.driver = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.editDriver.Creator = undefined;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on update data
    updateToDataBase(driver: IDriver): void {
        this.driverService.putDriver(driver.EmployeeDriveId, driver).subscribe(
            (complete: any) => {
                this.driver = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
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
                this.onDetailView();
                this.editDriver = undefined;
                this.loadData(undefined);
            });
    }
}