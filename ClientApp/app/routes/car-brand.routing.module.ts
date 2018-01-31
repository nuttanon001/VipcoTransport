import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { CarBrandCenterConponent } from "../components/car-brand/car-brand-center.component";
import { CarBrandMasterComponent } from "../components/car-brand/car-brand-master.component";

const brandRouters: Routes = [
    {
        path: "",
        component: CarBrandCenterConponent,
        children: [
            {
                path: ":key",
                component: CarBrandMasterComponent,
            },
            {
                path: "",
                component: CarBrandMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(brandRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class CarBrandRouters { }