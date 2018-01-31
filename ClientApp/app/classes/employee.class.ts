export interface IEmployee {
    EmpCode: string;
    NameThai: string;
    NameEng: string;
    SectionCode: string;
    SectionName: string;
}

export class Employee implements IEmployee {
    EmpCode: string;
    NameThai: string;
    NameEng: string;
    SectionCode: string;
    SectionName: string;
}