export interface DocumentFileModel{
    id: number;
    fileName: string;
    filePath: string;
    docId: number;
    version: number;
    userId: string;
    modified: Date;
    deleted: boolean;
    modifiedBy: string;
    createdBy: string;
    created: Date;
    filePathToView?: string;
    isFinal?: boolean;
    fileType?: number;
}