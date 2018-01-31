// angular
import { Component, OnInit, OnDestroy, ViewContainerRef, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// class
import {
    ICarType, ICarData,
    ITrailer, ITransportData,
    TransportData, ICompany
 } from "../../classes/index";
// service
import {
    TransportService, AuthenticationService,
    DialogsService, CarDataService,
    EmployeeDataService, CompanyService
} from "../../services/index";
import { TransportDataCommunicateService } from "../../communicates/transport-data-communicate.service"
// rxjs
import { Subscription } from 'rxjs/Subscription';
// primeng
import { SelectItem } from "primeng/primeng"

@Component({
    selector: "transport-data-detail-edit",
    templateUrl: "./transport-data-detail-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
})
export class TransportDataDetailEditComponent implements OnInit, OnDestroy {
    transportData: ITransportData;
    transportDataForm: FormGroup;
    subscription: Subscription;

    cars: Array<SelectItem>;
    carTypes: Array<SelectItem>;
    drivers: Array<SelectItem>;
    contactSources: Array<SelectItem>;
    contactDestinations: Array<SelectItem>;
    transportTypes: Array<SelectItem>;
    companys: Array<SelectItem>;

    carsData: Array<any>;
    //trailersData: Array<any>;
    carTypesData: Array<ICarType>;
    // form validators error
    formErrors = {
        TransportNo: "",
        TransDate: "",
        TransportTime: "",
        FinalDate: "",
        FinalTime: "",
        TransportInformation: "",
        Remark: "",
    };

    validationMessages = {
        "TransportNo": {
            "minlength": "Transport No must be at least 1 characters long",
            "maxlength": "Transport No cannot be more than 20 characters long."
        },
        "TransDate": {
            "required": "Transport date request is required",
        },
        "TransportTime": {
            "required": "Transport time request is required",
            "minlength": "Transport time must be time style hh:mm ",
            "maxlength": "Transport time cannot be more than 10 characters long."
        },
        "FinalDate": {
            "required": "Return date request is required",
        },
        "FinalTime": {
            "required": "Return time request is required",
            "minlength": "Return time must be time style hh:mm ",
            "maxlength": "Return time cannot be more than 10 characters long."
        },
        "TransportInformation": {
            "minlength": "Transport No must be at least 1 characters long",
            "maxlength": "Transport time cannot be more than 500 characters long."
        },
        "Remark": {
            "minlength": "Transport No must be at least 1 characters long",
            "maxlength": "Transport time cannot be more than 50 characters long."
        },
    }

    constructor(
        private transportComService: TransportDataCommunicateService,
        private transportService: TransportService,
        private carService: CarDataService,
        private authService: AuthenticationService,
        private employeeService: EmployeeDataService,
        private companyService: CompanyService,
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
        this.subscription = this.transportComService.toChildEdit$.subscribe(
            (transportData: ITransportData) => {
                if (this.authService.getUser) {
                    // debug here
                    // console.log(JSON.stringify(transportData));
                    if (transportData.TransportId) {
                        this.transportService.getTransportDataByKey(transportData.TransportId)
                            .subscribe(dbTransportData => {
                                // debug here
                                // console.log(JSON.stringify(dbTransportData));
                                // console.log("Here");
                                this.transportData = dbTransportData;
                            }, error => console.error(error), () => this.defineData(this.transportData));
                    } else {
                        this.transportData = transportData;
                        this.defineData(this.transportData)
                    }
                }
            });
    }

    // hook component
    ngOnDestroy(): void {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }

    // define data for car and trailer
    getCarDataAndTrailerData(): void {
        //this.carService.getCarDataViewModel()
        //    .subscribe(result => this.carsData = result, error => console.error(error));

        //this.carService.getTrailerViewModel()
        //    .subscribe(result => this.trailersData = result, error => console.error(error));

        this.carService.getCarByCompany(this.authService.getUser.CompanyId)
            .subscribe(result => this.carsData = result, error => console.error(error));
    }

    // define data
    refreshCarArray(carType: number): void {
        if (this.carsData) {
            // debug here
            // console.log("have data");
            this.cars = new Array;
            for (let item of this.carsData) {
                if (item.CarTypeId === carType) {
                    this.cars.push({ label: item.CarNo + "-" + item.BrandNameString, value: item.CarId });
                }
            }

            // if empty add -
            if (this.cars.length < 1) {
                this.cars.push({ label: "-", value: undefined });
            }

            this.transportData.CarId = this.cars[0].value;
            this.transportDataForm.patchValue({ CarId: this.transportData.CarId });
        } else {
            this.carService.getCarByCompany(this.authService.getUser.CompanyId)
                .subscribe(result => {
                    // debug here
                    // console.log("getter data");
                    this.carsData = result;
                    this.cars = new Array;
                    for (let item of this.carsData) {
                        if (item.CarTypeId === carType) {
                            this.cars.push({ label: item.CarNo + "-" + item.BrandNameString, value: item.CarId });
                        }
                    }
                    // if empty add -
                    if (this.cars.length < 1) {
                        this.cars.push({ label: "-", value: undefined });
                    }

                    this.transportData.CarId = this.cars[0].value;
                    this.transportDataForm.patchValue({ CarId: this.transportData.CarId });
                }, error => console.error(error));
        }
    }

    // define data
    refreshTrailerArray(carType: number): void {
        // don't use
    }

    // define data
    getDriverArray(): void {
        this.employeeService.getDriverViewModel()
            .subscribe(result => {
                this.drivers = new Array;
                this.drivers.push({ label: "-", value: 0 });
                for (let item of result) {
                    this.drivers.push({ label: item.EmployeeDriveCode + "-" + item.EmployeeName, value: item.EmployeeDriveId });
                }
            }, error => console.error(error));
    }

    // define data
    getContactArray(): void {
        this.employeeService.getContactViewModel()
            .subscribe(result => {
                this.contactSources = new Array;
                this.contactDestinations = new Array;
                // for empty value
                let empty: SelectItem = { label: "-", value: 0 };
                this.contactSources.push(empty);
                this.contactDestinations.push(empty);
                for (let item of result) {
                    let newItem: SelectItem = { label: item.LocationName + (item.ContactName ? " - " + item.ContactName : ""), value: item.ContactId };
                    this.contactSources.push(newItem);
                    this.contactDestinations.push(newItem);
                }
            }, error => console.error(error));
    }

    // define data
    getTransportTypeArray(): void {
        this.transportService.getTransportType()
            .subscribe(result => {
                if (!this.transportData.TransportTypeId) {
                    this.transportData.TransportTypeId = result[0].TransportTypeId;
                    this.transportDataForm.patchValue({ TransportTypeId: this.transportData.TransportTypeId });
                }
                this.transportTypes = new Array;
                // this.transportTypes.push({ label: "-", value: 0 });
                for (let item of result) {
                    this.transportTypes.push({ label: item.TransportTypeDesc, value: item.TransportTypeId });
                }
            }, error => console.error(error));
    }

    // define data
    getCarTypeArray(): void {
        this.carService.getCarType()
            .subscribe(result => {
                this.carTypesData = result.filter(item => item.Type == 1);

                if (!this.transportData.CarTypeId) {
                    this.transportData.CarTypeId = this.carTypesData[0].CarTypeId;
                    this.transportDataForm.patchValue({ CarTypeId: this.transportData.CarTypeId });
                }
                this.carTypes = new Array;
                for (let item of this.carTypesData) {
                    this.carTypes.push({ label: item.CarTypeDesc, value: item.CarTypeId });
                }

                this.onCarTypeIDValueChanged();
            }, error => console.error(error));
    }

    // define data
    getCompanyArray() {
        this.companyService.getAll()
            .subscribe(result => {
                if (!this.transportData.CompanyID) {
                    this.transportData.CompanyID = this.authService.getUser.CompanyId;
                    this.transportDataForm.patchValue({ CompanyID: this.transportData.CompanyID });
                }
                this.companys = new Array;
                for (let item of result) {
                    this.companys.push({ label: item.CompanyName, value: item.CompanyId });
                }
            }, error => console.error(error));
    }

    // define data
    defineData(transportData: ITransportData): void {
        if (!transportData) {
            this.transportData = new TransportData();
            this.transportData.CarId = 0;
            this.transportData.TrailerId = 0;
        }
        // debug here
        // console.log("Befor build form");
        this.buildForm();

        // debug here
        // console.log("After build form");

        // this.getCarDataAndTrailerData();
        this.getDriverArray();
        this.getContactArray();
        this.getTransportTypeArray();
        this.getCarTypeArray();
        this.getCompanyArray();
    }

    // build form
    buildForm(): void {
        // debug here
        // console.log(JSON.stringify(this.transportData));

        this.transportDataForm = this.fb.group({
            TransportId: this.transportData.TransportId,
            CarId: this.transportData.CarId,
            TrailerId: this.transportData.TrailerId,
            CarTypeId: this.transportData.CarTypeId,
            TransportNo: this.transportData.TransportNo,
            ContactSource: this.transportData.ContactSource,
            ContactDestination: this.transportData.ContactDestination,
            EmployeeRequestCode: this.transportData.EmployeeRequestCode,
            EmployeeDrive: this.transportData.EmployeeDrive,
            EmployeeDriveCode: this.transportData.EmployeeDriveCode,
            TransDate: [this.transportData.TransDate,
                [
                    Validators.required
                ]
            ],
            StringTransDate: this.transportData.StringTransDate,
            TransTime: [this.transportData.TransTime,
                [
                    Validators.minLength(4),
                    Validators.maxLength(10),
                    Validators.required
                ]
            ],
            FinalDate: [this.transportData.FinalDate,
                [
                    Validators.required
                ]
            ],
            FinalTime: [this.transportData.FinalTime,
                [
                    Validators.minLength(4),
                    Validators.maxLength(10),
                    Validators.required
                ]
            ],
            TransportTypeId: this.transportData.TransportTypeId,
            TransportInformation: [this.transportData.TransportInformation,
                [
                    Validators.minLength(1),
                    Validators.maxLength(500),
                ]
            ],
            WeightLoad: this.transportData.WeightLoad,
            Width: this.transportData.Width,
            Length: this.transportData.Length,
            Passenger: this.transportData.Passenger,
            JobInfo: this.transportData.JobInfo,
            Remark: [this.transportData.Remark,
                [
                    Validators.minLength(1),
                    Validators.maxLength(50),
                ]
            ],
            StatusNo: this.transportData.StatusNo,
            RoutineCount: this.transportData.RoutineCount,
            RoutineDay: this.transportData.RoutineDay,
            RoutineTransport: this.transportData.RoutineTransport,
            Creator: this.transportData.Creator,
            Modifyer: this.transportData.Modifyer,
            CompanyID: this.transportData.CompanyID,

        });
        this.transportDataForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now

        const controlCarType = this.transportDataForm.get("CarTypeId");
        controlCarType.valueChanges.subscribe((data: any) => this.onCarTypeIDValueChanged(data));
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.transportDataForm) { return; }
        const form = this.transportDataForm;
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

    // event on value CarTypeID
    onCarTypeIDValueChanged(event?: any): void {
        if (!this.transportDataForm) { return; }

        const controlCarType = this.transportDataForm.get("CarTypeId");
        const controlCar = this.transportDataForm.get("CarId");
        const controlTruck = this.transportDataForm.get("TrailerId");

        if (controlCarType) {
            if (controlCarType.value) {
                if (this.carTypesData.length > 0) {
                    // debug here
                    // console.log("onCarTypeIDValueChanged 1");
                    // this.carTypesData.filter((item, index) => item.CarTypeId = controlCarType.value);
                    let data = this.carTypesData.find(item => item.CarTypeId = controlCarType.value);

                    // debug here
                    // console.log("onCarTypeIDValueChanged 2");
                    if (data) {
                        if (data.Type === 1) {
                            this.refreshCarArray(data.CarTypeId);

                            controlCar.enable();
                            controlTruck.disable();

                            // debug here
                            // console.log("onCarTypeIDValueChanged 3");
                        } else {
                            this.refreshTrailerArray(data.CarTypeId);

                            controlCar.disable();
                            controlTruck.enable();
                        }
                    }
                }
            }
        }
    }

    // emit data to master component
    onFormValid(isValid: boolean): void {
        this.transportData = this.transportDataForm.value;

        //if (this.transportData.TransDate) {
        //    this.transportData.StringTransDate = this.transportData.TransDate.toString();
        //}
        // debug here
        //console.log(JSON.stringify(this.transportData));

        if (this.transportData.TransTime) {
            if (this.transportData.TransTime.indexOf("_") >= 0) {
                isValid = false;
            }
        }

        // debug here
        // console.log(JSON.stringify(this.transportData));
        this.transportComService.toParent([this.transportData, isValid]);
    }
}