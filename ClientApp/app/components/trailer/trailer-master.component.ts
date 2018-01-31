import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs/Observable";
// classes
import { ITrailer, Trailer, LazyLoad } from "../../classes/index";
import { LazyLoadEvent } from "primeng/primeng";
// service
import { TrailerCommunicateService } from "../../communicates/trailer-communicate.service";
import {
    CarDataService, DialogsService,
    CompanyService, AuthenticationService
} from "../../services/index";

@Component({
    selector: "trailer-master",
    templateUrl: "./trailer-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        CarDataService,
        CompanyService,
        TrailerCommunicateService,
        DialogsService
    ]
})

export class TrailerMasterComponent implements OnInit {
    searchField: FormControl;
    trailerForm: FormGroup;
    trailers: Array<ITrailer>;
    trailer: ITrailer;
    editTrailer: ITrailer;

    totalRow: number;
    scrollHeight: string;
    showEdit: boolean;
    canSave: boolean;

    companyID: number;
    constructor(
        private traiService: CarDataService,
        private traiComService: TrailerCommunicateService,
        private dialogsService: DialogsService,
        private authService: AuthenticationService,
        private viewContainerRef: ViewContainerRef,
        private route: ActivatedRoute,
        private fb: FormBuilder,
    ) { }
    // property
    get TrailerNull(): boolean {
        return this.trailer === undefined;
    }
    // component inti
    ngOnInit(): void {
        this.searchField = new FormControl();
        this.bulidForm();
        this.showEdit = false;
        this.canSave = false;
        this.traiComService.ToParent$.subscribe(
            (trailerCom: [ITrailer, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editTrailer = trailerCom[0];
                this.canSave = trailerCom[1];
                // debug here
                //console.log("can save " + this.canSave);
                //console.log(JSON.stringify(this.editTrailer));
            });

        this.route.params.subscribe((params: any) => {

            let condition: number = params["condition"];
            if (condition) {
                this.companyID = -1;
            }
        });
        this.companyID = this.companyID != -1 ? this.authService.getUser.CompanyId : this.companyID;

    }
    // bulid form
    bulidForm(): void {
        this.trailerForm = this.fb.group({
            "search": this.searchField
        });

        if (window.innerWidth >= 1600) {
            this.scrollHeight = 78 + "vh";
        } else if (window.innerWidth > 1360 && window.innerWidth < 1600) {
            this.scrollHeight = 75 + "vh";
        } else {
            this.scrollHeight = 70 + "vh";
        }
    }
    // load data for datatable
    loadData(event: LazyLoadEvent) {
        //in a real application, make a remote request to load data using state metadata from event
        //event.first = First row offset
        //event.rows = Number of rows per page
        //event.sortField = Field name to sort with
        //event.sortOrder = Sort order as number, 1 for asc and -1 for dec
        //filters: FilterMetadata object having field as key and filter value, filter matchMode as value
        if (!event) {
            event = {
                first: 0,
                rows: 100,
                sortField: "",
                sortOrder: 1,
            }
        }

        let lazydata: LazyLoad = {
            first: event.first,
            rows: event.rows,
            sortField: event.sortField,
            sortOrder: event.sortOrder,
            filter: this.searchField.value,
            option: this.companyID
        };


        this.traiService.getTrailerByLazyLoadData(lazydata).subscribe(result => {
            this.trailers = result.Data;
            this.totalRow = result.TotalRecordCount;
            //debug here
            //console.log(JSON.stringify(this.trailers));
            //console.log(JSON.stringify(this.tools));
        }, error => console.error(error));
    }
    // on detail view
    onDetailView(): void {
        if (this.trailer) {
            // debug here
            // console.log(JSON.stringify(this.trailer));
            setTimeout(() => this.traiComService.toChild(this.trailer), 1000);
        }
    }
    // on detail edit
    onDetailEdit(trailer?: ITrailer): void {
        if (!trailer) {
            trailer = new Trailer();
        }

        this.trailer = trailer;
        this.showEdit = true;
        setTimeout(() => this.traiComService.toChildEdit(this.trailer), 1000);
    }
    // on cancel edit
    onCancelEdit(): void {
        this.editTrailer = undefined;
        this.trailer = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }
    // on submit
    onSubmit(): void {
        if (this.editTrailer.Creator) {
            this.updateToDataBase(this.editTrailer);
        } else {
            this.insertToDataBase(this.editTrailer);
        }
    }
    // on insert data
    insertToDataBase(trailer: ITrailer): void {
        this.traiService.postTrailer(trailer).subscribe(
            (complete: any) => {
                this.trailer = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.editTrailer.Creator = undefined;
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on update data
    updateToDataBase(trailer: ITrailer): void {
        this.traiService.putTrailer(trailer.TrailerId, trailer).subscribe(
            (complete: any) => {
                this.trailer = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on save complete
    saveComplete(): void {
        this.dialogsService
            .context("System message", "Save completed.", this.viewContainerRef)
            .subscribe(result => {
                this.canSave = false;
                this.showEdit = false;
                this.onDetailView();
                this.editTrailer = undefined;
                this.loadData(undefined);
            });
    }
}