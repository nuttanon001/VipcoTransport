import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { CarDataCenterComponent } from "../components/car-data/car-data-center.component";
import { CarDataMasterComponent } from "../components/car-data/car-data-master.component";

const carDataRouters: Routes = [
    {
        path: "",
        component: CarDataCenterComponent,
        children: [
            {
                path: "admin/:condition",
                component: CarDataMasterComponent,
            },
            {
                path: ":key",
                component: CarDataMasterComponent,
            },
            {
                path: "",
                component: CarDataMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(carDataRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class CarDataRouters { }