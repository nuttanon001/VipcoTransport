export interface IUserEdition {
    UserId: number;
    EmployeeName: string;
    UserName: string;
    MailAddress: string;
    PasswordOld: string;
    PasswordNew: string;
    PasswordConfirm: string;
    CompanyName: string;
    CompanyID: number;
    EmailAlert: boolean;
    UserHasCompanyId: number;
}