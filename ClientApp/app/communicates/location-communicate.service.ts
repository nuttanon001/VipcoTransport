import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { ILocation } from "../classes/location.class";

@Injectable()
export class LocationCommunicateService {
    // Observable string sources
    private LocationParentSource = new Subject<[ILocation, boolean]>();
    private LocationChildSource = new Subject<ILocation>();
    private LocationEditChileSource = new Subject<ILocation>();
    // Observable string streams
    ToParent$ = this.LocationParentSource.asObservable();
    ToChild$ = this.LocationChildSource.asObservable();
    toChildEdit$ = this.LocationEditChileSource.asObservable();
    // Service message commands
    toParent(location: [ILocation, boolean]): void {
        this.LocationParentSource.next(location);
    }
    toChild(location: ILocation): void {
        this.LocationChildSource.next(location);
    }
    toChildEdit(locationEdit: ILocation): void {
        this.LocationEditChileSource.next(locationEdit);
    }
}