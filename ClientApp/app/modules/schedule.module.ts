import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

// 3rd party
import "hammerjs";
import { AngularSplitModule } from 'angular-split';
// component
import { ScheduleCenterComponent } from "../components/schedule/schedule-center.component";
import { ScheduleMasterComponent } from "../components/schedule/schedule-master.component";
import { ScheduleDailyComponent } from "../components/schedule/schedule-daily.component";
import { ScheduleWeeklyComponent } from "../components/schedule/schedule-weekly.component";
import { DetailTruckComponent } from "../components/schedule/detail-truck.component";
import { DetailComponent } from "../components/schedule/detail.component";
import { ScheduleChartBar } from "../components/schedule/schedule-chart-bar.component";
// module
import {
    CustomMaterialModule, CustomPrimeNgModule,
    TransportDataModule, TransportDataTruckModule,
    DialogsModule
} from "./index";
import { ScheduleRouters } from "../routes/schedule.routing.module";

@NgModule({
    declarations: [
        ScheduleCenterComponent,
        ScheduleMasterComponent,
        ScheduleWeeklyComponent,
        ScheduleChartBar,
        DetailTruckComponent,
        DetailComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CustomMaterialModule,
        CustomPrimeNgModule,
        ScheduleRouters,
        DialogsModule,
        AngularSplitModule,
        //TransportDataModule,
        //TransportDataTruckModule
    ]
})

export class ScheduleModule {
}
