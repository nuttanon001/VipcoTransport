// core modules
import { Injectable } from "@angular/core";
import { Http, Response, Headers, RequestOptions, ResponseContentType } from "@angular/http";
import { Observable } from "rxjs/Observable";
// import classes
import { IScheduleWeekly } from "../classes/schedule-week.class";
import { ITransportData } from "../classes/transport-data.class";
import { ITransportReq } from "../classes/transport-req.class";
import { ITransportType } from "../classes/transport-type.class";
import { LazyLoad } from "../classes/lazyload.class";
// import services
import { AuthenticationService } from "../services/auth.service";
// timezone
import * as moment from "moment-timezone";

@Injectable()
export class TransportService {
    constructor(
        private http: Http,
        private authService: AuthenticationService
    ) { }

    //#region Main
    // base url
    private baseUrl: string = "api/Transport/";
    // extract data
    private extractData(r: Response) { // for extractdata
        let body = r.json();
        // console.log(body);
        return body || {};
    }

    private extractResultCode(res: Response) {
        if (res) {
            if (res.status === 201) {
                return [{ status: res.status, json: res }]
            }
            else if (res.status === 200) {
                return [{ status: res.status, json: res }]
            }
        }
    }
    // extract with date type not string date
    private extractDataForDate(res: Response) { // for extract change date type
        // debug here
        //console.log("json is " + res.json());
        let body = res.json();
        if ("TransportReqDate" in body) {
            let data: ITransportReq = res.json();
            data.TransportDate = data.TransportDate != null ? new Date(data.TransportDate) : null;
            data.TransportReqDate = data.TransportReqDate != null ? new Date(data.TransportReqDate) : null;
            return data;
        } else if ("TransDate" in body) {
            let data: ITransportData = res.json();
            data.TransDate = data.TransDate != null ? new Date(data.TransDate) : null;
            data.FinalDate = data.FinalDate != null ? new Date(data.FinalDate) : null;
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
            errMsg = error.status + " - " + (error.statusText || '') + err;
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

            // console.log(data.name);

            if ("TransDate" in data) {
                // debug here
                // console.log("ITransportData");

                if (data.TransDate !== null) {
                    data.TransDate = moment.tz(data.TransDate, zone).format();
                }
                if (data.FinalDate !== null) {
                    data.FinalDate = moment.tz(data.FinalDate, zone).format();
                }
            } else if ("TransportDate" in data) {
                // debug here
                // console.log("ITransportReq");

                if (data.TransportReqDate !== null) {
                    data.TransportReqDate = moment.tz(data.TransportReqDate, zone).format();
                }
                if (data.TransportDate !== null) {
                    data.TransportDate = moment.tz(data.TransportDate, zone).format();
                }
            }
        }
        return data;
    }

    //===================== TransportData =============================\\

    // get transport-data all
    getTransportData(): Observable<Array<ITransportData>> {
        let url: string = this.baseUrl;
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }

    // get transport-data by key
    getTransportDataByKey(key: number): Observable<ITransportData> {
        let url: string = this.baseUrl + key;
        return this.http.get(url).map(this.extractDataForDate).catch(this.handleError);
    }

    //get transport-data by transport-requested
    getTransportDataByTransportRequest(key: number): Observable<ITransportData> {
        let url: string = this.baseUrl + "GetTransportDataByTransportRequested/" + key + "/";
        return this.http.get(url).map(this.extractDataForDate).catch(this.handleError);
    }

