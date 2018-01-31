// angular
import { Component, OnInit, OnDestroy, ViewContainerRef, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// class
import { ICarBrand,CarBrand } from "../../classes/car-brand.class";
// service
import { CarDataService } from "../../services/car.service";
import { CarBrandCommunicateService } from "../../communicates/car-brand-communicate.service"
import { DialogsService } from "../../services/dialogs.service";
// rxjs
import { Subscription } from 'rxjs/Subscription';
// primeng

@Component({
    selector: "car-brand-detail-edit",
    templateUrl: "./car-brand-detail-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
})
export class CarBrandDetailEditComponent implements OnInit, OnDestroy {
    carBrand: ICarBrand;
    carBrandForm: FormGroup;
    subscription: Subscription;

    hasChange: boolean;
    // form validators error
    formErrors = {
        BrandName: "",
        BrandDesc: "",
    };
    validationMessages = {
        "BrandName": {
            "required": "Brand name is required",
            "minlength": "Brand name must be at least 1 characters long",
            "maxlength": "Brand name cannot be more than 50 characters long."
        },
        "BrandDesc": {
            "minlength": "Brand desc must be at least 1 characters long",
            "maxlength": "Brand desc cannot be more than 200 characters long."
        },
    }

    constructor(
        private brandComService: CarBrandCommunicateService,
        private brandService: CarDataService,
        private dialogsService: DialogsService,
        private fb: FormBuilder,
        private viewContainerRef: ViewContainerRef
    ) { }

    // hook component
    ngOnInit(): void {
        this.hasChange = false;
        this.subscription = this.brandComService.toChildEdit$.subscribe(
            (carBrand: ICarBrand) => {
                this.brandService.getCarBrandByKey(carBrand.BrandId)
                    .subscribe(dbBrand => {
                        this.carBrand = dbBrand;
                        this.defineData(this.carBrand);
                    }, error => console.error(error));
            });
    }

    // hook component
    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }

    // define data
    defineData(carBrand: ICarBrand): void {
        if (!carBrand) {
            this.carBrand = new CarBrand();
        }
        // debug here
        // console.log("Befor build form");
        this.buildForm();
        // console.log("After build form");
    }

    // build form
    buildForm(): void {
        this.carBrandForm = this.fb.group({
            BrandId: [this.carBrand.BrandId],
            BrandName: [this.carBrand.BrandName,
            [
                Validators.required,
                Validators.minLength(1),
                Validators.maxLength(50)
            ]
            ],
            BrandDesc: [this.carBrand.BrandDesc,
            [
                Validators.minLength(1),
                Validators.maxLength(200)
            ]
            ],
            Creator: [this.carBrand.Creator],
            Modifyer: [this.carBrand.Modifyer]
        });
        this.carBrandForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.carBrandForm) { return; }
        this.hasChange = true;
        const form = this.carBrandForm;
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
        // on form valid or not
        this.onFormValid(form.valid);
    }
    // emit data to master component
    onFormValid(isValid: boolean) {
        this.carBrand = this.carBrandForm.value;
        this.brandComService.toParent([this.carBrand, isValid]);
    }
}