import { Injectable } from "@angular/core";
import { Http } from "@angular/http";
// classes
import { IReqHasCompany } from "../classes/index";
// seriver
import { AbstractRestService } from "./index";
@Injectable()
export class ReqHasCompanyService extends AbstractRestService<IReqHasCompany>{
    constructor(http: Http) {
        super(http, "api/ReqHasCompany/");
    }
}