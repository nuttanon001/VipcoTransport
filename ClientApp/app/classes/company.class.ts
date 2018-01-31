export interface ICompany {
    CompanyId: number;
    CompanyCode: string;
    CompanyName: string;
    Address1: string;
    Address2: string;
    Telephone: string;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;
}

export class Company implements ICompany {
    CompanyId: number;
    CompanyCode: string;
    CompanyName: string;
    Address1: string;
    Address2: string;
    Telephone: string;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;

    constructor() {
        this.CompanyId = 0;
    }
}