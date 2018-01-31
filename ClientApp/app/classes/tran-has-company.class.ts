export interface ITranHasCompany {
    TranHasCompanyId: number;
    TransportId: number;
    CompanyId: number;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;
}

export class TranHasCompany implements ITranHasCompany {
    TranHasCompanyId: number;
    TransportId: number;
    CompanyId: number;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;

    constructor() {
        this.TranHasCompanyId = 0;
    }
}