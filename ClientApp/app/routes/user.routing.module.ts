import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
// componentes
import {
    UserCenterComponent, UserMasterComponent,
    UserConfigComponent
} from "../components/user/index";

const userRouters: Routes = [
    {
        path: "",
        component: UserCenterComponent,
        children: [
            {
                path: "userconfig",
                component: UserConfigComponent,
            },
            {
                path: ":key",
                component: UserMasterComponent,
            },
            {
                path: "",
                component: UserMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(userRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class UserRouters { }