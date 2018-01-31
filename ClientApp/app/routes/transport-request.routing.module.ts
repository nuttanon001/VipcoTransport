import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { TransportRequestCenterComponent } from "../components/transport-request/transport-request-center.component";
import { TransportRequestMasterComponent } from "../components/transport-request/transport-request-master.component";
import { TransportRequestWaitingComponent } from "../components/transport-request/transport-request-waiting.component";

const transportRequestRouters: Routes = [
    {

        path: "",
        component: TransportRequestCenterComponent,
        children: [
            {
                path: "request-waiting",
                component: TransportRequestWaitingComponent,
            },
            {
                path: "admin/:condition",
                component: TransportRequestMasterComponent,
            },
            {
                path: ":key",
                component: TransportRequestMasterComponent,
            },
            {
                path: "",
                component: TransportRequestMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(transportRequestRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class TransportRequestRouters { }