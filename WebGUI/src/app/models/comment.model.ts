export interface CommentModel {
    id : number;
    comment?: string;
    docId?: number;
    created?: Date;
    modified?: Date;
    deleted?: boolean;
    modifiedBy?: string;
    createdBy?: string;
    userId?: string;
    userName?: string;
}