import { Injectable } from "@angular/core";
import { Http } from "@angular/http";
// classes
import { ITranHasCompany } from "../classes/index";
// seriver
import { AbstractRestService } from "./index";
@Injectable()
export class TranHasCompanyService extends AbstractRestService<ITranHasCompany>{
    constructor(http: Http) {
        super(http, "api/TranHasCompany/");
    }
}