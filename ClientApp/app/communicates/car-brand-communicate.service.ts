import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { ICarBrand } from "../classes/car-brand.class";

@Injectable()
export class CarBrandCommunicateService {
    // Observable string sources
    private CarBrandParentSource = new Subject<[ICarBrand, boolean]>();
    private CarBrandChildSource = new Subject<ICarBrand>();
    private CarBrandEditChileSource = new Subject<ICarBrand>();
    // Observable string streams
    ToParent$ = this.CarBrandParentSource.asObservable();
    ToChild$ = this.CarBrandChildSource.asObservable();
    toChildEdit$ = this.CarBrandEditChileSource.asObservable();
    // Service message commands
    toParent(carBrand: [ICarBrand, boolean]): void {
        this.CarBrandParentSource.next(carBrand);
    }
    toChild(carBrand: ICarBrand): void {
        this.CarBrandChildSource.next(carBrand);
    }
    toChildEdit(carBrandEdit: ICarBrand): void {
        this.CarBrandEditChileSource.next(carBrandEdit);
    }
}