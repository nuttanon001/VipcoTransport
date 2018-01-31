import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

//import { AuthGuard } from "../services/auth-guard.service";


import { TransportDataCenterComponent } from "../components/transport-data/transport-data-center.component";
import { TransportDataMasterComponent } from "../components/transport-data/transport-data-master.component";

const transportDataRouters: Routes = [
    {
        path: "",
        component: TransportDataCenterComponent,
        //canLoad: [AuthGuard],
        children: [
            {
                path: "admin/:condition",
                component: TransportDataMasterComponent,
            },
            {
                path: ":key",
                component: TransportDataMasterComponent,
            },
            {
                path: "",
                component: TransportDataMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(transportDataRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class TransportDataRouters { }