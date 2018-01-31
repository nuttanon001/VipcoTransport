import { Injectable } from "@angular/core";
import { Http } from "@angular/http";
// classes
import { IUserEdition } from "../classes/index";
// seriver
import { AbstractRestService } from "./index";
@Injectable()
export class UserEditionService extends AbstractRestService<IUserEdition>{
    constructor(http: Http) {
        super(http, "api/UserEdition/");
    }
}