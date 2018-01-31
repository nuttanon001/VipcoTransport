import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { TrailerCenterComponent } from "../components/trailer/trailer-center.component";
import { TrailerMasterComponent } from "../components/trailer/trailer-master.component";

const trailerRouters: Routes = [
    {
        path: "",
        component: TrailerCenterComponent,
        children: [
            {
                path: "admin/:condition",
                component: TrailerMasterComponent,
            },
            {
                path: ":key",
                component: TrailerMasterComponent,
            },
            {
                path: "",
                component: TrailerMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(trailerRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class TrailerRouters { }