import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

//import { AuthGuard } from "../services/auth-guard.service";
//import { AuthService } from "../services/auth.service";

import { TransportDataTruckCenterComponent } from "../components/transport-data-truck/transport-data-truck-center.component";
import { TransportDataTruckMasterComponent } from "../components/transport-data-truck/transport-data-truck-master.component";

const transportDataTruckRouters: Routes = [
    {
        path: "",
        component: TransportDataTruckCenterComponent,
        //canLoad: [AuthGuard],
        children: [
            {
                path: "admin/:condition",
                component: TransportDataTruckMasterComponent,
            },
            {
                path: ":key",
                component: TransportDataTruckMasterComponent,
                //canLoad: [AuthGuard],
            },
            {
                path: "",
                component: TransportDataTruckMasterComponent,
                //canLoad: [AuthGuard],
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(transportDataTruckRouters)
    ],
    exports: [
        RouterModule
    ],
    //providers: [AuthGuard, AuthService]
})

export class TransportDataTruckRouters { }