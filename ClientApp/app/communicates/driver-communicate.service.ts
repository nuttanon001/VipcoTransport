import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { IDriver } from "../classes/driver.class";

@Injectable()
export class DriverCommunicateService {
    // Observable string sources
    private DriverParentSource = new Subject<[IDriver, boolean]>();
    private DriverChildSource = new Subject<IDriver>();
    private DriverEditChileSource = new Subject<IDriver>();
    // Observable string streams
    ToParent$ = this.DriverParentSource.asObservable();
    ToChild$ = this.DriverChildSource.asObservable();
    toChildEdit$ = this.DriverEditChileSource.asObservable();
    // Service message commands
    toParent(driver: [IDriver, boolean]): void {
        this.DriverParentSource.next(driver);
    }
    toChild(driver: IDriver): void {
        this.DriverChildSource.next(driver);
    }
    toChildEdit(driverEdit: IDriver): void {
        this.DriverEditChileSource.next(driverEdit);
    }
}