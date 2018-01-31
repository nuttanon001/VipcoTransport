export interface IUserHasCompany {
    UserHasCompanyId : number ;
    UserId : number ;
    CompanyId : number ;
    EmailAlert : boolean;
    Creator : string;
    CreateDate : Date;
    Modifyer: string;
    ModifyDate : Date;
}

export class UserHasCompany implements IUserHasCompany {
    UserHasCompanyId: number;
    UserId: number;
    CompanyId: number;
    EmailAlert: boolean;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;

    constructor() {
        this.CompanyId = 0;
    }
}