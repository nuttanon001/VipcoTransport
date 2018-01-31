import { Component, ViewContainerRef } from "@angular/core";
// component
import { AbstractMasterComponent } from "../abstract/abstract-index";
// classes
import { ICompany, LazyLoad } from "../../classes/index";
import { LazyLoadEvent } from "primeng/primeng";
// service
import {
    CompanyService,DialogsService
} from "../../services/index";
// commuincate
import { CompanyCommunicateService } from "../../communicates/index"
// timezone
import * as moment from "moment-timezone";

@Component({
    selector: "company-master",
    templateUrl: "./company-master.component.html",
    styleUrls: ["../../styles/master.style.scss"],
    providers: [
        CompanyService,
        CompanyCommunicateService,
        DialogsService
    ],
})

export class CompanyMasterComponent
    extends AbstractMasterComponent<ICompany, CompanyService> {
    constructor(
        companyService: CompanyService,
        companyComService: CompanyCommunicateService,
        dialogsService: DialogsService,
        viewContainerRef: ViewContainerRef,
    ) {
        super(
            companyService,
            companyComService,
            dialogsService,
            viewContainerRef
        );
    }

    // on get data with lazy load
    onGetAllWithLazyload(lazyload: LazyLoad): void {
        if (!this.columns) {
            this.columns = [
                { field: "CompanyCode", header: "Code", style: { 'width': '15%' } },
                { field: "CompanyName", header: "Name" },
                { field: "Address1", header: "Address-1" },
                { field: "Address2", header: "Address-2" },
            ];
        }

        this.service.getAllWithLazyLoad(lazyload)
            .subscribe(restData => {
                this.values = restData.Data;
                this.totalRow = restData.TotalRow;
            }, error => console.error(error));
    }

    // on change time zone befor update to webapi
    changeTimezone(company: ICompany): ICompany {
        var zone = "Asia/Bangkok";
        if (company !== null) {
            if (company.CreateDate !== null) {
                company.CreateDate = moment.tz(company.CreateDate, zone).format();
            }
            if (company.ModifyDate !== null) {
                company.ModifyDate = moment.tz(company.ModifyDate, zone).format();
            }
        }
        return company;
    }

    // on insert data
    onInsertToDataBase(company: ICompany): void {
        // change timezone
        company = this.changeTimezone(company);
        // insert data
        this.service.post(company).subscribe(
            (complete: any) => {
                this.displayValue = complete;
                this.onSaveComplete();
            },
            (error: any) => {
                console.error(error);
                this.editValue.Creator = undefined;
                this.canSave = true;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }

    // on update data
    onUpdateToDataBase(company: ICompany): void {
        // change timezone
        company = this.changeTimezone(company);
        // update data
        this.service.putKeyNumber(company, company.CompanyId).subscribe(
            (complete: any) => {
                this.displayValue = complete;
                this.onSaveComplete();
            },
            (error: any) => {
                console.error(error);
                this.canSave = true;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
}