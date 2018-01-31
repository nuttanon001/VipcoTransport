import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { LocationCenterComponent } from "../components/location/location-center.component";
import { LocationMasterComponent } from "../components/location/location-master.component";

const locationRouters: Routes = [
    {
        path: "",
        component: LocationCenterComponent,
        children: [
            {
                path: ":key",
                component: LocationMasterComponent,
            },
            {
                path: "",
                component: LocationMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(locationRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class LocationRouters { }