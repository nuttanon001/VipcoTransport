// angular
import { Component, OnInit, OnDestroy, ViewContainerRef, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// class
import { IDriver, Driver } from "../../classes/index";
// service
import {
    EmployeeDataService, CarDataService,
    DialogsService, CompanyService,
    AuthenticationService
} from "../../services/index";
import { DriverCommunicateService } from "../../communicates/driver-communicate.service"
// rxjs
import { Subscription } from 'rxjs/Subscription';
// primeng
import { SelectItem } from "primeng/primeng"

@Component({
    selector: "driver-detail-edit",
    templateUrl: "./driver-detail-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
})
export class DriverDetailEditComponent implements OnInit, OnDestroy {
    driver: IDriver;
    driverForm: FormGroup;
    subscription: Subscription;

    cars: Array<SelectItem>;
    trailers: Array<SelectItem>;
    employees: Array<SelectItem>;
    companys: Array<SelectItem>;
    hasChange: boolean;
    // form validators error
    formErrors = {
        EmployeeDriveCode: "",
        PhoneNumber: "",
    };
    validationMessages = {
        "EmployeeDriveCode": {
            "required": "Employee is required."
        },
        "PhoneNumber": {
            "minlength": "Phone Number must be at least 1 characters long",
            "maxlength": "Phone Number cannot be more than 20 characters long."
        },
    }

    constructor(
        private driverComService: DriverCommunicateService,
        private driverService: EmployeeDataService,
        private carService: CarDataService,
        private companyService: CompanyService,
        private authService: AuthenticationService,
        private dialogsService: DialogsService,
        private fb: FormBuilder,
        private viewContainerRef: ViewContainerRef
    ) { }
    //property
    get CanChangeCompany(): boolean {
        if (this.authService.getUser)
            return this.authService.getUser.Role > 2;
        else
            return false;
    }
    // hook component
    ngOnInit(): void {
        this.hasChange = false;
        this.subscription = this.driverComService.toChildEdit$.subscribe(
            (driver: IDriver) => {
                this.driverService.getDriverByKey(driver.EmployeeDriveId)
                    .subscribe(dbDriver => {
                        this.driver = dbDriver;
                        this.defineData(this.driver);
                    }, error => console.error(error));
            });
    }

    // hook component
    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }

    // define data
    getCarArray() {
        this.carService.getCarDataViewModel()
            .subscribe(result => {
                this.cars = new Array;
                this.cars.push({ label:"-", value: 0});
                for (let item of result) {
                    this.cars.push({ label: item.CarNo + "-" + item.BrandNameString, value: item.CarId });
                }
            }, error => console.error(error));
    }

    // define data
    getTrailerArray() {
        this.carService.getTrailerViewModel()
            .subscribe(result => {
                this.trailers = new Array;
                this.trailers.push({ label: "-", value: 0 });
                for (let item of result) {
                    this.trailers.push({ label: item.TrailerNo + "-" + item.BrandNameString, value: item.TrailerId });
                }
            }, error => console.error(error));
    }

    // define data
    getEmployeeArray() {
        this.driverService.getEmployee()
            .subscribe(result => {
                //if (!this.driver.EmployeeDriveCode) {
                //    this.driver.EmployeeDriveCode = result[0].EmpCode;
                //    this.driverForm.patchValue({ EmployeeDriveCode: this.driver.EmployeeDriveCode });
                //}
                this.employees = new Array;
                this.employees.push({
                    label: "-", value: undefined
                });
                for (let item of result) {
                    this.employees.push({ label: item.EmpCode + " " + item.NameThai, value: item.EmpCode });
                }
            }, error => console.error(error));
    }

    // define data
    getCompanyArray() {
        this.companyService.getAll()
            .subscribe(result => {
                if (!this.driver.CompanyID) {
                    this.driver.CompanyID = this.authService.getUser.CompanyId;
                    this.driverForm.patchValue({ CompanyID: this.driver.CompanyID });
                }
                this.companys = new Array;
                for (let item of result) {
                    this.companys.push({ label: item.CompanyName, value: item.CompanyId });
                }
            }, error => console.error(error));
    }

    // define data
    defineData(driver: IDriver): void {
        if (!driver) {
            this.driver = new Driver();
        }
        // debug here
        // console.log("Befor build form");
        this.buildForm();
        // console.log("After build form");
        this.getCarArray();
        this.getTrailerArray();
        this.getEmployeeArray();
        this.getCompanyArray();
    }

    // build form
    buildForm(): void {
        this.driverForm = this.fb.group({
            EmployeeDriveId: this.driver.EmployeeDriveId,
            EmployeeDriveCode: [this.driver.EmployeeDriveCode,
                [
                    Validators.required
                ]
            ],
            CarId: this.driver.CarId,
            TrailerId: this.driver.TrailerId,
            PhoneNumber: [this.driver.PhoneNumber,
                [
                    Validators.minLength(1),
                    Validators.maxLength(20)
                ]
            ],
            Creator: this.driver.Creator,
            Modifyer: this.driver.Modifyer,
            CompanyID: this.driver.CompanyID,

        });
        this.driverForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.driverForm) { return; }
        this.hasChange = true;
        const form = this.driverForm;
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
        this.driver = this.driverForm.value;
        this.driverComService.toParent([this.driver, isValid]);
    }
}