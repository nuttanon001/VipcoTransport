import { OnInit, ElementRef, ViewChild, ViewContainerRef } from "@angular/core";
import "rxjs/Rx";
import { Observable } from "rxjs/Observable"
// classes
import { LazyLoad } from "../../classes/index";
import { LazyLoadEvent } from "primeng/primeng";
//services
import { DialogsService } from "../../services/dialogs.service";

export abstract class AbstractMasterComponent<Interface, Service> implements OnInit {
    displayValue: Interface;
    editValue: any;
    values: Array<Interface>;
    columns: Array<any>;
    totalRow: number;
    scrollHeight: string;
    // boolean event
    _showEdit: boolean;
    canSave: boolean;
    hideleft: boolean;
    // element
    @ViewChild("filter") filter: ElementRef;

    constructor(
        protected service: Service,
        protected serviceCom: any,
        protected dialogsService: DialogsService,
        protected viewContainerRef: ViewContainerRef,
    ) { }

    // property
    get DisplayDataNull(): boolean {
        return this.displayValue === undefined;
    }

    get ShowEdit(): boolean {
        if (this._showEdit != null) {
            return this._showEdit;
        }
    }

    set ShowEdit(showEdit: boolean) {
        if (showEdit !== this._showEdit) {
            this.hideleft = !showEdit;
            this._showEdit = showEdit;
        }
    }

    // angular hook
    ngOnInit(): void {
        if (window.innerWidth >= 1600) {
            this.scrollHeight = 70 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 65 + "vh";
        } else {
            this.scrollHeight = 60 + "vh";
        }

        this.ShowEdit = false;
        this.canSave = false;

        this.serviceCom.ToParent$.subscribe(
            (TypeValue: [Interface, boolean]) => {
                this.editValue = TypeValue[0];
                this.canSave = TypeValue[1];
            });

        Observable.fromEvent(this.filter.nativeElement, 'keyup')
            .debounceTime(150)
            .distinctUntilChanged()
            .subscribe(() => {
                this.onLoadData(undefined);
            });
    }
    // on load data to table
    onLoadData(event: LazyLoadEvent): void {
        if (!event) {
            event = {
                first: 0,
                rows: 25,
                sortField: "",
                sortOrder: 1,
            }
        }

        let lazydata: LazyLoad = {
            first: event.first,
            rows: event.rows,
            sortField: event.sortField,
            sortOrder: event.sortOrder,
            filter: this.filter.nativeElement.value
        };

        this.onGetAllWithLazyload(lazydata);
    }

    // on get all with lazyload
    abstract onGetAllWithLazyload(lazyload: LazyLoad): void;
    //this.service.getAllWithLazyLoad(lazydata)
    //.subscribe(restData => {
    //    this.values = restData.Data;
    //    this.totalRow = restData.TotalRow;
    //}, error => console.error(error));

    // row Track by
    rowTrackBy(index: number, row: any) { return row.id; }

    // on detail view
    onDetailView(): void {
        if (this.displayValue) {
            setTimeout(() => this.serviceCom.toChild(this.displayValue), 500);
        }
    }

    // on detail edit
    onDetailEdit(editValue?: Interface): void {
        this.displayValue = editValue;
        this.ShowEdit = true;
        setTimeout(() => this.serviceCom.toChildEdit(this.displayValue), 1000);
    }

    // on cancel edit
    onCancelEdit(): void {
        this.editValue = undefined;
        this.displayValue = undefined;
        this.canSave = false;
        this.ShowEdit = false;
        this.onDetailView();
    }

    // on submit
    onSubmit(): void {
        this.canSave = false;
        if (this.editValue.Creator) {
            this.onUpdateToDataBase(this.editValue);
        } else {
            this.onInsertToDataBase(this.editValue);
        }
    }

    // change data to timezone
    abstract changeTimezone(value: Interface): Interface;

    // on insert data
    abstract onInsertToDataBase(value: Interface): void;

    // on update data
    abstract onUpdateToDataBase(value: Interface): void;

    // on save complete
    onSaveComplete(): void {
        this.dialogsService
            .context("System message", "Save completed.", this.viewContainerRef)
            .subscribe(result => {
                this.canSave = false;
                this.ShowEdit = false;
                this.onDetailView();
                this.editValue = undefined;
                this.onLoadData(undefined);
            });
    }
}