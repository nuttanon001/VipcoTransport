import { Injectable } from "@angular/core";
import { Http } from "@angular/http";
// classes
import { ICompany } from "../classes/index";
// seriver
import { AbstractRestService } from "./index";
@Injectable()
export class CompanyService extends AbstractRestService<ICompany>{
    constructor(http: Http) {
        super(http, "api/Company/");
    }
}