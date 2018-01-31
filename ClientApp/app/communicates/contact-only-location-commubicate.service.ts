import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { IContact,ILocation } from "../classes/index";

@Injectable()
export class ContactOnlyLocationCommunicateService {
    // Observable string sources
    private ContactParentSource = new Subject<[IContact, boolean]>();
    private ContactEditChileSource = new Subject<ILocation>();
    // Observable string streams
    ToParent$ = this.ContactParentSource.asObservable();
    toChildEdit$ = this.ContactEditChileSource.asObservable();
    // Service message commands
    toParent(contact: [IContact, boolean]): void {
        this.ContactParentSource.next(contact);
    }
    toChildEdit(locationEdit: ILocation): void {
        this.ContactEditChileSource.next(locationEdit);
    }
}