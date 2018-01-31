import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { IContact } from "../classes/contact.class";

@Injectable()
export class ContactCommunicateService {
    // Observable string sources
    private ContactParentSource = new Subject<[IContact, boolean]>();
    private ContactChildSource = new Subject<IContact>();
    private ContactEditChileSource = new Subject<IContact>();
    // Observable string streams
    ToParent$ = this.ContactParentSource.asObservable();
    ToChild$ = this.ContactChildSource.asObservable();
    toChildEdit$ = this.ContactEditChileSource.asObservable();
    // Service message commands
    toParent(contact: [IContact, boolean]): void {
        this.ContactParentSource.next(contact);
    }
    toChild(contact: IContact): void {
        this.ContactChildSource.next(contact);
    }
    toChildEdit(contactEdit: IContact): void {
        this.ContactEditChileSource.next(contactEdit);
    }
}