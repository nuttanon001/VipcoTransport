// angular
import { Component, OnInit, OnDestroy, ViewContainerRef, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// class
import {
    ITransportReq, TransportReq,
    IAttachFile, ITransportType,
    IContact, ILocation, Location,
    ICompany
} from "../../classes/index";
// service
import {
    TransportService, DialogsService,
    CarDataService, EmployeeDataService,
    AuthenticationService, CompanyService
} from "../../services/index";

import {
    TransportReqCommunicateService,
    ContactOnlyLocationCommunicateService
} from "../../communicates/index"
// rxjs
import { Subscription } from 'rxjs/Subscription';
// primeng
import { SelectItem } from "primeng/primeng";

@Component({
    selector: "transport-request-detail-edit",
    templateUrl: "./transport-request-detail-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
    providers: [ContactOnlyLocationCommunicateService]
})

export class TransportRequestDetailEditComponent implements OnInit, OnDestroy {
    transportReq: ITransportReq;
    transportReqForm: FormGroup;
    subscription: Subscription;

    carTypes: Array<SelectItem>;
    employees: Array<SelectItem>;
    contactSources: Array<SelectItem>;
    contactDestinations: Array<SelectItem>;
    transportTypes: Array<SelectItem>;
    files: Array<string>;
    attachFiles: Array<IAttachFile> = new Array;
    toDay: Date = new Date;
    companys: Array<SelectItem>;

    transportTypeValues: Array<ITransportType>;
    @ViewChild("attachFile") attachFile;

    display: boolean;
    topPanel: number;
    buttomPanel: number;
    // form validators error
    formErrors = {
        CarTypeId: "",
        EmployeeRequestCode: "",
        DepartmentCode: "",
        TransportTypeId:"",
        TransportReqNo: "",
        EmailResponse: "",
        TransportDate:"",
        TransportTime: "",
        ContactSource: "",
        ContactDestination: "",
        ProblemName: "",
        ProblemPhone: "",
        TransportInformation: "",
        WeightLoad: "",
        Passenger: "",
        Length: "",
        Width: "",
    };
    validationMessages = {
        CarTypeId: {
            "required": "Vehicle Type is required",
        },
        EmployeeRequestCode: {
            "required": "Employee code is required",
        },
        DepartmentCode: {
            "required": "Department code is required",
            "minlength": "Department code must be at least 1 characters long",
            "maxlength": "Department code cannot be more than 20 characters long."
        },
        TransportTypeId: {
            "required": "Transport Type is required",
        },
        TransportReqNo: {
            "minlength": "Transport request must be at least 1 characters long",
            "maxlength": "Transport request cannot be more than 20 characters long."
        },
        EmailResponse: {
            //"email": "Mail address can't validation. ",
            "maxlength": "Mail address cannot be more than 200 characters long."
        },
        TransportDate: {
            "required": "Transport date is required",
        },
        TransportTime: {
            "required": "Transport time is required",
            "minlength": "Transport time must be time style hh:mm ",
            "maxlength": "Transport time cannot be more than 10 characters long."
        },
        ContactSource: {
            "required": "To is required",
        },
        ContactDestination: {
            "required": "From is required",
        },
        ProblemName: {
            "required": "Contact name is required",
            "minlength": "Problem name must be at least 1 characters long",
            "maxlength": "Problem name cannot be more than 50 characters long."
        },
        ProblemPhone: {
            "required": "Contact phone is required",
            "minlength": "Problem phone must be at least 1 characters long",
            "maxlength": "Problem phone cannot be more than 20 characters long."
        },
        TransportInformation: {
            "required": "Transport information is required",
            "minlength": "Transport information must be at least 1 characters long",
            "maxlength": "Transport information cannot be more than 500 characters long."
        },
        WeightLoad: {
            "required": "Weight Load is required",
        },
        Passenger: {
            "required": "Passenger is required",
        },
        Length: {
            "required": "Length is required",
        },
        Width: {
            "required": "Width is required",
        }
    }

    constructor(
        private transportComService: TransportReqCommunicateService,
        private contactComService: ContactOnlyLocationCommunicateService,
        private transportService: TransportService,
        private carService: CarDataService,
        private employeeService: EmployeeDataService,
        private dialogsService: DialogsService,
        private companyService: CompanyService,
        private fb: FormBuilder,
        private viewContainerRef: ViewContainerRef,
        private authService: AuthenticationService
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
        this.display = false;
        this.subscription = this.transportComService.toChildEdit$.subscribe(
            (transportReq: ITransportReq) => {
                // debug here
                // console.log(JSON.stringify(transportData));
                if (transportReq.TransportRequestId) {
                    this.transportService.getTransportReqByKey(transportReq.TransportRequestId)
                        .subscribe(dbTransportReq => {
                            // debug here
                            // console.log(JSON.stringify(dbTransportData));
                            // console.log("Here");
                            this.transportReq = dbTransportReq;
                            this.transportService.getAttachFileTransportRequest(this.transportReq.TransportRequestId)
                                .subscribe(dbAttachFile => this.attachFiles = dbAttachFile, error => {
                                    this.attachFiles = new Array;
                                    console.error(error);
                                });
                        }, error => console.error(error), () => {
                            this.defineData(this.transportReq);
                            if (this.attachFiles.length > 0) {
                                // Panel number
                                this.topPanel = 65;
                                this.buttomPanel = 35;
                            } else {
                                // Panel number
                                this.topPanel = 95;
                                this.buttomPanel = 5;
                            }
                        });
                } else {
                    this.transportReq = transportReq;
                    // Panel number
                    this.topPanel = 95;
                    this.buttomPanel = 5;

                    this.defineData(this.transportReq)
                }
            });

        this.contactComService.ToParent$.subscribe(
            (contactCom: [IContact, boolean]) => {
                if (contactCom[1]) {
                    this.getContactArray();
                }
                this.display = false;
            });
    }

    // hook component
    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }

    // define data contact
    getContactArray() {
        this.employeeService.getContactViewModel()
            .subscribe(result => {
                this.contactSources = new Array;
                this.contactDestinations = new Array;
                // for empty value
                let empty: SelectItem = { label: "-", value: 0 };
                this.contactSources.push(empty);
                this.contactDestinations.push(empty);
                for (let item of result) {
                    let newItem: SelectItem = { label: item.LocationName + (item.ContactName ? " - " + item.ContactName : "")  , value: item.ContactId };
                    this.contactSources.push(newItem);
                    this.contactDestinations.push(newItem);
                }
            }, error => console.error(error));
    }

    // define data transpot type
    getTransportTypeArray(): void {
        this.transportService.getTransportType()
            .subscribe(result => {
                this.transportTypeValues = result;

                this.transportTypes = new Array;
                this.transportTypes.push({ label: "-", value: 0 });
                for (let item of result) {
                    this.transportTypes.push({ label: item.TransportTypeDesc, value: item.TransportTypeId });
                }

                if (this.transportReqForm) {
                    if (this.transportReq.TransportTypeId)
                        this.onTransportTypeChange(this.transportReq.TransportTypeId);
                }

            }, error => console.error(error));
    }

    // define data employee
    getEmployeeArray() {
        this.employeeService.getEmployee()
            .subscribe(result => {
                this.employees = new Array;
                this.employees.push({ label: "-", value: undefined });
                for (let item of result) {
                    this.employees.push({ label: item.EmpCode + " " + item.NameThai, value: item.EmpCode });
                }
            }, error => console.error(error));
    }

    // define data car type
    getCarTypeArray() {
        this.carService.getCarType()
            .subscribe(result => {
                this.carTypes = new Array;
                this.carTypes.push({ label: "-", value: undefined });
                for (let item of result) {
                    this.carTypes.push({ label: item.CarTypeDesc, value: item.CarTypeId });
                }
            },error => console.error(error));
    }

    // define data
    getCompanyArray() {
        this.companyService.getAll()
            .subscribe(result => {
                if (!this.transportReq.CompanyID) {
                    this.transportReq.CompanyID = this.authService.getUser.CompanyId;
                    this.transportReqForm.patchValue({ CompanyID: this.transportReq.CompanyID });
                }
                this.companys = new Array;
                for (let item of result) {
                    this.companys.push({ label: item.CompanyName, value: item.CompanyId });
                }
            }, error => console.error(error));
    }

    // define data
    defineData(transportReq: ITransportReq): void {
        if (!transportReq) {
            this.transportReq = new TransportReq();
            this.transportReq.TransportRequestId = 0;
        }
        // debug here
        // console.log("Befor build form");
        this.buildForm();
        // console.log("After build form");
        this.getContactArray();
        this.getTransportTypeArray();
        this.getEmployeeArray();
        this.getCarTypeArray();
        this.getCompanyArray();

        if (this.transportReq.TransportRequestId == 0) {
            if (this.authService.getUser) {
                this.transportReq.EmployeeRequestCode = this.authService.getUser.EmployeeCode;
                this.transportReq.EmailResponse = this.authService.getUser.MailAddress;

                this.transportReqForm.patchValue({
                    EmployeeRequestCode: this.transportReq.EmployeeRequestCode,
                    EmailResponse: this.transportReq.EmailResponse
                });
            }
        }
    }

    // build form
    buildForm(): void {
        this.transportReqForm = this.fb.group({
            TransportRequestId: this.transportReq.TransportRequestId,
            CarTypeId: [this.transportReq.CarTypeId,
                [
                    Validators.required,
                ]
            ],
            EmployeeRequestCode: [this.transportReq.EmployeeRequestCode,
                [
                    Validators.required,
                ]
            ],
            DepartmentCode: [this.transportReq.DepartmentCode,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(200)
                ]
            ],
            TransportType: this.transportReq.TransportType,
            TransportTypeId: [this.transportReq.TransportTypeId,
                [
                    Validators.required,
                ]
            ],
            TransportStatus: this.transportReq.TransportStatus,
            TransportReqNo: [this.transportReq.TransportReqNo,
                [
                    Validators.minLength(1),
                    Validators.maxLength(20)
                ]
            ],
            EmailResponse: [this.transportReq.EmailResponse,
                [
                    Validators.maxLength(200)
                ]
            ],
            TransportReqDate: this.transportReq.TransportReqDate,
            TransportDate: [this.transportReq.TransportDate,
                [
                    Validators.required,
                ]
            ],
            StringTransportDate: this.transportReq.StringTransportDate,
            TransportTime: [this.transportReq.TransportTime,
                [
                    Validators.required,
                    Validators.minLength(5),
                    Validators.maxLength(10)
                ]
            ],
            ContactSource: [this.transportReq.ContactSource,
                [
                    Validators.required,
                ]
            ],
            ContactDestination: [this.transportReq.ContactDestination,
                [
                    Validators.required,
                ]
            ],
            ProblemName: [this.transportReq.ProblemName,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(50)
                ]
            ],
            ProblemPhone: [this.transportReq.ProblemPhone,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(20)
                ]
            ],
            TransportInformation: [this.transportReq.TransportInformation,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(500)
                ]
            ],
            WeightLoad: this.transportReq.WeightLoad,
            Passenger:this.transportReq.Passenger,
            Length: this.transportReq.Length,
            Width: this.transportReq.Width,
            Cost: this.transportReq.Cost,
            JobInfo: this.transportReq.JobInfo,
            Creator: this.transportReq.Creator,
            Modifyer: this.transportReq.Modifyer,
            CompanyID: this.transportReq.CompanyID,
        });
        this.transportReqForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now

        // control on value change
        const controlEmpRe = this.transportReqForm.get("EmployeeRequestCode");
        controlEmpRe.valueChanges.subscribe((data: any) => this.onDropDownCarValueChange(data));

        // change validity control
        this.transportReqForm.get("TransportTypeId")
            .valueChanges.subscribe((transportTypeId: number) => this.onTransportTypeChange(transportTypeId));

        this.transportReqForm.get("CarTypeId")
            .valueChanges.subscribe((carTypeId: number) => this.onCarTypeChange(carTypeId));

        // if (this.transportReq) {
        //    if (this.transportReq.TransportTypeId) {
        //        this.onTransportTypeChange(this.transportReq.TransportTypeId);
        //    }
        // }

        //const controlCar = this.transportReqForm.get("CarId");
        //controlCar.valueChanges.subscribe((data: any) => this.onDropDownCarValueChange(data));
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.transportReqForm) { return; }
        const form = this.transportReqForm;
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

    // on drop down change
    onDropDownCarValueChange(data?: any) {
        if (!this.transportReqForm) { return; }

        const form = this.transportReqForm;
        // check selectedMachine or selectedTool
        const controlEmpRe = form.get("EmployeeRequestCode");
        // debug here
        // console.log("befor disabled");
        if (controlEmpRe) {
            if (controlEmpRe.value) {
                this.employeeService.getEmployeeByKey(controlEmpRe.value)
                    .subscribe(result => form.patchValue({ DepartmentCode: result.SectionName }),
                    error => console.error(error));
            }
        }
        //console.log("value " + control1.value);
    }

    // on car type chare
    onCarTypeChange(carTypeId?: number) {
        if (!this.transportReqForm) { return; }
        if (carTypeId) {
            if (carTypeId === 1) {
                this.transportReqForm.get("WeightLoad")
                    .setValidators([
                        Validators.required,
                        Validators.minLength(1)
                    ]);
            } else {
                this.transportReqForm.get("WeightLoad").setValidators([]);
            }
        }
        this.transportReqForm.get("WeightLoad").updateValueAndValidity();
    }

    // on change validity control
    onTransportTypeChange(transportTypeId?: number) {
        if (!this.transportReqForm) { return; }
        let transportType = this.transportTypeValues.find(item => item.TransportTypeId === transportTypeId);
        if (transportType) {
            if (transportType.Property === 0) {
                this.transportReqForm.get("Length")
                    .setValidators([Validators.required]);
                this.transportReqForm.get("Width")
                    .setValidators([Validators.required]);

                const form = this.transportReqForm.get("CarTypeId");
                if (form) {
                    if (form.value === 1) {
                        this.transportReqForm.get("WeightLoad")
                            .setValidators([
                                Validators.required,
                                Validators.minLength(1)
                            ]);
                    } else {
                        this.transportReqForm.get("WeightLoad")
                            .setValidators([Validators.required]);
                    }
                } else {
                    this.transportReqForm.get("WeightLoad")
                        .setValidators([Validators.required]);
                }

                //clear validators1
                this.transportReqForm.get("Passenger").setValidators([]);
            } else if (transportType.Property === 1) {
                this.transportReqForm.get("Passenger")
                    .setValidators([Validators.required]);

                //clear validators
                this.transportReqForm.get("WeightLoad").setValidators([]);
                this.transportReqForm.get("Length").setValidators([]);
                this.transportReqForm.get("Width").setValidators([]);
            }
        } else {
            //clear validators
            this.transportReqForm.get("Passenger").setValidators([]);
            this.transportReqForm.get("WeightLoad").setValidators([]);
            this.transportReqForm.get("Length").setValidators([]);
            this.transportReqForm.get("Width").setValidators([]);
        }

        this.transportReqForm.get("WeightLoad").updateValueAndValidity();
        this.transportReqForm.get("Length").updateValueAndValidity();
        this.transportReqForm.get("Width").updateValueAndValidity();
        this.transportReqForm.get("Passenger").updateValueAndValidity();
    }

    // emit data to master component
    onFormValid(isValid: boolean) {
        this.transportReq = this.transportReqForm.value;

        //if (this.transportReq.TransportDate) {
        //    this.transportReq.StringTransportDate = this.transportReq.TransportDate.toString();
        //}

        // debug here
        // console.log(JSON.stringify(this.transportReq));

        if (this.transportReq.TransportTime) {
            if (this.transportReq.TransportTime.indexOf("_") >= 0)
                isValid = false;
        }

        let fi = this.attachFile ? this.attachFile.nativeElement : undefined;
        // console.log(JSON.stringify(fi));

        this.transportComService.toParent([this.transportReq, fi , isValid]);
    }

    // file input change
    fileInputChange($event) {
        let file = $event.dataTransfer ? $event.dataTransfer.files : $event.target.files;
        this.files = new Array;

        for (let i = 0; i < file.length; i++) {
            let fileInfo: File  = file[i];
            //console.log(fileInfo);
            //console.log(fileInfo.name);
            if (fileInfo.size <= 5242880)
                this.files.push(fileInfo.name);
            else
                this.dialogsService.error("Limit the size", "Limit the size to below 5MB.", this.viewContainerRef);
        }
        this.onValueChanged();
    }

    // delete file attach
    onDeleteAttachFile(attach: IAttachFile) {
        if (attach) {
            this.dialogsService.confirm("Confirm", "Are you sure you want to delete this file?", this.viewContainerRef)
                .subscribe(result => {
                    if (result) {
                        this.transportService.deleteAttachFileForTransportRequest(attach.AttachId)
                            .subscribe(resutl => this.attachFiles = this.attachFiles.filter(item => item.AttachId != attach.AttachId));
                    }
                });
        }
    }
    // open file attach
    onOpenNewLink(link: string): void {
        if (link)
            window.open(link, "_blank");
    }

    onClickAddLocation(): void {
        this.display = true;
        // new location
        let location: ILocation = new Location();
        location.LocationId = 0;
        // send contact
        setTimeout(() => this.contactComService.toChildEdit(location), 1000);
    }
}