import { Injectable } from "@angular/core";
import { Http } from "@angular/http";
// classes
import { IUserHasCompany } from "../classes/index";
// seriver
import { AbstractRestService } from "./index";
@Injectable()
export class UserHasCompanyService extends AbstractRestService<IUserHasCompany>{
    constructor(http: Http) {
        super(http, "api/UserHasCompany/");
    }
}