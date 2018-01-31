// core modules
import { Http, Response, Headers, RequestOptions, ResponseContentType } from "@angular/http";
import { Observable } from "rxjs/Observable";
// classes
import { LazyLoad } from "../classes/index"
import { TreeNode } from "primeng/primeng";

export abstract class AbstractRestService<T>{
    constructor(
        protected http: Http,
        protected actionUrl: string) {
    }

    //===================== Privete Members =======================\\
    // extract data
    public extractData(r: Response) { // for extractdata
        let body = r.json();
        // console.log(body);
        return body || {};
    }
    // handle error
    public handleError(error: Response | any) {// for error message
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
    // extract data for result code
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
    // get request option
    public getRequestOption(): RequestOptions {   // for request option
        return new RequestOptions({
            headers: new Headers({
                "Content-Type": "application/json"
            })
        });
    }
    //===================== HTTP-Rest =============================\\
    // get all
    getAll(): Observable<Array<T>> {
        return this.http.get(this.actionUrl)
            .map(this.extractData).catch(this.handleError);
    }
    // get one with key number
    getOneKeyNumber(key: number): Observable<T> {
        return this.http.get(this.actionUrl + key + "/")
            .map(this.extractData).catch(this.handleError);
    }
    // get one with key string
    getOneKeyString(key: string): Observable<T> {
        return this.http.get(this.actionUrl + key + "/")
            .map(this.extractData).catch(this.handleError);
    }
    // get one with condition number
    getOneWithConditionNumber(condition: number, subAction: string = "Find/"): Observable<T> {
        return this.http.get(this.actionUrl + subAction +condition + "/")
            .map(this.extractData).catch(this.handleError);
    }
    // get one with condition string
    getOneWithConditionString(condition: string, subAction: string = "Find/"): Observable<T> {
        return this.http.get(this.actionUrl + subAction + condition + "/")
            .map(this.extractData).catch(this.handleError);
    }

    // get all with filter
    getFindAll(filter: string, subAction: string = "FindAll/"): Observable<Array<T>> {
        let url: string = this.actionUrl + subAction;
        if (filter) { url += filter + "/"; }

        return this.http.get(url)
            .map(this.extractData).catch(this.handleError);
    }
    // get all with filter for tree
    getFindAllForTree(filter: string, subAction: string = "NodeFindAll/"): Observable<Array<TreeNode>> {
        let url: string = this.actionUrl + subAction;
        if (filter) { url += filter + "/"; }

        return this.http.get(url)
            .map(this.extractData).catch(this.handleError);
    }
    // get all with lazyload
    getAllWithLazyLoad(lazyload: LazyLoad, subAction: string = "FindAllLayzLoad/"): Observable<any> {
        return this.http.post(this.actionUrl + subAction, JSON.stringify(lazyload), this.getRequestOption())
            .map(this.extractData).catch(this.handleError);
    }
    // post
    post(nObject: T): Observable<T> {
        return this.http.post(this.actionUrl, JSON.stringify(nObject), this.getRequestOption())
            .map(this.extractData).catch(this.handleError);
    }
    // put with key number
    putKeyNumber(uObject: T, key: number): Observable<T> {
        //console.log(uObject);
        return this.http.put(this.actionUrl + key + "/", JSON.stringify(uObject), this.getRequestOption())
            .map(this.extractData).catch(this.handleError);
    }
    // put with key string
    putKeyString(uObject: T, key: string): Observable<T> {
        return this.http.put(this.actionUrl + key + "/", JSON.stringify(uObject), this.getRequestOption())
            .map(this.extractData).catch(this.handleError);
    }
}