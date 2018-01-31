import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { ITrailer } from "../classes/trailer.class";

@Injectable()
export class TrailerCommunicateService {
    // Observable string sources
    private TrailerParentSource = new Subject<[ITrailer, boolean]>();
    private TrailerChildSource = new Subject<ITrailer>();
    private TrailerEditChileSource = new Subject<ITrailer>();
    // Observable string streams
    ToParent$ = this.TrailerParentSource.asObservable();
    ToChild$ = this.TrailerChildSource.asObservable();
    toChildEdit$ = this.TrailerEditChileSource.asObservable();
    // Service message commands
    toParent(trailer: [ITrailer, boolean]): void {
        this.TrailerParentSource.next(trailer);
    }
    toChild(trailer: ITrailer): void {
        this.TrailerChildSource.next(trailer);
    }
    toChildEdit(trailerEdit: ITrailer): void {
        this.TrailerEditChileSource.next(trailerEdit);
    }
}