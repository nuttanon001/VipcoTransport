import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { MaterialModule } from "@angular/material";

// 3rd party
import "hammerjs";
import { AngularSplitModule } from 'angular-split';
// component
import { CarDataCenterComponent } from "../components/car-data/car-data-center.component";
import { CarDataDetailEditComponent } from "../components/car-data/car-data-detail-edit.component";
import { CarDataDetailViewComponent } from "../components/car-data/car-data-detail-view.component";
import { CarDataMasterComponent } from "../components/car-data/car-data-master.component";
// module
import { CustomMaterialModule } from "./customer-material.module";
import { CustomPrimeNgModule } from "./customer-primeng.module";
import { CarDataRouters } from "../routes/car-data.routing.module";
import { DialogsModule } from "../modules/dialogs.module";

@NgModule({
    declarations: [
        CarDataCenterComponent,
        CarDataDetailEditComponent,
        CarDataDetailViewComponent,
        CarDataMasterComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        CarDataRouters,
        DialogsModule,
        AngularSplitModule
    ]
})

export class CarDataModule {
}
