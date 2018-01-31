export interface ITruckHasCompany {
    TruckHasCompanyId: number;
    TrailerId: number;
    CompanyId: number;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;
}

export class TruckHasCompany implements ITruckHasCompany {
    TruckHasCompanyId: number;
    TrailerId: number;
    CompanyId: number;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;

    constructor() {
        this.TruckHasCompanyId = 0;
    }
}