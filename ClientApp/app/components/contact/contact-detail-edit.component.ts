// angular
import { Component, OnInit, OnDestroy, ViewContainerRef, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
// class
import { IContact, Contact } from "../../classes/contact.class";
// service
import { EmployeeDataService } from "../../services/employee.service";
import { ContactCommunicateService } from "../../communicates/contact-communicate.service"
import { DialogsService } from "../../services/dialogs.service";
// rxjs
import { Subscription } from 'rxjs/Subscription';
// primeng
import { SelectItem } from "primeng/primeng"

@Component({
    selector: "contact-detail-edit",
    templateUrl: "./contact-detail-edit.component.html",
    styleUrls: ["../../styles/style-of-detail-edit.component.scss"],
})
export class ContactDetailEditComponent implements OnInit, OnDestroy {
    contact: IContact;
    contactForm: FormGroup;
    subscription: Subscription;

    locations: Array<SelectItem>;
    hasChange: boolean;
    // form validators error
    formErrors = {
        ContactName: "",
        ContactPhone: "",
    };

    validationMessages = {
        "ContactName": {
            "minlength": "Contact name must be at least 1 characters long",
            "maxlength": "Contact name cannot be more than 50 characters long."
        },
        "ContactPhone": {
            "minlength": "Contact phone must be at least 1 characters long",
            "maxlength": "Contact phone cannot be more than 20 characters long."
        },
    }

    constructor(
        private contactComService: ContactCommunicateService,
        private contactService: EmployeeDataService,
        private dialogsService: DialogsService,
        private fb: FormBuilder,
        private viewContainerRef: ViewContainerRef
    ) { }

    // hook component
    ngOnInit(): void {
        this.hasChange = false;
        this.subscription = this.contactComService.toChildEdit$.subscribe(
            (contact: IContact) => {
                this.contactService.getContactByKey(contact.ContactId)
                    .subscribe(dbContact => {
                        this.contact = dbContact;
                        this.defineData(this.contact);
                    }, error => console.error(error));
            });
    }

    // hook component
    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }

    // define data
    defineData(contact: IContact): void {
        if (!contact) {
            this.contact = new Contact();
        }
        // debug here
        // console.log("Befor build form");
        this.buildForm();
        // console.log("After build form");
        this.getLocationArray();
    }

    // define data
    getLocationArray() {
        this.contactService.getLocation()
            .subscribe(result => {
                if (!this.contact.Location) {
                    this.contact.Location = result[0].LocationId;
                    this.contactForm.patchValue({ Location: this.contact.Location });
                }
                this.locations = new Array;
                for (let item of result) {
                    this.locations.push({ label: item.LocationCode + " " + item.LocationName, value: item.LocationId });
                }
            }, error => console.error(error));
    }

    // build form
    buildForm(): void {
        this.contactForm = this.fb.group({
            ContactId: [this.contact.ContactId],
            Location: [this.contact.Location],
            ContactName: [this.contact.ContactName,
            [
                Validators.minLength(1),
                Validators.maxLength(50)
            ]
            ],
            ContactPhone: [this.contact.ContactPhone,
            [
                Validators.minLength(1),
                Validators.maxLength(20)
            ]
            ],
            Creator: [this.contact.Creator],
            Modifyer: [this.contact.Modifyer]
        });
        this.contactForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.contactForm) { return; }
        this.hasChange = true;
        const form = this.contactForm;
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
        this.contact = this.contactForm.value;
        this.contactComService.toParent([this.contact, isValid]);
    }
}