    // get transport-data by lazy load
    getTransportDataByLazyLoad(lazyData: LazyLoad): Observable<any> {
        let url: string = this.baseUrl + "GetTransportWithLazyLoad" + "/";
        return this.http.post(
            url, JSON.stringify(lazyData), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }

    // get transport-data-truck by lazy load
    getTransportDataTruckByLazyLoad(lazyData: LazyLoad): Observable<any> {
        let url: string = this.baseUrl + "GetTransportTruckWithLazyLoad" + "/";
        return this.http.post(
            url, JSON.stringify(lazyData), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }

    // get transport-data schedule
    getTransportDailySchedule(pickDate: any): Observable<any> {

        let url: string = this.baseUrl + "GetTranspotDailySchedule/";

        // debug here
        // console.log(url);

        return this.http.post(
            url, JSON.stringify(pickDate), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }

    // get transport-data schedule
    getTransportWeeklySchedule(dateRange: IScheduleWeekly): Observable<any> {

        let url: string = this.baseUrl + "GetTranspotWeeklySchedule/";

        // dateRange.StartDateString = dateRange.StartDate.getDate() + "/" + (dateRange.StartDate.getMonth() + 1) + "/" + dateRange.StartDate.getFullYear();
        // dateRange.EndDateString = dateRange.EndDate.getDate() + "/" + (dateRange.EndDate.getMonth() + 1) + "/" + dateRange.EndDate.getFullYear();

        if (dateRange.StartDate !== null) {
            dateRange.StartDate2 = moment.tz(dateRange.StartDate, "Asia/Bangkok").format();
        }
        if (dateRange.EndDate !== null) {
            dateRange.EndDate2 = moment.tz(dateRange.EndDate, "Asia/Bangkok").format();
        }
        // debug here
        // console.log(JSON.stringify(dateRange));

        return this.http.post(
            url, JSON.stringify(dateRange), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }

    // get transport-data schedule with transport data
    getTransportScheduleWithTrasnsportData(stringData: string): Observable<any> {

        let url: string = this.baseUrl + "GetTransportDailyWithTransportID/";
        return this.http.post(
            url, JSON.stringify(stringData), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }

    // get transport-data report
    getTransportationReport(transpotId:number): Observable<any> {
        let url: string = this.baseUrl + "GetReportTransportation/";
        url += transpotId;

        return this.http.get(url, { responseType: ResponseContentType.Blob })
            .map(res => res.blob())
            .catch(this.handleError)
    }

    getTransportationPdfReport(transpotId: number): Observable<any> {
        let url: string = this.baseUrl + "GetReportTransportationPdf/";
        url += transpotId;

        return this.http.get(url, { responseType: ResponseContentType.Blob })
            .map(res => res.blob())
            .catch(this.handleError)
    }

    // post transport-data
    postTransportData(transportReq: number,nTransportData: ITransportData): Observable<ITransportData> {
        // if has auth system add this
        if (this.authService.userName) {
            nTransportData.Creator = this.authService.userName;
        }

        nTransportData = this.changeTimezone(nTransportData);

        // nTransportData.StringTransDate = nTransportData.TransDate.getDate() + "/" + (nTransportData.TransDate.getMonth() + 1) + "/" + nTransportData.TransDate.getFullYear();
        // debug here
        // console.log("postTransportData : " + JSON.stringify(nTransportData));

        let url: string = this.baseUrl + transportReq + "/";
        return this.http.post(
            url, JSON.stringify(nTransportData), this.getRequestOption()

        ).map(this.extractData).catch(this.handleError);
    }

    // put transport-data
    putTransportData(key: number, uTransportData: ITransportData): Observable<ITransportData> {
        // if  has auth system add this
        if (this.authService.userName) {
            uTransportData.Modifyer = this.authService.userName;
        }

        uTransportData = this.changeTimezone(uTransportData);

        // uTransportData.StringTransDate = uTransportData.TransDate.getDate() + "/" + (uTransportData.TransDate.getMonth() + 1) + "/" + uTransportData.TransDate.getFullYear();
        let url: string = this.baseUrl + key;
        return this.http.put(
            url, JSON.stringify(uTransportData), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }
    //===================== End TransportData =========================\\

    //===================== TransportReq ==============================\\

    // get transport-req all
    getTransportReq(): Observable<Array<ITransportReq>> {
        let url: string = this.baseUrl + "GetTransportReq/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }

    // get transport-req by key
    getTransportReqByKey(key: number): Observable<ITransportReq> {
        let url: string = this.baseUrl +"GetTransportReq/"+ key;
        return this.http.get(url).map(this.extractDataForDate).catch(this.handleError);
    }

    // get transport-req by lazy load
    getTransportReqByLazyLoad(lazyData: LazyLoad): Observable<any> {
        let url: string = this.baseUrl + "GetTransportReqWithLazyLoad" + "/";
        return this.http.post(
            url, JSON.stringify(lazyData), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }



    // get attach file transport-request
    getAttachFileTransportRequest(transportReqID: number): Observable<any> {
        let url: string = this.baseUrl + "GetTransportReqAttachFile/" + transportReqID + "/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }

    // get transport-req By transportId
    getTransportRequestByTransportDataID(transportDataId: number): Observable<Array<ITransportReq>> {
        let url: string = this.baseUrl + "GetTransportReqByTransportData/" + transportDataId + "/";
        return this.http.get(url).map(this.extractDataForDate).catch(this.handleError);
    }

    // get transport-data same transport request
    getTransportDataSameTransportRequestByID(transportReqID: number): Observable<any> {
        let url: string = this.baseUrl + "GetTransportDataSameTransportReq/" + transportReqID + "/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }

    // transport request change status to cancel
    cancelTransportRequest(key: string): Observable<any> {
        let url: string = this.baseUrl + "CancelTranspotRequest/" + key + "/" + this.authService.userName + "/";
        return this.http.delete(url).catch(this.handleError);
    }

    // post transport-req
    postTransportReq(nTransportReq: ITransportReq): Observable<ITransportReq> {
        // if has auth system add this
        if (this.authService.userName) {
            nTransportReq.Creator = this.authService.userName;
        }

        nTransportReq = this.changeTimezone(nTransportReq);

        // nTransportReq.StringTransportDate = nTransportReq.TransportDate.getDate() + "/" + (nTransportReq.TransportDate.getMonth() + 1) + "/" + nTransportReq.TransportDate.getFullYear();
        // debug here
        // console.log("postTransportReq " + JSON.stringify(nTransportReq));

        let url: string = this.baseUrl + "PostTransportReq/";
        return this.http.post(
            url, JSON.stringify(nTransportReq), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }

    // put transport-req
    putTransportReq(key: number, uTransportReq: ITransportReq): Observable<ITransportReq> {
        // if  has auth system add this
        if (this.authService.userName) {
            uTransportReq.Modifyer = this.authService.userName;
        }

        uTransportReq = this.changeTimezone(uTransportReq);

        // uTransportReq.StringTransportDate = uTransportReq.TransportDate.getDate() + "/" + (uTransportReq.TransportDate.getMonth() + 1) + "/" + uTransportReq.TransportDate.getFullYear();

        // debug here
        // console.log("putTransportReq " + JSON.stringify(uTransportReq));

        let url: string = this.baseUrl + "PutTransportReq/" + key;
        return this.http.put(
            url, JSON.stringify(uTransportReq), this.getRequestOption()
        ).map(this.extractData).catch(this.handleError);
    }

    // put transport-requested same transport-data
    putTransportRequestedSameTransportData(transportId: number, transportReqId: number) {
        let url: string = this.baseUrl + "PutUpdateTransportReqSameTransportData/" + transportId + "/" + transportReqId + "/";

        console.log(JSON.stringify(this.authService.getUser));

        return this.http.put(
            url,JSON.stringify(this.authService.getUser), this.getRequestOption()
        ).map(this.extractResultCode).catch(this.handleError);
    }

    //===================== End TransportReq ==========================\\

    //===================== TransportType =============================\\

    // get transporttype all
    getTransportType(): Observable<Array<ITransportType>> {
        let url: string = this.baseUrl + "GetTransportTypes/";
        return this.http.get(url).map(this.extractData).catch(this.handleError);
    }

    //===================== End TransportType =========================\\

    //===================== Upload File ===============================\\

    // upload file for transport request
    postAttachFileForTransportRequest(transportReqID: number, fileToUpload: any) {
        let input = new FormData();

        // console.log("fileToUpload is : " + fileToUpload);

        // fileToUpload.forEach((item, index) => {
        //    console.log("item is : " + item);
        //    input.append("file", item);
        // });

        for (var i = 0; i < fileToUpload.files.length; i++) {
            if (fileToUpload.files[i].size <= 5242880)
                input.append("files", fileToUpload.files[i]);
        }

        let url: string = this.baseUrl + "PostTransportReqAttach/" + transportReqID;
        return this.http.post(url, input).map(this.extractData).catch(this.handleError);
    }

    // delete file for transport request
    deleteAttachFileForTransportRequest(attachID: number): Observable<any>  {
        let url: string = this.baseUrl + "DeleteTransportReqAttach/" + attachID;
        return this.http.delete(url).catch(this.handleError);
    }

    //===================== End Upload File ===========================\\

}