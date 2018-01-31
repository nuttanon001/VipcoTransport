import { NgModule } from "@angular/core";
import {
    DataTableModule,
    DialogModule,
    SharedModule,
    CalendarModule,
    DropdownModule,
    InputMaskModule
} from "primeng/primeng";


@NgModule({
    imports: [
        DataTableModule,
        DialogModule,
        SharedModule,
        CalendarModule,
        DropdownModule,
        InputMaskModule
    ],
    exports: [
        DataTableModule,
        DialogModule,
        SharedModule,
        CalendarModule,
        DropdownModule,
        InputMaskModule
    ],
})
export class CustomPrimeNgModule { }