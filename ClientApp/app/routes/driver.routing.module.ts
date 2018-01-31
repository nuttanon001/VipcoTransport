import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { DriverCenterComponent } from "../components/driver/driver-center.component";
import { DriverMasterComponent } from "../components/driver/driver-master.component";

const driverRouters: Routes = [
    {
        path: "",
        component: DriverCenterComponent,
        children: [
            {
                path: "admin/:condition",
                component: DriverMasterComponent,
            },
            {
                path: ":key",
                component: DriverMasterComponent,
            },
            {
                path: "",
                component: DriverMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(driverRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class DriverRouters { }