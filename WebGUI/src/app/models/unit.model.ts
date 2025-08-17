export interface UnitModel {
    id: number;
    name: string;
    isActive?: boolean;
    modified ?: Date;
    deleted ?: boolean;
    modifiedBy ?: string;
    createdBy ?: string;
    created ?: Date;
  }
  