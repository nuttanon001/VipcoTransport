export interface ITrailer {
    TrailerId: number;
    TrailerNo: string;
    CarTypeId: number;
    Insurance: string;
    InsurDate?: Date;
    TrailerBrand: number;
    RegisterNo: string;
    TrailerDesc: string;
    TrailerWeight: number;
    TrailerLoad: number;
    ImagesString: string;
    Remark: string;
    Creator: string;
    Modifyer: string;
    CompanyID: number;
}

export class Trailer implements ITrailer {
    TrailerId: number;
    TrailerNo: string;
    CarTypeId: number;
    Insurance: string;
    InsurDate?: Date;
    TrailerBrand: number;
    RegisterNo: string;
    TrailerDesc: string;
    TrailerWeight: number;
    TrailerLoad: number;
    ImagesString: string;
    Remark: string;
    Creator: string;
    Modifyer: string;
    CompanyID: number;
}