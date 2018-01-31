import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { ITransportReq } from "../classes/transport-req.class";

@Injectable()
export class TransportReqCommunicateService {

    // Observable string sources
    private TransportReqParentSource = new Subject<[ITransportReq, any, boolean]>();
    private TransportReqChildSource = new Subject<ITransportReq>();
    private TransportReqEditChileSource = new Subject<ITransportReq>();

    // Observable string streams
    ToParent$ = this.TransportReqParentSource.asObservable();
    ToChild$ = this.TransportReqChildSource.asObservable();
    toChildEdit$ = this.TransportReqEditChileSource.asObservable();

    // Service message commands
    toParent(transportReq: [ITransportReq, any, boolean]): void {
        this.TransportReqParentSource.next(transportReq);
    }

    toChild(transportReq: ITransportReq): void {
        this.TransportReqChildSource.next(transportReq);
    }

    toChildEdit(transportReqEdit: ITransportReq): void {
        this.TransportReqEditChileSource.next(transportReqEdit);
    }
}