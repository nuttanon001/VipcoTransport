import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { FormControl, FormBuilder, FormGroup } from "@angular/forms";
import { Observable } from "rxjs/Observable";
// classes
import { IEmployee, Employee } from "../../classes/employee.class";
import { LazyLoad } from "../../classes/lazyload.class";
import { LazyLoadEvent } from "primeng/primeng";
// service
import { EmployeeCommunicateService } from "../../communicates/employee-communicate.service";
import { EmployeeDataService } from "../../services/employee.service";
import { DialogsService } from "../../services/dialogs.service";

@Component({
    selector: "employee-master",
    templateUrl: "./employee-master.component.html",
    styleUrls: ["../../styles/style-of-master.component.scss"],
    providers: [
        EmployeeDataService,
        EmployeeCommunicateService,
        DialogsService
    ]
})

export class EmployeeMasterComponent implements OnInit {
    searchField: FormControl;
    employeeForm: FormGroup;
    employees: Array<IEmployee>;
    employee: IEmployee;
    editEmployee: IEmployee;

    totalRow: number;
    scrollHeight: string;
    showEdit: boolean;
    canSave: boolean;
    constructor(
        private empService: EmployeeDataService,
        private empComService: EmployeeCommunicateService,
        private dialogsService: DialogsService,
        private viewContainerRef: ViewContainerRef,
        private fb: FormBuilder,
    ) { }
    // property
    get EmployeeNull(): boolean {
        return this.employee === undefined;
    }
    // component inti
    ngOnInit(): void {
        this.searchField = new FormControl();
        this.bulidForm();
        this.showEdit = false;
        this.canSave = false;

        this.empComService.ToParent$.subscribe(
            (employeeCom: [IEmployee, boolean]) => {
                // debug here
                //console.log("to parent");
                this.editEmployee = employeeCom[0];
                this.canSave = employeeCom[1];
                // debug here
                //console.log("can save " + this.canSave);
                //console.log(JSON.stringify(this.editEmployee));
            });
    }
    // bulid form
    bulidForm(): void {
        this.employeeForm = this.fb.group({
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

        let lazydata = {
            first: event.first,
            rows: event.rows,
            sortField: event.sortField,
            sortOrder: event.sortOrder,
            filter: this.searchField.value
        };


        this.empService.getEmployeeByLazyLoad(lazydata).subscribe(result => {
            this.employees = result.Data;
            this.totalRow = result.TotalRecordCount;
            //debug here
            //console.log(JSON.stringify(this.employees));
            //console.log(JSON.stringify(this.tools));
        }, error => console.error(error));

    }
    // on detail view
    onDetailView(): void {
        if (this.employee) {
            // debug here
            // console.log(JSON.stringify(this.employee));
            setTimeout(() => this.empComService.toChild(this.employee), 1000);
        }
    }
    // on detail edit
    onDetailEdit(employee?: IEmployee): void {
        if (!employee) {
            employee = new Employee();
        }

        this.employee = employee;
        this.showEdit = true;
        setTimeout(() => this.empComService.toChildEdit(this.employee), 1000);
    }
    // on cancel edit
    onCancelEdit(): void {
        this.editEmployee = undefined;
        this.employee = undefined;
        this.canSave = false;
        this.showEdit = false;
        this.onDetailView();
    }
    // on submit
    onSubmit(): void {
        if (this.editEmployee.EmpCode) {
            this.updateToDataBase(this.editEmployee);
        } else {
            this.insertToDataBase(this.editEmployee);
        }
    }
    // on insert data
    insertToDataBase(employee: IEmployee): void {
        this.empService.postEmployee(employee).subscribe(
            (complete: any) => {
                this.employee = complete;
                this.saveComplete();
            },
            (error: any) => {
                console.error(error);
                this.dialogsService.error("Failed !", "Save failed with the following error: Invalid Identifier code !!!", this.viewContainerRef)
            }
        );
    }
    // on update data
    updateToDataBase(employee: IEmployee): void {
        this.empService.putEmployee(employee.EmpCode, employee).subscribe(
            (complete: any) => {
                this.employee = complete;
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
                this.editEmployee = undefined;
                this.loadData(undefined);
            });
    }
}