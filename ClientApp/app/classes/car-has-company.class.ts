export interface ICarHasCompany {
    CarHasCompanyId: number;
    CarId: number;
    CompanyId: number;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;
}

export class CarHasCompany implements ICarHasCompany {
    CarHasCompanyId: number;
    CarId: number;
    CompanyId: number;
    Creator: string;
    CreateDate: Date;
    Modifyer: string;
    ModifyDate: Date;

    constructor() {
        this.CarHasCompanyId = 0;
    }
}