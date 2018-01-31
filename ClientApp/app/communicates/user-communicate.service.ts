import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { IUser } from "../classes/index";

@Injectable()
export class UserCommunicateService {
    // Observable string sources
    private UserParentSource = new Subject<[IUser, boolean]>();
    private UserChildSource = new Subject<IUser>();
    private USerChildEditSource = new Subject<IUser>();
    // Observable string streams
    ToParent$ = this.UserParentSource.asObservable();
    ToChild$ = this.UserChildSource.asObservable();
    ToChildEdit$ = this.USerChildEditSource.asObservable();
    // Service message command
    toParent(user: [IUser, boolean]): void {
        this.UserParentSource.next(user);
    }

    toChild(user: IUser): void {
        this.UserChildSource.next(user);
    }

    toChildEdit(user: IUser): void {
        this.USerChildEditSource.next(user);
    }

}