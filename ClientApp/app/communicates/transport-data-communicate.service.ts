import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { ITransportData } from "../classes/transport-data.class";

@Injectable()
export class TransportDataCommunicateService {

    // Observable string sources
    private TransportDataParentSource = new Subject<[ITransportData, boolean]>();
    private TransportDataChildSource = new Subject<ITransportData>();
    private TransportDataEditChileSource = new Subject<ITransportData>();

    // Observable string streams
    ToParent$ = this.TransportDataParentSource.asObservable();
    ToChild$ = this.TransportDataChildSource.asObservable();
    toChildEdit$ = this.TransportDataEditChileSource.asObservable();

    // Service message commands
    toParent(transportData: [ITransportData, boolean]): void {
        this.TransportDataParentSource.next(transportData);
    }

    toChild(transportData: ITransportData): void {
        this.TransportDataChildSource.next(transportData);
    }

    toChildEdit(transportDataEdit: ITransportData): void {
        this.TransportDataEditChileSource.next(transportDataEdit);
    }
}