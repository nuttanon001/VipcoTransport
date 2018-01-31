// angular
import { Component, OnInit, OnDestroy, ViewContainerRef, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// class
import {
    ITrailer, Trailer,
    ICarBrand, ICarType,
    ICompany
 } from "../../classes/index";
// service
import {
    CarDataService, AuthenticationService,
    DialogsService, CompanyService
} from "../../services/index";
import { TrailerCommunicateService } from "../../communicates/trailer-communicate.service"
// rxjs
import { Subscription } from 'rxjs/Subscription';
// primeng
import { SelectItem } from "primeng/primeng"

@Component({
    selector: "trailer-detail-edit",
    templateUrl: "./trailer-detail-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
})
export class TrailerDetailEditComponent implements OnInit, OnDestroy {
    @ViewChild("ImageFile") ImageFile;
    trailer: ITrailer;
    trailerForm: FormGroup;
    subscription: Subscription;

    carBrands: Array<SelectItem>;
    carTypes: Array<SelectItem>;
    companys: Array<SelectItem>;
    hasChange: boolean;
    // form validators error
    formErrors = {
        TrailerNo: "",
        CarTypeId: "",
        Insurance: "",
        TrailerBrand: "",
        RegisterNo: "",
        TrailerDesc: "",
    };
    validationMessages = {
        "TrailerNo": {
            "required": "TrailerNo is required",
            "minlength": "TrailerNo must be at least 1 characters long",
            "maxlength": "TrailerNo cannot be more than 20 characters long."
        },
        "CarTypeId": {
            "required": "Car type is required",
        },
        "Insurance": {
            "minlength": "Insurance must be at least 1 characters long",
            "maxlength": "Insurance cannot be more than 50 characters long."
        },
        "TrailerBrand": {
            "required": "Trailer brand is required",
        },
        "RegisterNo": {
            "minlength": "RegisterNo must be at least 1 characters long",
            "maxlength": "RegisterNo cannot be more than 20 characters long."
        },
        "TrailerDesc": {
            "minlength": "Trailer Desc must be at least 1 characters long",
            "maxlength": "Trailer Desc cannot be more than 80 characters long."
        },
    }

    constructor(
        private traiComService: TrailerCommunicateService,
        private traiService: CarDataService,
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
        this.subscription = this.traiComService.toChildEdit$.subscribe(
            (trailer: ITrailer) => {
                this.traiService.getTrailerByKey(trailer.TrailerId)
                    .subscribe(dbTrailer => {
                        this.trailer = dbTrailer;
                        this.defineData(this.trailer);
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
        this.traiService.getCarBrand()
            .subscribe(result => {
                if (!this.trailer.TrailerBrand) {
                    this.trailer.TrailerBrand = result[0].BrandId;
                    this.trailerForm.patchValue({ TrailerBrand: this.trailer.TrailerBrand });
                }
                this.carBrands = new Array;
                for (let item of result) {
                    this.carBrands.push({ label: item.BrandName, value: item.BrandId });
                }
            }, error => console.error(error));
    }

    // define data
    getCarTypeArray() {
        this.traiService.getCarType()
            .subscribe(result => {
                result = result.filter(item => item.Type === 2);

                if (!this.trailer.CarTypeId) {
                    this.trailer.CarTypeId = result[0].CarTypeId;
                    this.trailerForm.patchValue({ CarTypeId: this.trailer.CarTypeId });
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
                if (!this.trailer.CompanyID) {
                    this.trailer.CompanyID = this.authService.getUser.CompanyId;
                    this.trailerForm.patchValue({ CompanyID: this.trailer.CompanyID });
                }
                this.companys = new Array;
                for (let item of result) {
                    this.companys.push({ label: item.CompanyName, value: item.CompanyId });
                }
            }, error => console.error(error));
    }

    // define data
    defineData(trailer: ITrailer): void {
        if (!trailer) {
            this.trailer = new Trailer();
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
        this.trailerForm = this.fb.group({
            TrailerId: this.trailer.TrailerId,
            TrailerNo: [this.trailer.TrailerNo,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(20)
                ]
            ],
            CarTypeId: [this.trailer.CarTypeId,
                [
                    Validators.required,
                ]
            ],
            Insurance: [this.trailer.Insurance,
                [
                    Validators.minLength(1),
                    Validators.maxLength(50)
                ]
            ],
            InsurDate: this.trailer.InsurDate,
            TrailerBrand: this.trailer.TrailerBrand,
            RegisterNo: [this.trailer.RegisterNo,
                [
                    Validators.minLength(1),
                    Validators.maxLength(20)
                ]
            ],
            TrailerDesc: [this.trailer.TrailerDesc,
                [
                    Validators.minLength(1),
                    Validators.maxLength(80)
                ]
            ],
            TrailerWeight: this.trailer.TrailerWeight,
            TrailerLoad: this.trailer.TrailerLoad,
            Remark: this.trailer.Remark,
            ImagesString: this.trailer.ImagesString,
            Creator: this.trailer.Creator,
            Modifyer: this.trailer.Modifyer,
            CompanyID: this.trailer.CompanyID,
        });
        this.trailerForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.trailerForm) { return; }
        this.hasChange = true;
        const form = this.trailerForm;
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

    // on file upload change
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

    // read image file to string 64
    readImageFileToString64(inputValue: any): void {
        var file: File = inputValue.files[0];
        var myReader: FileReader = new FileReader();

        myReader.onloadend = (e) => {
            this.trailerForm.patchValue({ ImagesString: myReader.result });
        }
        myReader.readAsDataURL(file);
    }

    // emit data to master component
    onFormValid(isValid: boolean) {
        this.trailer = this.trailerForm.value;
        this.traiComService.toParent([this.trailer, isValid]);
    }
}