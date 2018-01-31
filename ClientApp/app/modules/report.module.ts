import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
// 3rd party
import "hammerjs";
// component
import {
    ReportCenterComponent, ReportMasterComponent
} from "../components/reports/index";
// module
import {
    CustomMaterialModule, CustomPrimeNgModule,
    TransportDataModule, TransportDataTruckModule
} from "./index";
// routes
import { ReportRouters } from "../routes/index";

@NgModule({
    declarations: [
        ReportCenterComponent,
        ReportMasterComponent,
    ],
    imports: [
        CommonModule,
        ReportRouters,
        CustomPrimeNgModule,
        TransportDataModule,
        CustomMaterialModule,
        TransportDataTruckModule
    ]
})

export class ReportModule {
}
