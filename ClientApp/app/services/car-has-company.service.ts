import { Injectable } from "@angular/core";
import { Http } from "@angular/http";
// classes
import { ICarHasCompany } from "../classes/index";
// seriver
import { AbstractRestService } from "./index";
@Injectable()
export class CarHasCompanyService extends AbstractRestService<ICarHasCompany>{
    constructor(http: Http) {
        super(http, "api/CarHasCompany/");
    }
}