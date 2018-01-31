export interface ICarBrand {
    BrandId: number;
    BrandName: string;
    BrandDesc: string;
    Creator: string;
    Modifyer: string;
}

export class CarBrand implements ICarBrand {
    BrandId: number;
    BrandName: string;
    BrandDesc: string;
    Creator: string;
    Modifyer: string;
}