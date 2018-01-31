// core modules
import { Injectable } from "@angular/core";
import { Http, Response, Headers, RequestOptions, ResponseContentType } from "@angular/http";
import { Observable } from "rxjs/Observable";
// import classes
import { IEmployee } from "../classes/employee.class";
import { IDriver } from "../classes/driver.class";
import { ILocation } from "../classes/location.class";
import { IContact } from "../classes/contact.class";
import { LazyLoad } from "../classes/lazyload.class";
// import services
import { AuthenticationService } from "../services/auth.service";

@Injectable()
export class EmployeeDataService {
    constructor(
        private http: Http,
        private authService: AuthenticationService
    ) { }

    //#region Main

    private baseUrl: string = "api/Employee/";
    // for extractdata
    private extractData(r: Response) {
        let body = r.json();
        // console.log(body);
        return body || {};
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
    // for request option
    private getRequestOption(): RequestOptions {   // for request option
        return new RequestOptions({
            headers: new Headers({
                "Content-Type": "application/json"
            })
        });
    }

    //===================== Employee ==================================\\
    // get all employee
    getEmployee(): Observable<Array<IEmployee>> {
        let url: string = this.baseUrl;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get employee by key
    getEmployeeByKey(key: string): Observable<IEmployee> {
        let url: string = this.baseUrl + key;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get employee by lazyload data
    getEmployeeByLazyLoad(lazyData: LazyLoad): Observable<any> {
        let url: string = this.baseUrl + "GetEmployeeWithLazyLoad" + "/";
        return this.http.post(
            url, JSON.stringify(lazyData), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // get employee id for system
    getEmployeeIDByUserName(): Observable<any> {
        let url: string = this.baseUrl + "GetEmployeeByUserName/" + this.authService.userName + "/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }

    // post employee
    postEmployee(nEmployee: IEmployee): Observable<IEmployee> {
        // if has auth system add this
        //if (this.authService.userName) {
        //    nTool.Creator = this.authService.userName;
        //}
        let url: string = this.baseUrl;
        return this.http.post(
            url, JSON.stringify(nEmployee), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // put employee
    putEmployee(key: string, uEmployee: IEmployee): Observable<IEmployee> {
        // if  has auth system add this
        //if (this.authService.userName) {
        //    uTool.Modifyer = this.authService.userName;
        //}
        let url: string = this.baseUrl + key;
        return this.http.put(
            url, JSON.stringify(uEmployee), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    //===================== End Employee ==============================\\

    //===================== Driver ====================================\\
    // get all driver
    getDriver(): Observable<Array<IDriver>> {
        let url: string = this.baseUrl + "GetDriver/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get view model
    getDriverViewModel(): Observable<any> {
        let url: string = this.baseUrl + "GetDrivers/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get driver by key
    getDriverByKey(key: number): Observable<IDriver> {
        let url: string = this.baseUrl + "GetDriver/" + key;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get driver by filter
    getDriverByFilter(searchString?: string): Observable<any> {
        let url: string = this.baseUrl + "GetDriverFilter" + "/";
        if (searchString) { url += searchString; }
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get all with lazyload
    getDriverAllWithLazyLoad(lazyload: LazyLoad, subAction: string = "FindAllLayzLoad/"): Observable<any> {
        return this.http.post("api/Driver/" + subAction, JSON.stringify(lazyload), this.getRequestOption())
            .map(this.extractData).catch(this.handleError);
    }
    // post driver
    postDriver(nDriver: IDriver): Observable<IDriver> {
        // if has auth system add this
        if (this.authService.userName) {
            nDriver.Creator = this.authService.userName;
        }
        nDriver.EmployeeDriveId = 0;
        let url: string = this.baseUrl + "PostDriver/";
        return this.http.post(
            url, JSON.stringify(nDriver), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // put driver
    putDriver(key: number, uDriver: IDriver): Observable<IDriver> {
        // if  has auth system add this
        if (this.authService.userName) {
            uDriver.Modifyer = this.authService.userName;
        }
        let url: string = this.baseUrl + "PutDriver/" + key;
        return this.http.put(
            url, JSON.stringify(uDriver), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    //===================== End Driver ================================\\

    //===================== Location ==================================\\
    // get all location
    getLocation(): Observable<Array<ILocation>> {
        let url: string = this.baseUrl + "GetLocation/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get location by key
    getLocationByKey(key: number): Observable<ILocation> {
        let url: string = this.baseUrl + "GetLocation/" + key;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get location by filter
    getLocationByFilter(searchString?: string): Observable<any> {
        let url: string = this.baseUrl + "GetLocationWithFilter" + "/";
        if (searchString) { url += searchString; }
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // post location
    postLocation(nLocation: ILocation): Observable<ILocation> {
        // if has auth system add this
        //if (this.authService.userName) {
        //    nTool.Creator = this.authService.userName;
        //}
        nLocation.LocationId = 0;
        let url: string = this.baseUrl + "PostLocation/";
        return this.http.post(
            url, JSON.stringify(nLocation), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // put location
    putLocation(key: number, uLocation: ILocation): Observable<ILocation> {
        // if  has auth system add this
        //if (this.authService.userName) {
        //    uTool.Modifyer = this.authService.userName;
        //}
        let url: string = this.baseUrl + "PutLocation/" + key;
        return this.http.put(
            url, JSON.stringify(uLocation), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    //===================== End Location ==============================\\

    //===================== Contact ===================================\\
    // get all contact
    getContact(): Observable<Array<IContact>> {
        let url: string = this.baseUrl + "GetTblContact/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get contact view model
    getContactViewModel(): Observable<any> {
        let url: string = this.baseUrl + "GetTblContacts/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get contact by key
    getContactByKey(key: number): Observable<IContact> {
        let url: string = this.baseUrl + "GetTblContact/" + key;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get contact by filter
    getContactByFilter(searchString?: string): Observable<any> {
        let url: string = this.baseUrl + "GetTblContactWithFilter" + "/";
        if (searchString) { url += searchString; }
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // post contact only location
    postContactOnlyLocaion(nContactOnlLocation: ILocation): Observable<IContact> {
        if (this.authService.userName) {
            nContactOnlLocation.Creator = this.authService.userName;
        }
        nContactOnlLocation.LocationId = 0;
        let url: string = this.baseUrl + "PostContactOnlyLocation/";
        return this.http.post(
            url, JSON.stringify(nContactOnlLocation), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // post contact
    postContact(nContact: IContact): Observable<IContact> {
        if (this.authService.userName) {
            nContact.Creator = this.authService.userName;
        }
        nContact.ContactId = 0;
        let url: string = this.baseUrl + "PostContact/";
        return this.http.post(
            url, JSON.stringify(nContact), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // put contact
    putContact(key: number, uContact: IContact): Observable<IContact> {
        // if  has auth system add this
        //if (this.authService.userName) {
        //    uTool.Modifyer = this.authService.userName;
        //}
        let url: string = this.baseUrl + "PutContact/" + key;
        return this.http.put(
            url, JSON.stringify(uContact), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    //===================== End Contact ===============================\\
}
