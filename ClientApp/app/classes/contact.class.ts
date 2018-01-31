export interface IContact {
    ContactId: number;
    Location: number;
    ContactName: string;
    ContactPhone: string;
    Creator: string;
    Modifyer: string;
}

export class Contact implements IContact {
    ContactId: number;
    Location: number;
    ContactName: string;
    ContactPhone: string;
    Creator: string;
    Modifyer: string;
}