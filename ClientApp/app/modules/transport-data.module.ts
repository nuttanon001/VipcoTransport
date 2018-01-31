import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

// 3rd party
import "hammerjs";
import { AngularSplitModule } from "angular-split";
// component
import { TransportDataCenterComponent } from "../components/transport-data/transport-data-center.component";
import { TransportDataDetailEditComponent } from "../components/transport-data/transport-data-detail-edit.component";
import { TransportDataDetailViewComponent } from "../components/transport-data/transport-data-detail-view.component";
import { TransportDataMasterComponent } from "../components/transport-data/transport-data-master.component";
// module
import { CustomMaterialModule } from "./customer-material.module";
import { CustomPrimeNgModule } from "./customer-primeng.module";
import { TransportDataRouters } from "../routes/transport-data.routing.module";
import { DialogsModule } from "../modules/dialogs.module";

@NgModule({
    declarations: [
        TransportDataCenterComponent,
        TransportDataDetailEditComponent,
        TransportDataDetailViewComponent,
        TransportDataMasterComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        TransportDataRouters,
        DialogsModule,
        AngularSplitModule,
    ],
    exports: [
        TransportDataDetailViewComponent,
    ]
})

export class TransportDataModule {
}
