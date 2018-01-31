import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

// 3rd party
import "hammerjs";
import { AngularSplitModule } from 'angular-split';
// component
import {
    TransportRequestCenterComponent, TransportRequestDetailEditComponent,
    TransportRequestDetailViewComponent, TransportRequestMasterComponent,
    TransportRequestWaitingComponent
 } from "../components/transport-request/index";
// module
import {
    CustomMaterialModule, CustomPrimeNgModule,
    TransportDataModule, TransportDataTruckModule,
    DialogsModule, LocationModule
} from "./index";
import { TransportRequestRouters } from "../routes/transport-request.routing.module";

@NgModule({
    declarations: [
        TransportRequestCenterComponent,
        TransportRequestDetailEditComponent,
        TransportRequestDetailViewComponent,
        TransportRequestMasterComponent,
        TransportRequestWaitingComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        TransportRequestRouters,
        TransportDataModule,
        TransportDataTruckModule,
        LocationModule,
        DialogsModule,
        AngularSplitModule
    ]
})

export class TransportRequestModule {
}
