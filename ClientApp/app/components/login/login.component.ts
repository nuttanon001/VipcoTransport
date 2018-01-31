import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormBuilder, FormGroup, FormControl, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { Observable } from "rxjs/Observable";

// services
import { AuthenticationService, DialogsService, UserService } from "../../services/index";
// classes
import { User, IUser } from "../../classes/user.classs";

@Component({
    templateUrl: "./login.component.html",
    styleUrls: [ "./login.component.scss" ],
    providers: [DialogsService, UserService ]
})

export class LoginComponent implements OnInit {
    user: IUser;
    loginForm: FormGroup;

    // form validators error
    formErrors = {
        Username: "",
        Password: "",
    };
    validationMessages = {
        "Username": {
            "required": "Username is required",
            "minlength": "Username must be at least 1 characters long",
        },
        "Password": {
            "required": "Password is required",
            "minlength": "Password must be at least 1 characters long",
        },
    }

    constructor(
        private authService: AuthenticationService,
        private userService: UserService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private fb: FormBuilder,
        private router: Router,
    ) { }

    ngOnInit(): void {
        this.user = new User();
        this.buildForm();
    }

    // build form
    buildForm(): void {
        this.loginForm = this.fb.group({
            Username: [this.user.Username,
                [
                    Validators.required,
                    Validators.minLength(1),
                ]
            ],
            Password: [this.user.Password,
                [

                    Validators.required,
                    Validators.minLength(1),
                ]
            ]
        });
        this.loginForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation message now
    }

    // on value of form change
    onValueChanged(data?: any): void {
        if (!this.loginForm) { return; }
        const form = this.loginForm;
        for (const field in this.formErrors) {
            // clear previous error message (if any)
            this.formErrors[field] = "";
            const control = form.get(field);
            if (control && control.dirty && !control.valid) {
                const messages = this.validationMessages[field];

                for (const key in control.errors) {
                    this.formErrors[field] += messages[key] + " ";
                }
            }
        }
    }

    /*setMessage(): void {
        // debug here
        console.log("login..." + this.message);
        this.message = `Logged ${this.authService.isLoggedIn ? "in" : "out"}`
    }*/

    onLogin(): void {
        /*this.message = "Trying to log in... username :" + this.user.Username + " password :" + this.user.Password;
        // debug here
        console.log("login..." + this.message);*/
        this.user = this.loginForm.value;
        let username = this.user.Username;
        let password = this.user.Password;
        this.authService.login(username, password)
            .subscribe((data) => {
                // login successful
                // no more alert Token
                // alert("this.authService.redirectUrl : " + this.authService.redirectUrl);
                this.userService.getUsers().subscribe(dbuser => {
                    localStorage.removeItem("userData");
                    localStorage.setItem("userData", JSON.stringify(dbuser));

                    // debug here
                    //console.log(JSON.stringify(dbuser));

                    let redirect = this.authService.redirectUrl ? this.authService.redirectUrl : "/home";
                    this.router.navigate([redirect]);
                }, error => {
                    console.error(error);
                    this.dialogsService.error("Login failure", "Warning : Username or Password mismatch !!!", this.viewContainerRef)
                });
            },
            (err) => {
                console.error(err);
                // login failure
                //this.loginError = true;
                this.dialogsService.error("Login failure", "Warning : Username or Password mismatch !!!", this.viewContainerRef)
            });
    }

    logout(): void {
        this.authService.logout();
        /*this.setMessage();*/
    }
}