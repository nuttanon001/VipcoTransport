import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
import { Router } from "@angular/router";
// classes
import { IUserEdition } from "../../classes/index";
// services
import {
    UserEditionService, AuthenticationService,
    DialogsService
 } from "../../services/index";
// rxjs
import { Subscription } from "rxjs/Subscription";
// primeng
import { SelectItem } from "primeng/primeng";
// validation
import { PasswordValidation } from "../../validations/index";

@Component({
    selector: "user-config",
    templateUrl: "./user-config.component.html",
    styleUrls: ["../../styles/master.style.scss", "../../styles/view.style.scss"],
    providers: [
        UserEditionService,
        DialogsService,
        AuthenticationService
    ],
})

export class UserConfigComponent implements OnInit {
    userEdition: IUserEdition;
    userEditionForm: FormGroup;
    subscription: Subscription;

    // form validators error
    formErrors = {
        MailAddress: "",
        PasswordNew: "",
    };

    // validation message
    validationMessages = {
        MailAddress: {
            "email": "Mail address can't validation. ",
        },
        PasswordNew: {
            "minlength": "Password request must be at least 5 characters long",
        }
    }

    // constructor
    constructor(
        private router: Router,
        private userEditionService: UserEditionService,
        private dialogsService: DialogsService,
        private authService: AuthenticationService,
        private viewContainerRef: ViewContainerRef,
        private fb: FormBuilder,
    ) { }

    // hook component
    ngOnInit(): void {
        if (this.authService.getUser) {
            this.userEditionService.getOneWithConditionNumber(this.authService.getUser.UserId)
                .subscribe(dbUserEdition => {
                    this.userEdition = dbUserEdition;
                    this.defineData();
                });
        }
    }

    // define data
    defineData(): void {
        this.buildForm();
    }

    // build form
    buildForm(): void {
        this.userEditionForm = this.fb.group({
            UserId: this.userEdition.UserId,
            EmployeeName: this.userEdition.EmployeeName,
            UserName: this.userEdition.UserName,
            MailAddress: [this.userEdition.MailAddress,
                [
                    Validators.email,
                ]
            ],
            PasswordOld: this.userEdition.PasswordOld,
            PasswordNew: [this.userEdition.PasswordNew,
                [
                    Validators.minLength(5),
                ]
            ],
            PasswordConfirm: this.userEdition.PasswordConfirm,
            CompanyName: this.userEdition.CompanyName,
            CompanyID: this.userEdition.CompanyID,
            EmailAlert: this.userEdition.EmailAlert,
            UserHasCompanyId: this.userEdition.UserHasCompanyId
        },
        {
            validator: PasswordValidation.MatchPassword // your validation method
        });
        this.userEditionForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.userEditionForm) { return; }
        const form = this.userEditionForm;
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

    // emit data to master component
    onUpdateUserEdition() {
        if (this.userEditionForm.valid) {
            this.userEdition = this.userEditionForm.value;
            this.userEditionService.post(this.userEdition).subscribe(dbResult => {
                this.dialogsService
                    .context("System message", "Update completed.", this.viewContainerRef)
                    .subscribe(result => {
                        this.authService.logout();
                        this.router.navigate(["/login"]);
                    });
            });
        }
    }
}