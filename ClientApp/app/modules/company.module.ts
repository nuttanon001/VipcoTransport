import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
// 3rd party
import { AngularSplitModule } from 'angular-split';
// component
import {
    CompanyCenterComponent, CompanyEditComponent,
    CompanyMasterComponent, CompanyViewComponent
} from "../components/company/company-index";
// module
import {
    CustomMaterialModule, CustomPrimeNgModule,
    DialogsModule
} from "./index";
import { CompanyRouters } from "../routes/index";

@NgModule({
    declarations: [
        CompanyCenterComponent,
        CompanyEditComponent,
        CompanyViewComponent,
        CompanyMasterComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        CompanyRouters,
        DialogsModule,
        AngularSplitModule
    ]
})

export class CompanyModule {
}
