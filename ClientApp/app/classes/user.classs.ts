export interface IUser {
    UserId: number;
    Username: string;
    Password: string;
    FirstName: string;
    LastName: string;
    Role: number;
    EmployeeCode: string;
    MailAddress: string;
    CompanyId: number;
    Creator: string;
    Modifyer: string;
}

export class User implements IUser {
    UserId: number;
    Username: string;
    Password: string;
    FirstName: string;
    LastName: string;
    Role: number;
    EmployeeCode: string;
    MailAddress: string;
    CompanyId: number;
    Creator: string;
    Modifyer: string;
}