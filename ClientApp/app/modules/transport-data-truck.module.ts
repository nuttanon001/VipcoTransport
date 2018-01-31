import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

// 3rd party
import "hammerjs";
import { AngularSplitModule } from "angular-split";
// component
import { TransportDataTruckCenterComponent } from "../components/transport-data-truck/transport-data-truck-center.component";
import { TransportDataTruckDetailEditComponent } from "../components/transport-data-truck/transport-data-truck-detail-edit.component";
import { TransportDataTruckDetailViewComponent } from "../components/transport-data-truck/transport-data-truck-detail-view.component";
import { TransportDataTruckMasterComponent } from "../components/transport-data-truck/transport-data-truck-master.component";
// module
import { CustomMaterialModule } from "./customer-material.module";
import { CustomPrimeNgModule } from "./customer-primeng.module";
import { TransportDataTruckRouters } from "../routes/transport-data-truck.routing.module";
import { DialogsModule } from "../modules/dialogs.module";

@NgModule({
    declarations: [
        TransportDataTruckCenterComponent,
        TransportDataTruckDetailEditComponent,
        TransportDataTruckDetailViewComponent,
        TransportDataTruckMasterComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        TransportDataTruckRouters,
        DialogsModule,
        AngularSplitModule,
    ],
    exports: [
        TransportDataTruckDetailViewComponent
    ]
})

export class TransportDataTruckModule {
}
