import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { CarTypeCenterConponent } from "../components/car-type/car-type-center.component";
import { CarTypeMasterComponent } from "../components/car-type/car-type-master.component";

const typeRouters: Routes = [
    {
        path: "",
        component: CarTypeCenterConponent,
        children: [
            {
                path: ":key",
                component: CarTypeMasterComponent,
            },
            {
                path: "",
                component: CarTypeMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(typeRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class CarTypeRouters { }