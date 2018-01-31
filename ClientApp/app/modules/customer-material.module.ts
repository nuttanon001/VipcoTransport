import { NgModule } from "@angular/core";
import {
    MdButtonModule,
    MdCheckboxModule,
    MdProgressBarModule,
    MdTooltipModule,
    MdSidenavModule,
    MdInputModule,
    MdIconModule,
    MdMenuModule,
    MdDialogModule,
    MdTabsModule,
    MdRadioModule
} from "@angular/material";

@NgModule({
    imports: [
        MdButtonModule,
        MdCheckboxModule,
        MdProgressBarModule,
        MdTooltipModule,
        MdSidenavModule,
        MdMenuModule,
        MdInputModule,
        MdIconModule,
        MdDialogModule,
        MdTabsModule,
        MdRadioModule,
    ],
    exports: [
        MdButtonModule,
        MdCheckboxModule,
        MdProgressBarModule,
        MdTooltipModule,
        MdSidenavModule,
        MdMenuModule,
        MdInputModule,
        MdIconModule,
        MdDialogModule,
        MdTabsModule,
        MdRadioModule,
    ],
})
export class CustomMaterialModule { }