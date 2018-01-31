import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from "./components/home/home.component";
import { NavMenuComponent } from "./components/navmenu/navmenu.component";
import { LoginComponent } from "./components/login/login.component";


import { AuthGuard,AuthenticationService } from "./services/index";

const appRoutes: Routes = [
    {
        path: "home",
        component: HomeComponent
    },
    {
        path: "login",
        component: LoginComponent
    },
    {
        path: "car-data",
        loadChildren: "./modules/car-data.module#CarDataModule",
        canLoad: [AuthGuard]
    },
    {
        path: "trailer",
        loadChildren: "./modules/trailer.module#TrailerModule",
        canLoad: [AuthGuard]
    },
    {
        path: "type",
        loadChildren: "./modules/car-type.module#CarTypeModule",
        canLoad: [AuthGuard]
    },
    {
        path: "brand",
        loadChildren: "./modules/car-brand.module#CarBrandModule",
        canLoad: [AuthGuard]
    },
    {
        path: "employee",
        loadChildren: "./modules/employee.module#EmployeeModule",
        canLoad: [AuthGuard]
    },
    {
        path: "driver",
        loadChildren: "./modules/driver.module#DriverModule",
        canLoad: [AuthGuard]
    },
    {
        path: "location",
        loadChildren: "./modules/location.module#LocationModule",
        canLoad: [AuthGuard]
    },
    {
        path: "contact",
        loadChildren: "./modules/contact.module#ContactModule",
        canLoad: [AuthGuard]
    },
    {
        path: "transport-request",
        loadChildren: "./modules/transport-request.module#TransportRequestModule",
        canLoad: [AuthGuard]
    },
    {
        path: "transport-data",
        loadChildren: "./modules/transport-data.module#TransportDataModule",
        canLoad: [AuthGuard]
    },
    {
        path: "transport-data-truck",
        loadChildren: "./modules/transport-data-truck.module#TransportDataTruckModule",
        canLoad: [AuthGuard]
    },
    {
        path: "user",
        loadChildren: "./modules/user.module#UserModule",
        canLoad: [AuthGuard]
    },
    {
        path: "company",
        loadChildren: "./modules/company.module#CompanyModule",
        canLoad: [AuthGuard]
    },
    {
        path: "schedule",
        loadChildren: "./modules/schedule.module#ScheduleModule",
        //canLoad: [AuthGuard]
    },
    {
        path: "transport-data-report",
        loadChildren: "./modules/report.module#ReportModule",
        //canLoad: [AuthGuard]
    },
    {
        path: "full",
        redirectTo: "/home"
    },
    {
        path: "**",
        redirectTo: "/home"
    }
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule],
    providers: [AuthGuard,AuthenticationService]
})
export class AppRoutingModule { }