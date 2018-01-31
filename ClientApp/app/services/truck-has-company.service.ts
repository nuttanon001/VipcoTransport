import { Injectable } from "@angular/core";
import { Http } from "@angular/http";
// classes
import { ITruckHasCompany } from "../classes/index";
// seriver
import { AbstractRestService } from "./index";
@Injectable()
export class TruckHasCompanyService extends AbstractRestService<ITruckHasCompany>{
    constructor(http: Http) {
        super(http, "api/TruckHasCompany/");
    }
}