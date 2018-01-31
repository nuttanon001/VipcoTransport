import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { ScheduleCenterComponent } from "../components/schedule/schedule-center.component";
import { ScheduleMasterComponent } from "../components/schedule/schedule-master.component";

const scheduleRouters: Routes = [
    {
        path: "",
        component: ScheduleCenterComponent,
        children: [
            {
                path: "",
                component: ScheduleMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(scheduleRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class ScheduleRouters { }