// angular
import { Component, OnInit, OnDestroy, ViewContainerRef, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// class
import { IEmployee, Employee } from "../../classes/employee.class";
// service
import { EmployeeDataService } from "../../services/employee.service";
import { EmployeeCommunicateService } from "../../communicates/employee-communicate.service"
import { DialogsService } from "../../services/dialogs.service";
// rxjs
import { Subscription } from 'rxjs/Subscription';
// primeng
import { SelectItem } from "primeng/primeng"

@Component({
    selector: "employee-detail-edit",
    templateUrl: "./employee-detail-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
})
export class EmployeeDetailEditComponent implements OnInit, OnDestroy {
    employee: IEmployee;
    employeeForm: FormGroup;
    subscription: Subscription;

    hasChange: boolean;
    // form validators error
    formErrors = {
        EmpCode: "",
        NameThai: "",
        NameEng: "",
    };
    validationMessages = {
        "EmpCode": {
            "required": "Employee code is required",
            "minlength": "Employee code must be at least 1 characters long",
            "maxlength": "Employee code cannot be more than 20 characters long."
        },
        "NameThai": {
            "required": "Name thai is required",
            "minlength": "Name thai must be at least 1 characters long",
            "maxlength": "Name thai cannot be more than 100 characters long."
        },
        "NameEng": {
            "minlength": "Name english must be at least 1 characters long",
            "maxlength": "Name english cannot be more than 100 characters long."
        },
    }

    constructor(
        private empComService: EmployeeCommunicateService,
        private empService: EmployeeDataService,
        private dialogsService: DialogsService,
        private fb: FormBuilder,
        private viewContainerRef: ViewContainerRef
    ) { }

    // hook component
    ngOnInit(): void {
        this.hasChange = false;
        this.subscription = this.empComService.toChildEdit$.subscribe(
            (employee: IEmployee) => {
                this.empService.getEmployeeByKey(employee.EmpCode)
                    .subscribe(dbEmployee => {
                        this.employee = dbEmployee;
                        this.defineData(this.employee);
                    }, error => console.error(error));
            });
    }

    // hook component
    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }

    // define data
    defineData(employee: IEmployee): void {
        if (!employee) {
            this.employee = new Employee();
        }
        // debug here
        // console.log("Befor build form");
        this.buildForm();
        // console.log("After build form");
    }

    // build form
    buildForm(): void {
        this.employeeForm = this.fb.group({
            EmpCode: [this.employee.EmpCode,
            [
                Validators.required,
                Validators.minLength(1),
                Validators.maxLength(20)
            ]
            ],
            NameThai: [this.employee.NameThai,
            [
                Validators.required,
                Validators.minLength(1),
                Validators.maxLength(100)
            ]
            ],
            NameEng: [this.employee.NameEng,
            [
                Validators.minLength(1),
                Validators.maxLength(100)
            ]
            ]
        });
        this.employeeForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.employeeForm) { return; }
        this.hasChange = true;
        const form = this.employeeForm;
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
        this.employee = this.employeeForm.value;
        this.empComService.toParent([this.employee, isValid]);
    }
}