import { NgModule } from "@angular/core";
import { FormsModule,ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
// 3rd party
import "hammerjs";
import { AngularSplitModule } from "angular-split";
// componentes
import {
    UserCenterComponent, UserDetailEditComponent,
    UserDetailViewComponent, UserMasterComponent,
    UserConfigComponent
} from "../components/user/index";
// modules
import {
    CustomMaterialModule, CustomPrimeNgModule,
    DialogsModule,
} from "./index";
import { UserRouters } from "../routes/index";

@NgModule({
    declarations: [
        UserCenterComponent,
        UserMasterComponent,
        UserDetailEditComponent,
        UserDetailViewComponent,
        UserConfigComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        UserRouters,
        DialogsModule,
        AngularSplitModule
    ]
})

export class UserModule {

}