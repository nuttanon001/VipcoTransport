// angular
import { Component, OnInit, OnDestroy, ViewContainerRef, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// class
import { ICarType, CarType } from "../../classes/car-type.class";
// service
import { CarDataService } from "../../services/car.service";
import { CarTypeCommunicateService } from "../../communicates/car-type-communicate.service"
import { DialogsService } from "../../services/dialogs.service";
// rxjs
import { Subscription } from 'rxjs/Subscription';
// primeng
import { SelectItem } from "primeng/primeng"

@Component({
    selector: "car-type-detail-edit",
    templateUrl: "./car-type-detail-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
})
export class CarTypeDetailEditComponent implements OnInit, OnDestroy {
    carType: ICarType;
    carTypeForm: FormGroup;
    subscription: Subscription;

    hasChange: boolean;
    vehicleTypes: Array<SelectItem>;

    // form validators error
    formErrors = {
        CarTypeNo: "",
        CarTypeDesc: "",
        Remark: "",
        Type: "",
    };
    validationMessages = {
        "CarTypeNo": {
            "required": "Type no is required",
            "minlength": "Type no must be at least 1 characters long",
            "maxlength": "Type no cannot be more than 50 characters long."
        },
        "CarTypeDesc": {
            "required": "Description is required",
            "minlength": "Type desc must be at least 1 characters long",
            "maxlength": "Type desc cannot be more than 50 characters long."
        },
        "Type": {
            "required": "Vehicle Type is required",
        },
        "Remark": {
            "minlength": "Remark must be at least 1 characters long",
            "maxlength": "Remark cannot be more than 50 characters long."
        },
    }

    constructor(
        private typeComService: CarTypeCommunicateService,
        private typeService: CarDataService,
        private dialogsService: DialogsService,
        private fb: FormBuilder,
        private viewContainerRef: ViewContainerRef
    ) { }

    // hook component
    ngOnInit(): void {
        this.hasChange = false;
        this.subscription = this.typeComService.toChildEdit$.subscribe(
            (carType: ICarType) => {
                this.typeService.getCarTypeByKey(carType.CarTypeId)
                    .subscribe(dbType => {
                        this.carType = dbType;
                        this.defineData(this.carType);
                    }, error => console.error(error));
            });
    }

    // hook component
    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }

    // define data
    defineData(carType: ICarType): void {
        if (!carType) {
            this.carType = new CarType();
        }

        this.vehicleTypes = new Array;
        this.vehicleTypes.push({ label: "-", value: undefined });
        this.vehicleTypes.push({ label: "CarType", value: 1 });
        this.vehicleTypes.push({ label: "TruckType", value: 2 });


        // debug here
        // console.log("Befor build form");
        this.buildForm();
        // console.log("After build form");
    }

    // build form
    buildForm(): void {
        this.carTypeForm = this.fb.group({
            CarTypeId: [this.carType.CarTypeId],
            Type: [this.carType.Type,
                [
                    Validators.required,
                ]
            ],
            CarTypeNo: [this.carType.CarTypeNo,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(20)
                ]
            ],
            CarTypeDesc: [this.carType.CarTypeDesc,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(50)
                ]
            ],
            Remark: [this.carType.Remark,
                [
                    Validators.minLength(1),
                    Validators.maxLength(50)
                ]
            ],
            Creator: [this.carType.Creator],
            Modifyer: [this.carType.Modifyer]
        });
        this.carTypeForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.carTypeForm) { return; }
        this.hasChange = true;
        const form = this.carTypeForm;
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
        this.carType = this.carTypeForm.value;
        this.typeComService.toParent([this.carType, isValid]);
    }
}