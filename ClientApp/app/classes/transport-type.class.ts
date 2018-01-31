export interface ITransportType {
    TransportTypeId: number;
    TransportTypeDesc: string;
    Remark: string;
    Property: number;
    Creator: string;
    Modifyer: string;
}

export class TransportType implements ITransportType {
    TransportTypeId: number;
    TransportTypeDesc: string;
    Remark: string;
    Property: number;
    Creator: string;
    Modifyer: string;
}