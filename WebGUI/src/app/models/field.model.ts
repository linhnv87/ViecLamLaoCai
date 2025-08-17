export interface FieldModel {
    id : number;
    title ?: string;
    active ?: boolean;
    modified ?: Date;
    deleted ?: boolean;
    modifiedBy ?: string;
    createdBy ?: string;
    created ?: Date;
    order?: number;
}