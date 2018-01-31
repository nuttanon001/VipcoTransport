// angular
import { Component } from "@angular/core";
import { FormBuilder, FormControl, Validators } from "@angular/forms";
// class
import { ICompany, Company } from "../../classes/index";
// component
import { AbstractEditComponent } from "../abstract/abstract-index";
// service
import { CompanyService } from "../../services/index";
import { CompanyCommunicateService } from "../../communicates/index"
// rxjs
import { Subscription } from "rxjs/Subscription";

@Component({
    selector: "company-edit",
    templateUrl: "./company-edit.component.html",
    styleUrls: ["../../styles/edit.style.scss"],
})

export class CompanyEditComponent
    extends AbstractEditComponent<ICompany, CompanyService> {

    constructor(
        companyService: CompanyService,
        companyComService: CompanyCommunicateService,
        private fb: FormBuilder,
    ) {
        super(
            companyService,
            companyComService,
        );
    }

    // on get data by key
    onGetDataByKey(company?: ICompany): void {
        if (company) {
            this.service.getOneKeyNumber(company.CompanyId)
                .subscribe(dbCompany => {
                    this.editValue = dbCompany;
                }, error => console.error(error), () => this.defineData());
        } else {
            this.editValue = new Company();
            this.defineData()
        }
    }

    // define data for edit form
    defineData(): void {
        this.formErrors = {
            EducationName: "",
            Detail: "",
        };
        this.validationMessages = {
            "CompanyCode": {
                "required": "Code is required",
                "minlength": "Code must be at least 1 characters long",
                "maxlength": "Code cannot be more than 20 characters long."
            },
            "CompanyName": {
                "required": "Company name is required",
                "maxlength": "Company name cannot be more than 200 characters long.",
            },
            "Address1": {
                "maxlength": "Address1 cannot be more than 200 characters long.",
            },
            "Address2": {
                "maxlength": "Address2 cannot be more than 200 characters long.",
            },
            "Telephone": {
                "maxlength": "Telephone cannot be more than 50 characters long.",
            }
        }
        this.buildForm();
    }

    // build form
    buildForm(): void {
        this.editValueForm = this.fb.group({
            CompanyId: [this.editValue.CompanyId],
            CompanyCode: [this.editValue.CompanyCode,
                [
                    Validators.required,
                    Validators.minLength(1),
                    Validators.maxLength(20),
                ]
            ],
            CompanyName: [this.editValue.CompanyName,
                [
                    Validators.required,
                    Validators.maxLength(200)
                ]
            ],
            Address1: [this.editValue.Address1,
                [
                    Validators.maxLength(200)
                ]
            ],
            Address2: [this.editValue.Address2,
                [
                    Validators.maxLength(200)
                ]
            ],
            Telephone: [this.editValue.Telephone,
                [
                    Validators.maxLength(50)
                ]
            ],
            Creator: [this.editValue.Creator],
            CreateDate: [this.editValue.CreateDate],
            Modifyer: [this.editValue.Modifyer],
            ModifyDate: [this.editValue.ModifyDate]
        });
        this.editValueForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }
}

