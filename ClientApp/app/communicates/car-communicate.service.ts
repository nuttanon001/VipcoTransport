import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { ICarData } from "../classes/car-data.class";

@Injectable()
export class CarDataCommunicateService {
    // Observable string sources
    private CarDataParentSource = new Subject<[ICarData,boolean]>();
    private CarDataChildSource = new Subject<ICarData>();
    private CarDataEditChileSource = new Subject<ICarData>();
    // Observable string streams
    ToParent$ = this.CarDataParentSource.asObservable();
    ToChild$ = this.CarDataChildSource.asObservable();
    toChildEdit$ = this.CarDataEditChileSource.asObservable();
    // Service message commands
    toParent(carData: [ICarData,boolean]): void {
        this.CarDataParentSource.next(carData);
    }
    toChild(carData: ICarData): void {
        this.CarDataChildSource.next(carData);
    }
    toChildEdit(carDataEdit: ICarData): void {
        this.CarDataEditChileSource.next(carDataEdit);
    }
}