import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { Observable } from "rxjs/Observable";
import "rxjs/Rx";
// classes
import { IContact, Contact } from "../../classes/contact.class";
// service
import { ContactCommunicateService } from "../../communicates/contact-communicate.service";
import { EmployeeDataService,DialogsService } from "../../services/index";

@Component({
    selector: "contact-master",
    templateUrl: "./contact-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        EmployeeDataService,
        ContactCommunicateService,
        DialogsService
    ]
})

export class ContactMasterComponent implements OnInit {
    searchField: FormControl;
    contactForm: FormGroup;
    contacts: Array<IContact>;
    contact: IContact;
    editContact: IContact;

    totalRow: number;
    scrollHeight: string;
    showEdit: boolean;
    canSave: boolean;
    constructor(
        private contactService: EmployeeDataService,
        private contactComService: ContactCommunicateService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private fb: FormBuilder,
    ) { }
    // property
    get ContactNull(): boolean {
        return this.contact === undefined;
    }
    // component inti
    ngOnInit(): void {
        this.searchField = new FormControl();
        this.bulidForm();
        this.loadData(undefined);
        this.showEdit = false;
        this.canSave = false;

        this.contactComService.ToParent$.subscribe(
            (contactCom: [IContact, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editContact = contactCom[0];
                this.canSave = contactCom[1];
                // debug here
                //console.log("can save " + this.canSave);
                //console.log(JSON.stringify(this.editContact));
            });
    }
    // bulid form
    bulidForm(): void {
        this.contactForm = this.fb.group({
            "search": this.searchField
        });
        // no lazy load
        this.searchField.valueChanges
            .debounceTime(250)
            .distinctUntilChanged()
            .switchMap(term => this.contactService.getContactByFilter(term.trim()))
            .subscribe(result => {
                this.contacts = result.Data;
                this.totalRow = result.TotalRecordCount
            }, error => console.error(error));

        if (window.innerWidth >= 1600) {
            this.scrollHeight = 78 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 75 + "vh";
        } else {
            this.scrollHeight = 70 + "vh";
        }
    }
    // load data for datatable
    loadData(filter?: string) {
        this.contactService.getContactByFilter(filter).subscribe(result => {
            this.contacts = result.Data;
            this.totalRow = result.TotalRecordCount
            //debug here
            //console.log(JSON.stringify(this.contacts));
            //console.log(JSON.stringify(this.tools));
        }, error => console.error(error));

    }
    // on detail view
    onDetailView(): void {
        if (this.contact) {
            // debug here
            // console.log(JSON.stringify(this.contact));
            setTimeout(() => this.contactComService.toChild(this.contact), 1000);
        }
    }
    // on detail edit
    onDetailEdit(contact?: IContact): void {
        if (!contact) {
            contact = new Contact();
        }

        this.contact = contact;
        this.showEdit = true;
        setTimeout(() => this.contactComService.toChildEdit(this.contact), 1000);
    }
    // on cancel edit
    onCancelEdit(): void {
        this.editContact = undefined;
        this.contact = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }
    // on submit
    onSubmit(): void {
        if (this.editContact.Creator) {
            this.updateToDataBase(this.editContact);
        } else {
            this.insertToDataBase(this.editContact);
        }
    }
    // on insert data
    insertToDataBase(contact: IContact): void {
        this.contactService.postContact(contact).subscribe(
            (complete: any) => {
                this.contact = complete;
                this.saveComplete();
            },
            (error: any) => {
                this.editContact.Creator = undefined;
                console.error(error);
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on update data
    updateToDataBase(contact: IContact): void {
        this.contactService.putContact(contact.ContactId, contact).subscribe(
            (complete: any) => {
                this.contact = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on save complete
    saveComplete(): void {
        this.dialogsService
            .context("System message", "Save completed.", this.viewContainerRef)
            .subscribe(result => {
                this.canSave = false;
                this.showEdit = false;
                this.onDetailView();
                this.editContact = undefined;
                this.loadData(undefined);
            });
    }
}