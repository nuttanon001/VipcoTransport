export interface IDriver {
    EmployeeDriveId: number;
    EmployeeDriveCode: string;
    CarId: number;
    TrailerId: number;
    PhoneNumber: string;
    Creator: string;
    Modifyer: string;
    CompanyID: number;
}

export class Driver implements IDriver {
    EmployeeDriveId: number;
    EmployeeDriveCode: string;
    CarId: number;
    TrailerId: number;
    PhoneNumber: string;
    Creator: string;
    Modifyer: string;
    CompanyID: number;
}