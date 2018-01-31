import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import {
    ReportCenterComponent, ReportMasterComponent
} from "../components/reports/index";

const reportRouters: Routes = [
    {
        path: "",
        component: ReportCenterComponent,
        children: [
            {
                path: ":key",
                component: ReportMasterComponent,
            },
            {
                path: "",
                component: ReportMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(reportRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class ReportRouters { }