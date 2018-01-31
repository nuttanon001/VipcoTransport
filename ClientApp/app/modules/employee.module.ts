import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

// 3rd party
import "hammerjs";
import { AngularSplitModule } from 'angular-split';
// component
import { EmployeeCenterComponent } from "../components/employee/employee-center.component";
import { EmployeeDetailEditComponent } from "../components/employee/employee-detail-edit.component";
import { EmployeeDetailViewComponent } from "../components/employee/employee-detail-view.component";
import { EmployeeMasterComponent } from "../components/employee/employee-master.component";
// module
import { CustomMaterialModule } from "./customer-material.module";
import { CustomPrimeNgModule } from "./customer-primeng.module";
import { EmployeeRouters } from "../routes/employee.routing.module";
import { DialogsModule } from "../modules/dialogs.module";

@NgModule({
    declarations: [
        EmployeeCenterComponent,
        EmployeeDetailEditComponent,
        EmployeeDetailViewComponent,
        EmployeeMasterComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        EmployeeRouters,
        DialogsModule,
        AngularSplitModule
    ]
})

export class EmployeeModule {
}
