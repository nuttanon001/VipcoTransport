// angular
import { Component, OnInit, OnDestroy, ViewContainerRef, ViewChild} from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// class
import {
    ICarData, CarData,
    ICarBrand, ICarType,
    ICompany
} from "../../classes/index";
// service
import {
    CarDataService, AuthenticationService,
    DialogsService, CompanyService
} from "../../services/index";
import { CarDataCommunicateService } from "../../communicates/index";
// rxjs
import { Subscription } from 'rxjs/Subscription';
// primeng
import { SelectItem } from "primeng/primeng"

@Component({
    selector: "car-data-detail-edit",
    templateUrl: "./car-data-detail-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
})
export class CarDataDetailEditComponent implements OnInit, OnDestroy {
    @ViewChild("ImageFile") ImageFile;
    carData: ICarData;
    carForm: FormGroup;
    subscription: Subscription;

    carBrands: Array<SelectItem>;
    carTypes: Array<SelectItem>;
    companys: Array<SelectItem>;
    hasChange: boolean;
    // form validators error
    formErrors = {
        CarTypeId: "",
        CarNo: "",
        CarBrand: "",
        RegisterNo: "",
        CarDate: "",
        Insurance: "",
        InsurDate: "",
        ActInsurance: "",
        ActDate: "",
    };

    validationMessages = {
        "CarTypeId": {
            "required": "CarType is required",
        },
        "CarBrand": {
            "required": "CarBrand is required",
        },
        "CarNo": {
            "required": "CarNo is required",
            "minlength": "CarNo must be at least 1 characters long",
            "maxlength": "CarNo cannot be more than 20 characters long."
        },
        "RegisterNo": {
            "required": "RegisterNo is required",
            "minlength": "RegisterNo must be at least 1 characters long",
            "maxlength": "RegisterNo cannot be more than 20 characters long."
        },
        "CarDate": {
            "required": "CarDate is required",
        },
        "Insurance": {
            "required": "Insurance is required",
            "minlength": "Insurance must be at least 1 characters long",
            "maxlength": "Insurance cannot be more than 50 characters long."
        },
        "ActInsurance": {
            "required": "Act insurance is required",
            "minlength": "Act insurance must be at least 1 characters long",
            "maxlength": "Act insurance cannot be more than 50 characters long."
        },
        "InsurDate": {
            "required": "insurance date is required",
        },
        "ActDate": {
            "required": "act date is required",
        },
    }

    constructor(
        private carComService: CarDataCommunicateService,
        private carService: CarDataService,
        private companyService: CompanyService,
        private authService: AuthenticationService,
        private dialogsService:DialogsService,
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
        this.subscription = this.carComService.toChildEdit$.subscribe(
            (car: ICarData) => {
                this.carService.getCarDataByKey(car.CarId)
                    .subscribe(dbCar => {
                        this.carData = dbCar;
                        this.defineData(this.carData);
                    }, error => console.error(error));
            });
    }
    // hook component
    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }

    // define data
    getCarBrandArray() {
        this.carService.getCarBrand()
            .subscribe(result => {
                if (!this.carData.CarBrand) {
                    this.carData.CarBrand = result[0].BrandId;
                    this.carForm.patchValue({ CarBrand: this.carData.CarBrand });
                }
                this.carBrands = new Array;
                for (let item of result) {
                    this.carBrands.push({ label: item.BrandName, value: item.BrandId });
                }
            }, error => console.error(error));
    }

    // define data
    getCarTypeArray() {
        this.carService.getCarType()
            .subscribe(result => {
                result = result.filter(item => item.Type === 1);

                if (!this.carData.CarTypeId) {
                    this.carData.CarTypeId = result[0].CarTypeId;
                    this.carForm.patchValue({ CarTypeId: this.carData.CarTypeId });
                }
                this.carTypes = new Array;
                for (let item of result) {
                    this.carTypes.push({ label: item.CarTypeDesc, value: item.CarTypeId });
                }
            }, error => console.error(error));
    }

    // define data
    getCompanyArray() {
        this.companyService.getAll()
            .subscribe(result => {
                if (!this.carData.CompanyID) {
                    this.carData.CompanyID = this.authService.getUser.CompanyId;
                    this.carForm.patchValue({ CompanyID: this.carData.CompanyID });
                }
                this.companys = new Array;
                for (let item of result) {
                    this.companys.push({ label: item.CompanyName, value: item.CompanyId });
                }
            }, error => console.error(error));
    }

    // define data
    defineData(carData: ICarData): void {
        if (!carData) {
            this.carData = new CarData();
        }
        // debug here
        // console.log("Befor build form");
        this.buildForm();
        // console.log("After build form");
        this.getCarBrandArray();
        this.getCarTypeArray();
        this.getCompanyArray();
    }

    // build form
    buildForm(): void {
        this.carForm = this.fb.group({
            CarId: this.carData.CarId,
            CarTypeId: [this.carData.CarTypeId,
                [
                    Validators.required,
                ]
            ],
            CarNo: [this.carData.CarNo,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(20)
                ]
            ],
            CarBrand: [this.carData.CarBrand,
                [
                    Validators.required,
                ]
            ],
            RegisterNo: [this.carData.RegisterNo,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(20)
                ]
            ],
            CarDate: [this.carData.CarDate,
                [
                    Validators.required,
                ]
            ],
            CarWeight: [this.carData.CarWeight],
            Insurance: [this.carData.Insurance,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(50)
                ]
            ],
            InsurDate: [this.carData.InsurDate,
                [
                    Validators.required,
                ]
            ],
            ActInsurance: [this.carData.ActInsurance,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(50)
                ]
            ],
            ActDate: [this.carData.ActDate,
                [
                    Validators.required
                ]
            ],
            Remark: this.carData.Remark,
            ImagesString: this.carData.ImagesString,
            Status: this.carData.Status,
            Creator: this.carData.Creator,
            Modifyer: this.carData.Modifyer,
            CompanyID: this.carData.CompanyID,
        });
        this.carForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.carForm) { return; }
        this.hasChange = true;
        const form = this.carForm;
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

    onFileUploadChange($event): void {
        let file = $event.dataTransfer ? $event.dataTransfer.files[0] : $event.target.files[0];
        let pattern = /image/;
        // debug here
        // console.log(file.type);
        if (!file.type.match(pattern)) {
            this.ImageFile.nativeElement.value = "";
            this.dialogsService.error("ไม่เข้าเงื่อนไข", "ระบบบันทึกเฉพาะไฟล์รูปภาพเท่านั้น !!!", this.viewContainerRef)
            return;
        } else {
            this.readImageFileToString64($event.target);
        }
    }

    readImageFileToString64(inputValue: any): void {
        var file: File = inputValue.files[0];
        var myReader: FileReader = new FileReader();

        myReader.onloadend = (e) => {
            // debug here
            //console.log(JSON.stringify(myReader.result));
            this.carForm.patchValue({ ImagesString: myReader.result });
        }
        myReader.readAsDataURL(file);
    }

    // emit data to master component
    onFormValid(isValid: boolean) {
        this.carData = this.carForm.value;
        this.carComService.toParent([this.carData, isValid]);
    }
}