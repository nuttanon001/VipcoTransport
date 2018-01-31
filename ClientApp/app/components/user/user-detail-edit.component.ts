import { Component, OnInit, OnDestroy } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// classes
import { IUser, User } from "../../classes/index";
// services
import { UserService, EmployeeDataService, CompanyService, AuthenticationService } from "../../services/index";
import { UserCommunicateService } from "../../communicates/index";
// rxjs
import { Subscription } from "rxjs/Subscription";
// primeng
import { SelectItem } from "primeng/primeng";

@Component({
    selector: "user-detail-edit",
    templateUrl: "./user-detail-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
})

export class UserDetailEditComponent implements OnInit, OnDestroy {
    user: IUser;
    userForm: FormGroup;
    subscription: Subscription;

    employees: Array<SelectItem>;
    roles: Array<SelectItem>;
    companys: Array<SelectItem>;
    // form validators error
    formErrors = {
        Username: "",
        MailAddress: "",
    };
    validationMessages = {
        Username: {
            "required": "User name is required",
            "minlength": "User name must be at least 4 characters long",
            "maxlength": "User name cannot be more than 50 characters long."
        },
        MailAddress: {
            "email": "Mail address can't validation. ",
        },
    }

    constructor(
        private userService: UserService,
        private userComService: UserCommunicateService,
        private companyService: CompanyService,
        private authService: AuthenticationService,
        private empService: EmployeeDataService,
        private fb: FormBuilder,
    ) { }

    // hook component
    ngOnInit(): void {
        this.subscription = this.userComService.ToChildEdit$.subscribe(
            (user: IUser) => {
                if (user.UserId) {
                    this.userService.getUserByKey(user.UserId)
                        .subscribe(dbUser => {
                            this.user = dbUser;
                        }, error => console.error(error), () => this.defineData(this.user));
                } else {
                    this.user = user;
                    this.defineData(this.user)
                }
            });
    }
    // hook component
    ngOnDestroy(): void {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }

    // define data employee
    getEmployeeArray() {
        this.empService.getEmployee()
            .subscribe(result => {
                this.employees = new Array;
                this.employees.push({ label: "-", value: undefined });
                for (let item of result) {
                    this.employees.push({ label: item.EmpCode + " " + item.NameThai + " : " + item.SectionName, value: item.EmpCode });
                }
            }, error => console.error(error));
    }

    // define data role
    getRoleArray() {
        this.roles = new Array;
        this.roles.push({ label: "Requirement", value: 1 });
        this.roles.push({ label: "Standerd", value: 2 });
        this.roles.push({ label: "Administrator", value: 3 });
        this.roles.push({ label: "Top Level", value: 4 });
    }

    // define data
    getCompanyArray() {
        this.companyService.getAll()
            .subscribe(result => {
                if (!this.user.CompanyId) {
                    this.user.CompanyId = this.authService.getUser.CompanyId;
                    this.userForm.patchValue({ CompanyId: this.user.CompanyId });
                }
                this.companys = new Array;
                for (let item of result) {
                    this.companys.push({ label: item.CompanyName, value: item.CompanyId });
                }
            }, error => console.error(error));
    }

    // define data
    defineData(user: IUser): void {
        if (!user) {
            this.user = new User();
            this.user.UserId = 0;
        }
        // debug here
        // console.log("Befor build form");
        this.buildForm();
        // console.log("After build form");
        this.getRoleArray();
        this.getEmployeeArray();
        this.getCompanyArray();
    }

    // build form
    buildForm(): void {
        this.userForm = this.fb.group({
            UserId: [this.user.UserId],
            Username: [this.user.Username,
            [
                Validators.required,
                Validators.minLength(4),
                Validators.maxLength(50)
            ]
            ],
            Password: [this.user.Password],
            EmployeeCode: [this.user.EmployeeCode,
                [
                    Validators.required,
                ]
            ],
            Role: [this.user.Role,
                [
                    Validators.required,
                ]
            ],
            CompanyId: [this.user.CompanyId],
            MailAddress: [this.user.MailAddress,
                [
                    Validators.email,
                ]
            ],
            Creator: [this.user.Creator],
            Modifyer: [this.user.Modifyer]
        });
        this.userForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.userForm) { return; }
        const form = this.userForm;
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
        this.user = this.userForm.value;
        this.userComService.toParent([this.user, isValid]);
    }
}