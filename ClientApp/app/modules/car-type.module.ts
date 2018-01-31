import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

// 3rd party
import "hammerjs";
import { AngularSplitModule } from 'angular-split';
// component
import { CarTypeCenterConponent } from "../components/car-type/car-type-center.component";
import { CarTypeDetailEditComponent } from "../components/car-type/car-type-detail-edit.component";
import { CarTypeDetailViewComponent } from "../components/car-type/car-type-detail-view.component";
import { CarTypeMasterComponent } from "../components/car-type/car-type-master.component";
// module
import { CustomMaterialModule } from "./customer-material.module";
import { CustomPrimeNgModule } from "./customer-primeng.module";
import { CarTypeRouters } from "../routes/car-type.routing.module";
import { DialogsModule } from "../modules/dialogs.module";

@NgModule({
    declarations: [
        CarTypeCenterConponent,
        CarTypeDetailEditComponent,
        CarTypeDetailViewComponent,
        CarTypeMasterComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        CarTypeRouters,
        DialogsModule,
        AngularSplitModule
    ]
})

export class CarTypeModule {
}
