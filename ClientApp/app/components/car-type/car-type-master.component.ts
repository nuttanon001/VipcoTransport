import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import "rxjs/Rx";
// classes
import { ICarType, CarType } from "../../classes/car-type.class";
// service
import { CarTypeCommunicateService } from "../../communicates/car-type-communicate.service";
import { CarDataService } from "../../services/car.service";
import { DialogsService } from "../../services/dialogs.service";

@Component({
    selector: "car-type-master",
    templateUrl: "./car-type-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        CarDataService,
        CarTypeCommunicateService,
        DialogsService
    ]
})

export class CarTypeMasterComponent implements OnInit {
    searchField: FormControl;
    carTypeForm: FormGroup;
    carTypes: Array<ICarType>;
    carType: ICarType;
    editCarType: ICarType;

    totalRow: number;
    scrollHeight: string;
    showEdit: boolean;
    canSave: boolean;
    constructor(
        private typeService: CarDataService,
        private typeComService: CarTypeCommunicateService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private fb: FormBuilder,
    ) { }
    // property
    get CarTypeNull(): boolean {
        return this.carType === undefined;
    }
    // component inti
    ngOnInit(): void {
        this.searchField = new FormControl();
        this.bulidForm();
        this.loadData(undefined);
        this.showEdit = false;
        this.canSave = false;

        this.typeComService.ToParent$.subscribe(
            (carTypeCom: [ICarType, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editCarType = carTypeCom[0];
                this.canSave = carTypeCom[1];
                // debug here
                console.log("can save " + this.canSave);
                console.log(JSON.stringify(this.editCarType));
            });
    }
    // bulid form
    bulidForm(): void {
        this.carTypeForm = this.fb.group({
            "search": this.searchField
        });
        // no lazy load
        this.searchField.valueChanges
            .debounceTime(250)
            .distinctUntilChanged()
            .switchMap(term => this.typeService.getCarTypeByFilter(term.trim()))
            .subscribe(result => {
                this.carTypes = result.Data;
                this.totalRow = result.TotalRecordCount
            }, error => console.error(error));

        if (window.innerWidth >= 1600) {
            this.scrollHeight = 78 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 75 + "vh";
        } else {
            this.scrollHeight = 70 + "vh";
        }
    }
    // load data for datatable
    loadData(filter?: string) {
        this.typeService.getCarTypeByFilter(filter).subscribe(result => {
            this.carTypes = result.Data;
            this.totalRow = result.TotalRecordCount
            //debug here
            //console.log(JSON.stringify(this.carTypes));
            //console.log(JSON.stringify(this.tools));
        }, error => console.error(error));

    }
    // on detail view
    onDetailView(): void {
        if (this.carType) {
            // debug here
            // console.log(JSON.stringify(this.carType));
            setTimeout(() => this.typeComService.toChild(this.carType), 1000);
        }
    }
    // on detail edit
    onDetailEdit(carType?: ICarType): void {
        if (!carType) {
            carType = new CarType();
            carType.Type = undefined;
        }

        this.carType = carType;
        this.showEdit = true;
        setTimeout(() => this.typeComService.toChildEdit(this.carType), 1000);
    }
    // on cancel edit
    onCancelEdit(): void {
        this.editCarType = undefined;
        this.carType = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }
    // on submit
    onSubmit(): void {
        if (this.editCarType.Creator) {
            this.updateToDataBase(this.editCarType);
        } else {
            this.insertToDataBase(this.editCarType);
        }
    }
    // on insert data
    insertToDataBase(carType: ICarType): void {
        this.typeService.postCarType(carType).subscribe(
            (complete: any) => {
                this.carType = complete;
                this.saveComplete();
            },
            (error: any) => {
                this.editCarType.Creator = undefined;
                console.error(error);
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on update data
    updateToDataBase(carType: ICarType): void {
        this.typeService.putCarType(carType.CarTypeId, carType).subscribe(
            (complete: any) => {
                this.carType = complete;
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
                this.editCarType = undefined;
                this.loadData(undefined);
            });
    }
}