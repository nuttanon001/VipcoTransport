export interface IAttachFile{
    AttachId: number;
    AttachFileName: string;
    AttachAddress: string;
    Creator: string;
    Modifyer: string;
}

export class AttachFile implements IAttachFile {
    AttachId: number;
    AttachFileName: string;
    AttachAddress: string;
    Creator: string;
    Modifyer: string;
}