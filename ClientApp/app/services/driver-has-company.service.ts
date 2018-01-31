import { Injectable } from "@angular/core";
import { Http } from "@angular/http";
// classes
import { IDriverHasCompany } from "../classes/index";
// seriver
import { AbstractRestService } from "./index";
@Injectable()
export class DriverHasCompanyService extends AbstractRestService<IDriverHasCompany>{
    constructor(http: Http) {
        super(http, "api/DriverHasCompany/");
    }
}