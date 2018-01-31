import { Injectable } from "@angular/core";
// classes
import { ICompany } from "../classes/index";
// services
import { AbstractCommunicateService } from "./index";

@Injectable()
export class CompanyCommunicateService extends AbstractCommunicateService<ICompany> {
    constructor() {
        super();
    }
}