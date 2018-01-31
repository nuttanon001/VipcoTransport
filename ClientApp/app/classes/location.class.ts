export interface ILocation {
    LocationId: number;
    LocationCode: string;
    LocationName: string;
    LocationAddress: string;
    Creator: string;
    Modifyer: string;
}

export class Location implements ILocation {
    LocationId: number;
    LocationCode: string;
    LocationName: string;
    LocationAddress: string;
    Creator: string;
    Modifyer: string;
}