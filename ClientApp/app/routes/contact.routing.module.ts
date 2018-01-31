import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { ContactCenterComponent } from "../components/contact/contact-center.component";
import { ContactMasterComponent } from "../components/contact/contact-master.component";

const contactRouters: Routes = [
    {
        path: "",
        component: ContactCenterComponent,
        children: [
            {
                path: ":key",
                component: ContactMasterComponent,
            },
            {
                path: "",
                component: ContactMasterComponent,
            }
        ]
    }
]

@NgModule({
    imports: [
        RouterModule.forChild(contactRouters)
    ],
    exports: [
        RouterModule
    ]
})

export class ContactRouters { }