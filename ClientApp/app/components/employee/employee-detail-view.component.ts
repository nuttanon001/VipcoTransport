import { Component, OnInit, OnDestroy } from "@angular/core";
// class
import { IEmployee } from "../../classes/employee.class";
// service
import { EmployeeDataService } from "../../services/employee.service";
import { EmployeeCommunicateService } from "../../communicates/employee-communicate.service"

import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: "employee-detail-view",
    templateUrl: "./employee-detail-view.component.html",
    styleUrls: ["../../styles/style-of-detail.component.scss"],
})

export class EmployeeDetailViewComponent implements OnInit, OnDestroy {
    employee: IEmployee;
    subscription: Subscription;

    constructor(
        private employeeComService: EmployeeCommunicateService,
        private employeeService: EmployeeDataService,
    ) { }

    ngOnInit(): void {
        this.subscription = this.employeeComService.ToChild$.subscribe(
            (employee: IEmployee) => {
                // debug here
                // console.log(JSON.stringify(employee));
                this.employeeService.getEmployeeByKey(employee.EmpCode)
                    .subscribe(dbEmployee => this.employee = dbEmployee, error => console.error(error));
            });
    }

    ngOnDestroy() {
        // prevent memory leak when component destroyed
        this.subscription.unsubscribe();
    }
}
