import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { CompanyCenterComponent,CompanyMasterComponent } from "../components/company/company-index";

const companyRouters: Routes = [
    {
        path: "",
        component: CompanyCenterComponent,
        children: [
            {
                path: ":key",
                component: CompanyMasterComponent,
            },
            {
                path: "",
                component: CompanyMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(companyRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class CompanyRouters { }