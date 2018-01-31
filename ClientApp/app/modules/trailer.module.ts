import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

// 3rd party
import "hammerjs";
import { AngularSplitModule } from 'angular-split';
// component
import { TrailerCenterComponent } from "../components/trailer/trailer-center.component";
import { TrailerDetailEditComponent } from "../components/trailer/trailer-detail-edit.component";
import { TrailerDetailViewComponent } from "../components/trailer/trailer-detail-view.component";
import { TrailerMasterComponent } from "../components/trailer/trailer-master.component";
// module
import { CustomMaterialModule } from "./customer-material.module";
import { CustomPrimeNgModule } from "./customer-primeng.module";
import { TrailerRouters } from "../routes/trailer.routing.module";
import { DialogsModule } from "../modules/dialogs.module";

@NgModule({
    declarations: [
        TrailerCenterComponent,
        TrailerDetailEditComponent,
        TrailerDetailViewComponent,
        TrailerMasterComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        TrailerRouters,
        DialogsModule,
        AngularSplitModule
    ]
})

export class TrailerModule {
}
