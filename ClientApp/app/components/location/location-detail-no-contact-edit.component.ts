// angular
import { Component, OnInit, OnDestroy, ViewContainerRef } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// class
import { ILocation, Location } from "../../classes/location.class";
// service
import { EmployeeDataService } from "../../services/employee.service";
import { ContactOnlyLocationCommunicateService } from "../../communicates/index"
import { DialogsService } from "../../services/dialogs.service";
// rxjs
import { Subscription } from 'rxjs/Subscription';
// primeng
import { SelectItem } from "primeng/primeng"

@Component({
    selector: "location-detail-no-contact-edit",
    templateUrl: "./location-detail-no-contact-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
})
export class LocationDetailNoContactEditComponent implements OnInit, OnDestroy {
    location: ILocation;
    locationForm: FormGroup;
    subscription: Subscription;

    hasChange: boolean;
    // form validators error
    formErrors = {
        LocationCode: "",
        LocationName: "",
        LocationAddress: ""
    };

    validationMessages = {
        "LocationCode": {
            "minlength": "Location name must be at least 1 characters long",
            "maxlength": "Location name cannot be more than 20 characters long."
        },
        "LocationName": {
            "required": "Location name is required",
            "minlength": "Location name must be at least 1 characters long",
            "maxlength": "Location name cannot be more than 50 characters long."
        },
        "LocationAddress": {
            "minlength": "Location name must be at least 1 characters long",
            "maxlength": "Location name cannot be more than 200 characters long."
        },
    }

    constructor(
        private locationComService: ContactOnlyLocationCommunicateService,
        private locationService: EmployeeDataService,
        private dialogsService: DialogsService,
        private fb: FormBuilder,
        private viewContainerRef: ViewContainerRef
    ) { }

    // hook component
    ngOnInit(): void {
        this.hasChange = false;
        this.subscription = this.locationComService.toChildEdit$.subscribe(
            (location: ILocation) => {
                this.location = location;
                this.defineData(this.location);
            });
    }

    // hook component
    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
    // define data
    defineData(location: ILocation): void {
        if (!location)
            this.location = new Location();

        this.buildForm();
    }

    // build form
    buildForm(): void {
        this.locationForm = this.fb.group({
            LocationId: [this.location.LocationId],
            LocationCode: [this.location.LocationCode,
                [
                    Validators.minLength(1),
                    Validators.maxLength(20)
                ]
            ],
            LocationName: [this.location.LocationName,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(50)
                ]
            ],
            LocationAddress: [this.location.LocationAddress,
                [
                    Validators.minLength(1),
                    Validators.maxLength(200)
                ]
            ],
            Creator: [this.location.Creator],
            Modifyer: [this.location.Modifyer]
        });
        this.locationForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.locationForm) { return; }
        this.hasChange = true;
        const form = this.locationForm;
        for (const field in this.formErrors) {
            // clear previous error message (if any)
            this.formErrors[field] = "";
            const control = form.get(field);
            if (control && control.dirty && !control.valid) {
                const messages = this.validationMessages[field];

                for (const key in control.errors) {
                    this.formErrors[field] += messages[key] + " ";
                }
            }
        }
    }

    // on completed location
    onCompletedLocation(): void {
        let location: ILocation = this.locationForm.value;
        if (location) {
            this.locationService.postContactOnlyLocaion(location)
                .subscribe(dbContact => {
                    console.log(JSON.stringify(dbContact));
                    if (dbContact) {
                        this.locationComService.toParent([dbContact, true]);
                    }
                }, error => {
                    this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef);
                    this.locationComService.toParent([undefined, false]);
                });
        }
    }

    //on cancel location
    onCancelLocation(): void {
        console.log("Failed");
        this.locationComService.toParent([undefined, false]);
    }
}