export interface IReqHasCompany {
    ReqHasCompanyId: number;
    TransportRequestId: number;
    CompanyId: number;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;
}

export class ReqHasCompany implements IReqHasCompany {
    ReqHasCompanyId: number;
    TransportRequestId: number;
    CompanyId: number;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;

    constructor() {
        this.ReqHasCompanyId = 0;
    }
}