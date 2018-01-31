import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
// 3rd party
import "hammerjs";
import { AngularSplitModule } from 'angular-split';
// component
import { CarBrandCenterConponent } from "../components/car-brand/car-brand-center.component";
import { CarBrandDetailEditComponent } from "../components/car-brand/car-brand-detail-edit.component";
import { CarBrandDetailViewComponent } from "../components/car-brand/car-brand-detail-view.component";
import { CarBrandMasterComponent } from "../components/car-brand/car-brand-master.component";
// module
import { CustomMaterialModule } from "./customer-material.module";
import { CustomPrimeNgModule } from "./customer-primeng.module";
import { CarBrandRouters } from "../routes/car-brand.routing.module";
import { DialogsModule } from "../modules/dialogs.module";

@NgModule({
    declarations: [
        CarBrandCenterConponent,
        CarBrandDetailEditComponent,
        CarBrandDetailViewComponent,
        CarBrandMasterComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        CarBrandRouters,
        DialogsModule,
        AngularSplitModule
    ]
})

export class CarBrandModule {
}
