import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import "rxjs/Rx";
// classes
import { ICarBrand, CarBrand } from "../../classes/car-brand.class";
// service
import { CarBrandCommunicateService } from "../../communicates/car-brand-communicate.service";
import { CarDataService } from "../../services/car.service";
import { DialogsService } from "../../services/dialogs.service";

@Component({
    selector: "car-brand-master",
    templateUrl: "./car-brand-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        CarDataService,
        CarBrandCommunicateService,
        DialogsService
    ]
})

export class CarBrandMasterComponent implements OnInit {
    searchField: FormControl;
    carBrandForm: FormGroup;
    carBrands: Array<ICarBrand>;
    carBrand: ICarBrand;
    editCarBrand: ICarBrand;

    totalRow: number;
    scrollHeight: string;
    showEdit: boolean;
    canSave: boolean;
    constructor(
        private brandService: CarDataService,
        private brandComService: CarBrandCommunicateService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private fb: FormBuilder,
    ) { }
    // property
    get CarBrandNull(): boolean {
        return this.carBrand === undefined;
    }
    // component inti
    ngOnInit(): void {
        this.searchField = new FormControl();
        this.bulidForm();
        this.loadData(undefined);
        this.showEdit = false;
        this.canSave = false;

        this.brandComService.ToParent$.subscribe(
            (carBrandCom: [ICarBrand, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editCarBrand = carBrandCom[0];
                this.canSave = carBrandCom[1];
                // debug here
                //console.log("can save " + this.canSave);
                //console.log(JSON.stringify(this.editCarBrand));
            });
    }
    // bulid form
    bulidForm(): void {
        this.carBrandForm = this.fb.group({
            "search": this.searchField
        });
        // no lazy load
        this.searchField.valueChanges
            .debounceTime(250)
            .distinctUntilChanged()
            .switchMap(term => this.brandService.getCarBrandByFilter(term.trim()))
            .subscribe(result => {
                this.carBrands = result.Data;
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
    loadData(filter? :string) {
        this.brandService.getCarBrandByFilter(filter).subscribe(result => {
            this.carBrands = result.Data;
            this.totalRow = result.TotalRecordCount
            //debug here
            //console.log(JSON.stringify(this.carBrands));
            //console.log(JSON.stringify(this.tools));
        }, error => console.error(error));

    }
    // on detail view
    onDetailView(): void {
        if (this.carBrand) {
            // debug here
            // console.log(JSON.stringify(this.carBrand));
            setTimeout(() => this.brandComService.toChild(this.carBrand), 1000);
        }
    }
    // on detail edit
    onDetailEdit(carBrand?: ICarBrand): void {
        if (!carBrand) {
            carBrand = new CarBrand();
        }

        this.carBrand = carBrand;
        this.showEdit = true;
        setTimeout(() => this.brandComService.toChildEdit(this.carBrand), 1000);
    }
    // on cancel edit
    onCancelEdit(): void {
        this.editCarBrand = undefined;
        this.carBrand = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }
    // on submit
    onSubmit(): void {
        if (this.editCarBrand.Creator) {
            this.updateToDataBase(this.editCarBrand);
        } else {
            this.insertToDataBase(this.editCarBrand);
        }
    }
    // on insert data
    insertToDataBase(carBrand: ICarBrand): void {
        this.brandService.postCarBrand(carBrand).subscribe(
            (complete: any) => {
                this.carBrand = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.editCarBrand.Creator = undefined;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on update data
    updateToDataBase(carBrand: ICarBrand): void {
        this.brandService.putCarBrand(carBrand.BrandId, carBrand).subscribe(
            (complete: any) => {
                this.carBrand = complete;
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
                this.editCarBrand = undefined;
                this.loadData(undefined);
            });
    }
}