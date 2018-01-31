import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs/Observable";
// classes
import {
    ICarData, CarData, LazyLoad
} from "../../classes/index";
import { LazyLoadEvent } from "primeng/primeng";
// service
import { CarDataCommunicateService } from "../../communicates/index";
import {
    CarDataService, DialogsService,
    CompanyService, AuthenticationService
} from "../../services/index";
// timezone
import * as moment from "moment-timezone";

@Component({
    selector: "car-data-master",
    templateUrl: "./car-data-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        CarDataService,
        CompanyService,
        CarDataCommunicateService,
        DialogsService
    ]
})

export class CarDataMasterComponent implements OnInit {
    searchField: FormControl;
    carDataForm: FormGroup;
    carDatas: Array<ICarData>;
    carData: ICarData;
    editCarData: ICarData;

    totalRow: number;
    scrollHeight: string;
    showEdit: boolean;
    canSave: boolean;

    companyID: number;
    constructor(
        private carService: CarDataService,
        private carComService: CarDataCommunicateService,
        private dialogsService: DialogsService,
        private authService: AuthenticationService,
        private viewContainerRef: ViewContainerRef,
        private route: ActivatedRoute,
        private fb: FormBuilder,
    ) { }
    // property
    get CarDataNull(): boolean {
        return this.carData === undefined;
    }
    // component inti
    ngOnInit(): void {
        this.searchField = new FormControl();
        this.bulidForm();
        this.showEdit = false;
        this.canSave = false;
        this.carComService.ToParent$.subscribe(
            (carDataCom: [ICarData,boolean]) => {
                // debug here
                //console.log("to parent");
                this.editCarData = carDataCom[0];
                this.canSave = carDataCom[1];
                // debug here
                //console.log("can save " + this.canSave);
                //console.log(JSON.stringify(this.editCarData));
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
        this.carDataForm = this.fb.group({
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

        this.carService.getCarDataByLazyLoadData(lazydata).subscribe(result => {
            this.carDatas = result.Data;
            this.totalRow = result.TotalRecordCount;
            //debug here
            //console.log(JSON.stringify(this.carDatas));
            //console.log(JSON.stringify(this.tools));
        }, error => console.error(error));

    }
    // on detail view
    onDetailView(): void {
        if (this.carData) {
            setTimeout(() => this.carComService.toChild(this.carData), 1000);
        }
    }
    // on detail edit
    onDetailEdit(carData?: ICarData): void {
        if (!carData) {
            carData = new CarData();
        }

        this.carData = carData;
        this.showEdit = true;
        setTimeout(() => this.carComService.toChildEdit(this.carData), 1000);
    }
    // on cancel edit
    onCancelEdit() :void{
        this.editCarData = undefined;
        this.carData = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }
    // on submit
    onSubmit(): void{
        if (this.editCarData.Creator) {
            this.updateToDataBase(this.editCarData);
        } else {
            this.insertToDataBase(this.editCarData);
        }
    }
    // on insert data
    insertToDataBase(carData: ICarData): void {
        this.carService.postCarData(carData).subscribe(
            (complete: any) => {
                this.carData = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.editCarData.Creator = undefined;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on update data
    updateToDataBase(carData: ICarData): void {
        this.carService.putCarData(carData.CarId,carData).subscribe(
            (complete: any) => {
                this.carData = complete;
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
                this.editCarData = undefined;
                this.loadData(undefined);
            });
    }
}