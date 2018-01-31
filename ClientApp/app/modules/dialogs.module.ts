import { NgModule } from "@angular/core";
import { DialogsService } from "../services/dialogs.service";

import { ConfirmDialog } from "../components/dialog/confirm-dialog.component";
import { ContextDialog } from "../components/dialog/context-dialog.component";
import { ErrorDialog } from "../components/dialog/error-dialog.component";

import { CustomMaterialModule } from "./customer-material.module";

@NgModule({
    imports: [
        CustomMaterialModule
    ],
    exports: [
        ConfirmDialog,
        ContextDialog,
        ErrorDialog
    ],
    declarations: [
        ConfirmDialog,
        ContextDialog,
        ErrorDialog
    ],
    providers: [
        DialogsService,
    ],
    entryComponents: [
        ConfirmDialog,
        ContextDialog,
        ErrorDialog
    ],
})
export class DialogsModule { }
