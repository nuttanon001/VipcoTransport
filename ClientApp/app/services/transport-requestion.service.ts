import { Injectable } from "@angular/core";
import { Http } from "@angular/http";
import { Observable } from "rxjs/Observable";
// classes
import { ITransportReq,ICondition } from "../classes/index";
// seriver
import { AbstractRestService } from "./index";
@Injectable()
export class TransportRequestionService extends AbstractRestService<ITransportReq>{
    constructor(http: Http) {
        super(http, "api/TransportRequestion/");
    }

    // get Request-Waiting
    getRequestWaiting(condition: ICondition): Observable<any> {
        let url: string = this.actionUrl + "GetTranspotRequestWaiting/";

        // debug here
        // console.log("url: " + url);

        return this.http.post(url, JSON.stringify(condition), this.getRequestOption())
            .map(this.extractData).catch(this.handleError);
    }

}