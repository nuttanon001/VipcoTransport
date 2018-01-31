// core modules
import { Injectable } from "@angular/core";
import { Http, Response, Headers, RequestOptions, ResponseContentType } from "@angular/http";
import { Observable } from "rxjs/Observable";
// import classes
import { ICarBrand } from "../classes/car-brand.class";
import { ICarData } from "../classes/car-data.class";
import { ICarType } from "../classes/car-type.class";
import { ITrailer } from "../classes/trailer.class";
import { LazyLoad } from "../classes/lazyload.class";
// import services
import { AuthenticationService } from "../services/auth.service";
// timezone
import * as moment from "moment-timezone";
@Injectable()
export class CarDataService {
    constructor(
        private http: Http,
        private authService: AuthenticationService
    ) { }

    //#region Main
    // base url
    private baseUrl = "api/Car/";
    // extract data
    private extractData(r: Response) { // for extractdata
        let body = r.json();
        // console.log(body);
        return body || {};
    }
    // extract with date type not string date
    private extractDataForDate(res: Response) { // for extract change date type
        // debug here
        //console.log("json is " + res.json());
        let body = res.json();
        if ("CarDate" in body) {
            let data: ICarData = res.json();
            data.CarDate = data.CarDate != null ? new Date(data.CarDate) : null;
            data.InsurDate = data.InsurDate != null ? new Date(data.InsurDate) : null;
            data.ActDate = data.ActDate != null ? new Date(data.ActDate) : null;
            return data;
        } else if ("InsurDate" in body) {
            let data: ITrailer = res.json();
            data.InsurDate = data.InsurDate != null ? new Date(data.InsurDate) : null;
            return data;
        } else {
            return body;
        }
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
    // get request option
    private getRequestOption(): RequestOptions {   // for request option
        return new RequestOptions({
            headers: new Headers({
                "Content-Type": "application/json"
            })
        });
    }
    // change timezone date
    private changeTimezone(data: any): any {
        var zone = "Asia/Bangkok";
        if (data !== null) {
            if ("CarDate" in data) {
                if (data.CarDate !== null) {
                    data.CarDate = moment.tz(data.CarDate, zone).format();
                }
                if (data.InsurDate !== null) {
                    data.InsurDate = moment.tz(data.InsurDate, zone).format();
                }
                if (data.ActDate !== null) {
                    data.ActDate = moment.tz(data.ActDate, zone).format();
                }
            } else if ("InsurDate" in data) {
                if (data.InsurDate !== null) {
                    data.InsurDate = moment.tz(data.InsurDate, zone).format();
                }
            }
        }
        return data;
    }

    //===================== CarData ===================================\\
    // get all car-data
    getCarData(): Observable<Array<ICarData>> {
        let url: string = this.baseUrl;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get all car-data-view-model
    getCarDataViewModel(): Observable<any> {
        let url: string = this.baseUrl + "GetCars/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get car by company
    getCarByCompany(companyId: number): Observable<Array<ICarData>> {
        let url: string = this.baseUrl + "GetCarsByCompany/" + companyId;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get car-data by key
    getCarDataByKey(key: number): Observable<ICarData> {
        let url: string = this.baseUrl + key;
        return this.http.get(url).map(this.extractDataForDate).catch(this.handleError);
    }
    // get car-data with lazyload
    getCarDataByLazyLoadData(lazyData: LazyLoad): Observable<any> {
        let url: string = this.baseUrl + "GetCarDataWithLazyLoad" + "/";
        return this.http.post(
            url, JSON.stringify(lazyData), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // post car-data
    postCarData(nCarData: ICarData): Observable<ICarData> {
        if (this.authService.userName) {
            nCarData.Creator = this.authService.userName;
        }
        // debug here
        //console.log(JSON.stringify(nCarData));

        nCarData.CarId = 0;
        let url: string = this.baseUrl;
        return this.http.post(
            url, JSON.stringify(nCarData), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // put car-data
    putCarData(key: number, uCarData: ICarData): Observable<ICarData> {
        if (this.authService.userName) {
            uCarData.Modifyer = this.authService.userName;
        }
        // debug here
        //console.log(JSON.stringify(uCarData));

        let url: string = this.baseUrl + key;
        return this.http.put(
            url, JSON.stringify(uCarData), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    //===================== End CarData ===============================\\

    //===================== Trailer ===================================\\
    // get all trailer
    getTrailer(): Observable<Array<ITrailer>> {
        let url: string = this.baseUrl + "GetTrailer/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get trailer by key
    getTrailerByKey(key: number): Observable<ITrailer> {
        let url: string = this.baseUrl + "GetTrailer/" + key;
        return this.http.get(url).map(this.extractDataForDate).catch(this.handleError);
    }
    // get car by company
    getTruckByCompany(companyId: number): Observable<Array<ITrailer>> {
        let url: string = this.baseUrl + "GetTrucksByCompany/" + companyId;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get trailer-viewmodel
    getTrailerViewModel(): Observable<any> {
        let url: string = this.baseUrl + "GetTrailers/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get trailer with lazyload
    getTrailerByLazyLoadData(lazyData: LazyLoad): Observable<any> {
        let url: string = this.baseUrl + "GetTrailerWithLazyData" + "/";
        return this.http.post(
            url, JSON.stringify(lazyData), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // post trailer
    postTrailer(nTrailer: ITrailer): Observable<ITrailer> {
        // if has auth system add this
        if (this.authService.userName) {
            nTrailer.Creator = this.authService.userName;
        }
        nTrailer.TrailerId = 0;
        let url: string = this.baseUrl + "PostTrailer/";
        return this.http.post(
            url, JSON.stringify(nTrailer), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // put trailer
    putTrailer(key: number, uTrailer: ITrailer): Observable<ITrailer> {
        // if has auth system add this
        if (this.authService.userName) {
            uTrailer.Modifyer = this.authService.userName;
        }
        let url: string = this.baseUrl + "PutTrailer/" +  key;
        return this.http.put(
            url, JSON.stringify(uTrailer), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    //===================== End Trailer ===============================\\

    //===================== CarType ===================================\\
    // get all car-type
    getCarType(): Observable<Array<ICarType>> {
        let url: string = this.baseUrl + "GetCarType/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get car-type by key
    getCarTypeByKey(key: number): Observable<ICarType> {
        let url: string = this.baseUrl + "GetCarType/" + key;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get car-type by filter
    getCarTypeByFilter(searchString?: string): Observable<any> {
        let url: string = this.baseUrl + "GetCarTypeFilter" + "/";
        if (searchString) { url += searchString; }
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // port car-type
    postCarType(nCarType: ICarType): Observable<ICarType> {
        // if has auth system add this
        //if (this.authService.userName) {
        //    nTool.Creator = this.authService.userName;
        //}
        nCarType.CarTypeId = 0;
        let url: string = this.baseUrl + "PostCarType/";
        return this.http.post(
            url, JSON.stringify(nCarType), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // put car-type
    putCarType(key: number, uCarType: ICarType): Observable<ICarType> {
        // if  has auth system add this
        //if (this.authService.userName) {
        //    uTool.Modifyer = this.authService.userName;
        //}
        let url: string = this.baseUrl +"PutCarType/"+ key;
        return this.http.put(
            url, JSON.stringify(uCarType), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    //===================== End CarType ===============================\\

    //===================== CarBrand ==================================\\
    // get all car-brand
    getCarBrand(): Observable<Array<ICarBrand>> {
        let url: string = this.baseUrl + "GetCarBrand/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get car-brand by key
    getCarBrandByKey(key: number): Observable<ICarBrand> {
        let url: string = this.baseUrl + "GetCarBrand/" + key;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // get car-brand by filter
    getCarBrandByFilter(searchString?: string): Observable<any> {
        let url: string = this.baseUrl + "GetCarBrandFilter" + "/";
        if (searchString) { url += searchString; }
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }
    // post car-brand
    postCarBrand(nCarBrand: ICarBrand): Observable<ICarBrand> {
        // if has auth system add this
        //if (this.authService.userName) {
        //    nTool.Creator = this.authService.userName;
        //}
        nCarBrand.BrandId = 0;
        let url: string = this.baseUrl + "PostCarBarnd/";
        return this.http.post(
            url, JSON.stringify(nCarBrand), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    // put car-brand
    putCarBrand(key: number, uCarBrand: ICarBrand): Observable<ICarBrand> {
        // if  has auth system add this
        //if (this.authService.userName) {
        //    uTool.Modifyer = this.authService.userName;
        //}
        let url: string = this.baseUrl + "PutCarBarnd/" + key;
        return this.http.put(
            url, JSON.stringify(uCarBrand), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    //===================== End CarBrand ==============================\\
}