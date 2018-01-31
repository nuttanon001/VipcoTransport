export interface ICarData {
    CarId: number;
    CarTypeId: number;
    CarNo: string;
    CarBrand: number;
    RegisterNo: string;
    CarDate: Date;
    CarWeight: number;
    Insurance: string;
    InsurDate: Date;
    ActInsurance: string;
    ActDate: Date;
    Remark: string;
    ImagesString: string;
    Status: boolean;
    Creator: string;
    Modifyer: string;
    CompanyID: number;
}

export class CarData implements ICarData {
    CarId: number;
    CarTypeId: number;
    CarNo: string;
    CarBrand: number;
    RegisterNo: string;
    CarDate: Date;
    CarWeight: number;
    Insurance: string;
    InsurDate: Date;
    ActInsurance: string;
    ActDate: Date;
    Remark: string;
    ImagesString: string;
    Status: boolean;
    Creator: string;
    Modifyer: string;
    CompanyID: number;
}