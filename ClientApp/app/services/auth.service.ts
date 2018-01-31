import { Injectable } from "@angular/core";
import { Http, Headers, Response, RequestOptions } from "@angular/http";
import { Observable } from "rxjs/Observable";
import 'rxjs/Rx';

import { IUser } from "../classes/user.classs";

@Injectable()
export class AuthenticationService  {
    // store the URL so we can redirect after logging in
    redirectUrl: string;
    public token: string;
    constructor(private http: Http) {
        // set token if saved in local storage
        let currentUser = JSON.parse(localStorage.getItem("currentUser"));
        this.token = currentUser && currentUser.token;
    }

    // Login
    login(username: string, password: string): Observable<boolean> {
        //console.log("login service....");

        let data = {
            Username: username,
            Password: password
        };

        // console.log(data);
        return this.http.post("api/Jwt", this.toUrlEncodedString(data)
            , new RequestOptions({
                headers: new Headers({
                    "Content-Type": "application/x-www-form-urlencoded"
                })
            }))
            .map((response: Response) => {
                let token = response.json() && response.json().token;
                let expires_in = response.json().expires_in;

                //debug here
                // console.log("Token is :" + JSON.stringify(token));
                // console.log(JSON.stringify(new Date(expires_in)));

                if (token) {
                    // set token property
                    this.token = token;
                    // store username and jwt token in local storage to keep user logged in between page refreshes
                    localStorage.setItem("currentUser", JSON.stringify({ username: username, token: token, expires :new Date(expires_in)}));

                    // return true to indicate successful login
                    return true;
                } else {
                    // return false to indicate failed login
                    return false;
                }
            });
    }

    // Logout
    logout(): void {
        // clear token remove user from local storage to log user out
        this.token = null;
        localStorage.removeItem("currentUser");
        localStorage.removeItem("userData");
    }

    // for error message
    private handleError(error: Response | any) {
        // In a real world app, we might use a remote logging infrastructure
        let errMsg: string;
        if (error instanceof Response) {
            const body = error.json() || '';
            const err = body.error || JSON.stringify(body);
            errMsg = error.status + " - " + (error.statusText || '') + err;
        } else {
            errMsg = error.message ? error.message : error.toString();
        }
        console.error(errMsg);
        return Observable.throw(errMsg);
    }

    private getRequestOption(): RequestOptions {   // for request option
        return new RequestOptions({
            headers: new Headers({
                "Content-Type": "application/json"
            })
        });
    }

    // Converts a Json object to urlencoded format
    toUrlEncodedString(data: any) {
        let body = "";
        for (let key in data) {
            if (body.length) {
                body += "&";
            }
            body += key + "=";
            body += encodeURIComponent(data[key]);
        }
        return body;
    }

    // get user name from localStorage
    get userName(): string {
        let userName = localStorage.getItem("currentUser");
        if (userName) {
            return JSON.parse(userName).username;
        }
        else {
            return null;
        }
    }

    get getUser(): IUser {
        let user = localStorage.getItem("userData");
        if (user)
            return JSON.parse(user);
        else
            return null;
    }

    get isLoggedIn(): boolean {
        if (localStorage.getItem("currentUser")) {
            let currentUser = JSON.parse(localStorage.getItem("currentUser"));
            let expires_in = new Date(currentUser && currentUser.expires);
            let currentDate = new Date();

            // check time
            if (currentDate > expires_in) {
                this.logout();
                return false;
            }
            else
                return true;
        }
        else
            return false;
    }
}