import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

// 3rd party
import "hammerjs";
import { AngularSplitModule } from 'angular-split';
// component
import { ContactCenterComponent } from "../components/contact/contact-center.component";
import { ContactDetailEditComponent } from "../components/contact/contact-detail-edit.component";
import { ContactDetailViewComponent } from "../components/contact/contact-detail-view.component";
import { ContactMasterComponent } from "../components/contact/contact-master.component";
// module
import { CustomMaterialModule } from "./customer-material.module";
import { CustomPrimeNgModule } from "./customer-primeng.module";
import { ContactRouters } from "../routes/contact.routing.module";
import { DialogsModule } from "../modules/dialogs.module";

@NgModule({
    declarations: [
        ContactCenterComponent,
        ContactDetailEditComponent,
        ContactDetailViewComponent,
        ContactMasterComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        ContactRouters,
        DialogsModule,
        AngularSplitModule
    ]
})

export class ContactModule {
}
