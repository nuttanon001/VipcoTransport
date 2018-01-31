export interface IDriverHasCompany {
    DriverHasCompanyId: number;
    EmployeeDriveId: number;
    CompanyId: number;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;
}

export class DriverHasCompany implements IDriverHasCompany {
    DriverHasCompanyId: number;
    EmployeeDriveId: number;
    CompanyId: number;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;

    constructor() {
        this.DriverHasCompanyId = 0;
    }
}