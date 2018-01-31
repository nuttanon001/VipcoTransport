import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

// 3rd party
import { AngularSplitModule } from 'angular-split';
// component
import { DriverCenterComponent } from "../components/driver/driver-center.component";
import { DriverDetailEditComponent } from "../components/driver/driver-detail-edit.component";
import { DriverDetailViewComponent } from "../components/driver/driver-detail-view.component";
import { DriverMasterComponent } from "../components/driver/driver-master.component";
// module
import { CustomMaterialModule } from "./customer-material.module";
import { CustomPrimeNgModule } from "./customer-primeng.module";
import { DriverRouters } from "../routes/driver.routing.module";
import { DialogsModule } from "../modules/dialogs.module";

@NgModule({
    declarations: [
        DriverCenterComponent,
        DriverDetailEditComponent,
        DriverDetailViewComponent,
        DriverMasterComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        DriverRouters,
        DialogsModule,
        AngularSplitModule
    ]
})

export class DriverModule {
}
