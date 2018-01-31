import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from "@angular/router";
import { MdMenuTrigger } from "@angular/material";
// service
import { AuthenticationService } from "../../services/index";
// class
import { IUser } from "../../classes/index";

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.scss'],
})
export class NavMenuComponent implements OnInit {
    @ViewChild("trigger1") trigger1: MdMenuTrigger;
    @ViewChild("trigger2") trigger2: MdMenuTrigger;
    @ViewChild("trigger3") trigger3: MdMenuTrigger;

    constructor(
        private authService: AuthenticationService,
        private router: Router,
    ) { }

    ngOnInit(): void {
        // reset login status
        this.authService.logout();
    }

    // Propertires
    //=============================================\\
    get GetLevel3(): boolean {
        if (this.authService.getUser)
            return this.authService.getUser.Role > 2;
        else
            return false;
    }

    get GetLevel2(): boolean {
        if (this.authService.getUser)
            return this.authService.getUser.Role > 1;
        else
            return false;
    }

    get GetLevel1(): boolean {
        if (this.authService.getUser)
            return this.authService.getUser.Role > 0;
         else
            return false;
    }

    // On menu close
    //=============================================\\
    menuOnCloseMenu1(): void {
        if (!this.GetLevel2)
            return;
        this.trigger2.closeMenu();
        this.trigger3.closeMenu();
    }

    menuOnCloseMenu2(): void {
        if (!this.GetLevel2)
            return;

        this.trigger1.closeMenu();
        this.trigger3.closeMenu();
    }

    menuOnCloseMenu3(): void {
        if (!this.GetLevel2)
            return;
        this.trigger1.closeMenu();
        this.trigger2.closeMenu();
    }
    //=============================================\\
    // On menu open
    //=============================================\\
    menuOnOpenMenu1(): void {
        if (!this.GetLevel1)
            return;
        this.trigger1.openMenu();
    }

    menuOnOpenMenu2(): void {
        if (!this.GetLevel2)
            return;
        this.trigger2.openMenu();
    }

    menuOnOpenMenu3(): void {
        if (!this.GetLevel2)
            return;
        this.trigger3.openMenu();
    }
    //=============================================\\

    onLogOut(): void {
        this.authService.logout();
        this.router.navigate(["login"]);
    }
}
