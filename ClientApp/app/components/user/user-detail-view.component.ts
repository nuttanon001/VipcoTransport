import { Component, OnInit, OnDestroy } from "@angular/core";
// classes
import { IUser,ICompany } from "../../classes/index";
// service
import { UserService,CompanyService } from "../../services/index";
import { UserCommunicateService } from "../../communicates/index";
// rxjs
import { Subscription } from "rxjs/Subscription";

@Component({
    selector: "user-detail-view",
    templateUrl: "./user-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})

export class UserDetailViewComponent implements OnInit, OnDestroy {
    user: IUser;
    company: ICompany;
    subScription: Subscription;

    constructor(
        private userService: UserService,
        private companyService: CompanyService,
        private userComService: UserCommunicateService
    ) {}

    get RoleString(): string {
        if (this.user) {
            if (this.user.Role == 1)
                return "Requirement";
            else if (this.user.Role == 2)
                return "Standerd";
            else if (this.user.Role == 4)
                return "TopLevel";
            else
                return "Administrator";
        } else
            return "Anonymous";

    }

    ngOnInit(): void {
        this.subScription = this.userComService.ToChild$.subscribe(
            (user: IUser) => {
                this.userService.getUserByKey(user.UserId)
                    .subscribe(dbUser => {
                        this.user = dbUser;
                        if (this.user.CompanyId) {
                            this.companyService.getOneKeyNumber(this.user.CompanyId)
                                .subscribe(dbCompany => {
                                    this.company = dbCompany;
                                });
                        }
                    },error => console.error(error));
            });
    }

    ngOnDestroy(): void {
        this.subScription.unsubscribe();
    }
}