import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
// 3rd party
import 'hammerjs';
// component
import { AppComponent } from "./components/app/app.component"
import { HomeComponent } from "./components/home/home.component";
import { NavMenuComponent } from "./components/navmenu/navmenu.component";
import { LoginComponent } from "./components/login/login.component";
// router
import { AppRoutingModule } from "./app.routing.module";
import { CustomMaterialModule } from "./modules/customer-material.module";
// module
import { DialogsModule } from "./modules/dialogs.module";
import { ScheduleModule } from "./modules/schedule.module";

export const sharedConfig: NgModule = {
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        LoginComponent,
        HomeComponent
    ],
    imports: [
        RouterModule,
        DialogsModule,
        AppRoutingModule,
        CustomMaterialModule
    ]
};
