import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { ICarType } from "../classes/car-type.class";

@Injectable()
export class CarTypeCommunicateService {
    // Observable string sources
    private CarTypeParentSource = new Subject<[ICarType, boolean]>();
    private CarTypeChildSource = new Subject<ICarType>();
    private CarTypeEditChileSource = new Subject<ICarType>();
    // Observable string streams
    ToParent$ = this.CarTypeParentSource.asObservable();
    ToChild$ = this.CarTypeChildSource.asObservable();
    toChildEdit$ = this.CarTypeEditChileSource.asObservable();
    // Service message commands
    toParent(carType: [ICarType, boolean]): void {
        this.CarTypeParentSource.next(carType);
    }
    toChild(carType: ICarType): void {
        this.CarTypeChildSource.next(carType);
    }
    toChildEdit(carTypeEdit: ICarType): void {
        this.CarTypeEditChileSource.next(carTypeEdit);
    }
}