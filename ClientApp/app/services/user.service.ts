import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'

import { AuthenticationService } from "../services/index";
import { IUser } from "../classes/index";

@Injectable()
export class UserService {
    constructor(
        private http: Http,
        private authenticationService: AuthenticationService) {
    }
    // base url
    private baseUrl: string = "api/Employee/";
    // extract data
    private extractData(r: Response) { // for extractdata
        let body = r.json();
        // console.log(body);
        return body || {};
    }
    // handle error
    private handleError(error: Response | any) {// for error message
        // In a real world app, we might use a remote logging infrastructure
        let errMsg: string;
        if (error instanceof Response) {
            const body = error.json() || '';
            const err = body.error || JSON.stringify(body);
            errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
        } else {
            errMsg = error.message ? error.message : error.toString();
        }
        console.error(errMsg);
        return Observable.throw(errMsg);
    }
    // for request option
    private getRequestOption(): RequestOptions {
        return new RequestOptions({
            headers: new Headers({
                "Content-Type": "application/json"
            })
        });
    }

    //===================== User ==================================\\
    // get user
    getUsers(): Observable<IUser> {
        let url: string = this.baseUrl + "GetUser/" + this.authenticationService.userName;
        // get users from api
        let headers = new Headers({ "Authorization": "Bearer " + this.authenticationService.token });
        let options = new RequestOptions({ headers: headers });
        // console.log(JSON.stringify(options));

        return this.http.get(url, options)
                   .map(this.extractData).catch(this.handleError);
    }
    // get user by key
    getUserByKey(key: number): Observable<IUser> {
        let url: string = this.baseUrl + "GetUser2/" + key;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get user by filter
    getUserByFilter(searchString?: string): Observable<any> {
        let url: string = this.baseUrl + "GetUserWithFilter/";
        if (searchString) { url += searchString; }
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // post user
    postUser(nUser: IUser): Observable<IUser> {
        // if has auth system add this
        //if (this.authService.userName) {
        //    nTool.Creator = this.authService.userName;
        //}
        nUser.UserId = 0;
        let url: string = this.baseUrl + "PostUser/";
        return this.http.post(
            url, JSON.stringify(nUser), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // put user
    putUser(key: number, uUser: IUser): Observable<IUser> {
        // if  has auth system add this
        //if (this.authService.userName) {
        //    uTool.Modifyer = this.authService.userName;
        //}
        let url: string = this.baseUrl + "PutUser/" + key;

        // console.log(JSON.stringify(this.getRequestOption()));

        return this.http.put(
            url, JSON.stringify(uUser), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    //===================== End User ==============================\\

}