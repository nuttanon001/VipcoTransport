import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

// 3rd party
import "hammerjs";
import { AngularSplitModule } from 'angular-split';
// component
import {
    LocationCenterComponent, LocationDetailEditComponent,
    LocationDetailViewComponent, LocationMasterComponent,
    LocationDetailNoContactEditComponent
 } from "../components/location/index";
// module
import {
    CustomMaterialModule, CustomPrimeNgModule,
    DialogsModule
} from "./index";
import { LocationRouters } from "../routes/location.routing.module";

@NgModule({
    declarations: [
        LocationCenterComponent,
        LocationDetailEditComponent,
        LocationDetailViewComponent,
        LocationMasterComponent,
        LocationDetailNoContactEditComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        LocationRouters,
        DialogsModule,
        AngularSplitModule
    ],
    exports: [
        LocationDetailNoContactEditComponent
    ]
})

export class LocationModule {
}
