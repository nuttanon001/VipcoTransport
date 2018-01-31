import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { IEmployee } from "../classes/employee.class";

@Injectable()
export class EmployeeCommunicateService {
    // Observable string sources
    private EmployeeParentSource = new Subject<[IEmployee, boolean]>();
    private EmployeeChildSource = new Subject<IEmployee>();
    private EmployeeEditChileSource = new Subject<IEmployee>();
    // Observable string streams
    ToParent$ = this.EmployeeParentSource.asObservable();
    ToChild$ = this.EmployeeChildSource.asObservable();
    toChildEdit$ = this.EmployeeEditChileSource.asObservable();
    // Service message commands
    toParent(employee: [IEmployee, boolean]): void {
        this.EmployeeParentSource.next(employee);
    }
    toChild(employee: IEmployee): void {
        this.EmployeeChildSource.next(employee);
    }
    toChildEdit(employeeEdit: IEmployee): void {
        this.EmployeeEditChileSource.next(employeeEdit);
    }
}