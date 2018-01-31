export interface ITransportData {
    TransportId: number;
    CarId: number;
    TrailerId: number;
    CarTypeId: number;
    TransportNo: string;
    ContactSource: number;
    ContactDestination: number;
    EmployeeRequestCode: string;
    EmployeeDrive: number;
    EmployeeDriveCode: string;
    TransDate: Date;
    StringTransDate: string;
    TransTime: string;
    FinalDate: Date;
    FinalTime: string;
    TransportTypeId: number;
    TransportInformation: string;
    WeightLoad: number;
    Width: number;
    Length: number;
    Passenger: number;
    Remark: string;
    StatusNo: string;
    RoutineCount: number;
    RoutineDay: number;
    RoutineTransport: boolean;
    JobInfo: string;
    Creator: string;
    Modifyer: string;
    CompanyID: number;
    BranchName: string;

}

export class TransportData implements ITransportData {
    TransportId: number;
    CarId: number;
    TrailerId: number;
    CarTypeId: number;
    TransportNo: string;
    ContactSource: number;
    ContactDestination: number;
    EmployeeRequestCode: string;
    EmployeeDrive: number;
    EmployeeDriveCode: string;
    TransDate: Date;
    StringTransDate: string;
    TransTime: string;
    FinalDate: Date;
    FinalTime: string;
    TransportTypeId: number;
    TransportInformation: string;
    WeightLoad: number;
    Width: number;
    Length: number;
    Passenger: number;
    Remark: string;
    StatusNo: string;
    RoutineCount: number;
    RoutineDay: number;
    RoutineTransport: boolean;
    JobInfo: string;
    Creator: string;
    Modifyer: string;
    CompanyID: number;
    BranchName: string;

}