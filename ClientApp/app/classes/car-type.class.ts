export interface ICarType {
    CarTypeId: number;
    CarTypeNo: string;
    CarTypeDesc: string;
    Type: number;
    Remark: string;
    Creator: string;
    Modifyer: string;
}

export class CarType implements ICarType {
    // add propertity here
    CarTypeId: number;
    CarTypeNo: string;
    CarTypeDesc: string;
    Type: number;
    Remark: string;
    Creator: string;
    Modifyer: string;
}