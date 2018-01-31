import { Component,OnInit,ViewContainerRef } from "@angular/core";
import { FormControl,FormBuilder,FormGroup } from "@angular/forms";
import { Observable } from "rxjs/Observable";
// classes
import { IUser,User } from "../../classes/index";
// service
import {
    UserService, EmployeeDataService,
    CompanyService, DialogsService,
    AuthenticationService
} from "../../services/index";
// communicate
import { UserCommunicateService } from "../../communicates/index";

@Component({
    selector: "user-master",
    templateUrl: "./user-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        UserService,
        DialogsService,
        CompanyService,
        EmployeeDataService,
        UserCommunicateService,
        AuthenticationService
    ]
})

export class UserMasterComponent implements OnInit {
    searchField: FormControl;
    userForm: FormGroup;
    users: Array<IUser>;
    user: IUser;
    editUser: IUser;

    totalRow: number;
    scrollHeight: string;
    showEdit: boolean;
    canSave: boolean;
    hideleft: boolean;
    constructor(
        private userService: UserService,
        private userComService: UserCommunicateService,
        private authService: AuthenticationService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private fb:FormBuilder
    ) { }

    get UserIsNull(): boolean {
        return this.user === undefined;
    }

    ngOnInit(): void {
        this.searchField = new FormControl();
        this.bulidForm();
        this.loadData(undefined);
        this.showEdit = false;
        this.canSave = false;
        this.hideleft = true;

        this.userComService.ToParent$.subscribe(
            (userCom: [IUser, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editUser = userCom[0];
                this.canSave = userCom[1];
            });
    }

    // bulid form
    bulidForm(): void {
        this.userForm = this.fb.group({
            search: this.searchField
        });
        // no lazy load
        this.searchField.valueChanges
            .debounceTime(250)
            .distinctUntilChanged()
            .switchMap(term => this.userService.getUserByFilter(term.trim()))
            .subscribe(result => {
                this.users = result.Data;
                this.totalRow = result.TotalRecordCount
            }, error => console.error(error));

        if (window.innerWidth >= 1600) {
            this.scrollHeight = 78 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 75 + "vh";
        } else {
            this.scrollHeight = 70 + "vh";
        }
    }
    // load data
    loadData(filter?: string) {
        this.userService.getUserByFilter(filter).subscribe(result => {
            this.users = result.Data;
            this.totalRow = result.TotalRecordCount
        }, error => console.error(error));

    }
    // on detail view
    onDetailView(): void {
        if (this.user) {
            setTimeout(() => this.userComService.toChild(this.user), 1000);
        }
    }
    // on detail edit
    onDetailEdit(user?: IUser): void {
        if (!user) {
            user = new User();
        }

        let canEdit: boolean = this.authService.getUser ? this.authService.getUser.Role > 3 : false;

        if (canEdit) {
            this.user = user;
            this.showEdit = true;
            setTimeout(() => this.userComService.toChildEdit(this.user), 1000);
        } else {
            this.dialogsService.error("Access Deny", "You don't have permission to access", this.viewContainerRef);
        }
    }
    // on cancel edit
    onCancelEdit(): void {
        this.editUser = undefined;
        this.user = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }
    // on submit
    onSubmit(): void {
        if (this.editUser.Creator) {
            this.updateToDataBase(this.editUser);
        } else {
            this.insertToDataBase(this.editUser);
        }
    }
    // on insert data
    insertToDataBase(user: IUser): void {
        this.userService.postUser(user).subscribe(
            (complete: any) => {
                this.user = complete;
                this.saveComplete();
            },
            (error: any) => {
                this.editUser.Creator = undefined;
                console.error(error);
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on update data
    updateToDataBase(user: IUser): void {
        this.userService.putUser(user.UserId, user).subscribe(
            (complete: any) => {
                this.user = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on save complete
    saveComplete(): void {
        this.dialogsService
            .context("System message", "Save completed.", this.viewContainerRef)
            .subscribe(result => {
                this.canSave = false;
                this.showEdit = false;
                this.onDetailView();
                this.editUser = undefined;
                this.loadData(undefined);
            });
    }
}
