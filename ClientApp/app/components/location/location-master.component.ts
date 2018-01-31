import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import "rxjs/Rx";
// classes
import { ILocation, Location } from "../../classes/location.class";
// service
import { LocationCommunicateService } from "../../communicates/location-communicate.service";
import { EmployeeDataService } from "../../services/employee.service";
import { DialogsService } from "../../services/dialogs.service";

@Component({
    selector: "location-master",
    templateUrl: "./location-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        EmployeeDataService,
        LocationCommunicateService,
        DialogsService
    ]
})

export class LocationMasterComponent implements OnInit {
    searchField: FormControl;
    locationForm: FormGroup;
    locations: Array<ILocation>;
    location: ILocation;
    editLocation: ILocation;

    totalRow: number;
    scrollHeight: string;
    showEdit: boolean;
    canSave: boolean;
    constructor(
        private locationService: EmployeeDataService,
        private locationComService: LocationCommunicateService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private fb: FormBuilder,
    ) { }
    // property
    get LocationNull(): boolean {
        return this.location === undefined;
    }
    // component inti
    ngOnInit(): void {
        this.searchField = new FormControl();
        this.bulidForm();
        this.loadData(undefined);
        this.showEdit = false;
        this.canSave = false;

        this.locationComService.ToParent$.subscribe(
            (locationCom: [ILocation, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editLocation = locationCom[0];
                this.canSave = locationCom[1];
                // debug here
                // console.log("can save " + this.canSave);
                // console.log(JSON.stringify(this.editLocation));
            });
    }
    // bulid form
    bulidForm(): void {
        this.locationForm = this.fb.group({
            "search": this.searchField
        });
        // no lazy load
        this.searchField.valueChanges
            .debounceTime(250)
            .distinctUntilChanged()
            .switchMap(term => this.locationService.getLocationByFilter(term.trim()))
            .subscribe(result => {
                this.locations = result.Data;
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
        this.locationService.getLocationByFilter(filter).subscribe(result => {
            this.locations = result.Data;
            this.totalRow = result.TotalRecordCount
            //debug here
            //console.log(JSON.stringify(this.locations));
            //console.log(JSON.stringify(this.tools));
        }, error => console.error(error));

    }
    // on detail view
    onDetailView(): void {
        if (this.location) {
            // debug here
            console.log(JSON.stringify(this.location));
            setTimeout(() => this.locationComService.toChild(this.location), 1000);
        }
    }
    // on detail edit
    onDetailEdit(location?: ILocation): void {
        if (!location) {
            location = new Location();
        }

        this.location = location;
        this.showEdit = true;
        setTimeout(() => this.locationComService.toChildEdit(this.location), 1000);
    }
    // on cancel edit
    onCancelEdit(): void {
        this.editLocation = undefined;
        this.location = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }
    // on submit
    onSubmit(): void {
        if (this.editLocation.Creator) {
            this.updateToDataBase(this.editLocation);
        } else {
            this.insertToDataBase(this.editLocation);
        }
    }
    // on insert data
    insertToDataBase(location: ILocation): void {
        this.locationService.postLocation(location).subscribe(
            (complete: any) => {
                this.location = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.editLocation.Creator = undefined;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on update data
    updateToDataBase(location: ILocation): void {
        this.locationService.putLocation(location.LocationId, location).subscribe(
            (complete: any) => {
                this.location = complete;
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
                this.editLocation = undefined;
                this.loadData(undefined);
            });
    }
}