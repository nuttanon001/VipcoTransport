import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { EmployeeCenterComponent } from "../components/employee/employee-center.component";
import { EmployeeMasterComponent } from "../components/employee/employee-master.component";

const employeeRouters: Routes = [
    {
        path: "",
        component: EmployeeCenterComponent,
        children: [
            {
                path: ":key",
                component: EmployeeMasterComponent,
            },
            {
                path: "",
                component: EmployeeMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(employeeRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class EmployeeRouters { }