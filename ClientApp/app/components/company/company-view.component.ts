// angular
import { Component } from "@angular/core";
// class
import { ICompany } from "../../classes/index";
// component
import { AbstractViewComponent } from "../abstract/abstract-index";
// service
import { CompanyService } from "../../services/index";
import { CompanyCommunicateService } from "../../communicates/index"
// rxjs
import { Subscription } from "rxjs/Subscription";

@Component({
    selector: "company-view",
    templateUrl: "./company-view.component.html",
    styleUrls: ["../../styles/view.style.scss"],
})

export class CompanyViewComponent
    extends AbstractViewComponent<ICompany, CompanyService> {

    constructor(
        companyService: CompanyService,
        companyComService: CompanyCommunicateService,
    ) {
        super(
            companyService,
            companyComService,
        );
    }

    // on get data by key
    onGetDataByKey(company: ICompany): void {
        // clear data
        this.displayValue = undefined;
        this.service.getOneKeyNumber(company.CompanyId)
            .subscribe(dbEducation => {
                this.displayValue = dbEducation;
            }, error => console.error(error));
    }
}