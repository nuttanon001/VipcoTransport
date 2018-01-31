import { Subject } from "rxjs/Subject";

export abstract class AbstractCommunicateService<T> {

    // Observable string sources
    private ParentSource = new Subject<[T, boolean]>();
    private ChildSource = new Subject<T>();
    private EditChileSource = new Subject<T>();

    // Observable string streams
    ToParent$ = this.ParentSource.asObservable();
    ToChild$ = this.ChildSource.asObservable();
    toChildEdit$ = this.EditChileSource.asObservable();

    // Service message commands
    toParent(TypeValue: [T, boolean]): void {
        this.ParentSource.next(TypeValue);
    }

    toChild(DisplayValue: T): void {
        this.ChildSource.next(DisplayValue);
    }

    toChildEdit(EditValue: T): void {
        this.EditChileSource.next(EditValue);
    }
}