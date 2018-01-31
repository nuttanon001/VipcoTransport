import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { IContact } from "../../classes/contact.class";
// service
import { EmployeeDataService } from "../../services/employee.service";
import { ContactCommunicateService } from "../../communicates/contact-communicate.service"

import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "contact-detail-view",
    templateUrl: "./contact-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})

export class ContactDetailViewComponent implements OnInit, OnDestroy {
    contact: IContact;
    subscription: Subscription;

    constructor(
        private contactComService: ContactCommunicateService,
        private contactService: EmployeeDataService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.contactComService.ToChild$.subscribe(
            (contact: IContact) => {
                // debug here
                // console.log(JSON.stringify(contact));
                this.contactService.getContactByKey(contact.ContactId)
                    .subscribe(dbContact => this.contact = dbContact, error => console.error(error));
            });
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}
