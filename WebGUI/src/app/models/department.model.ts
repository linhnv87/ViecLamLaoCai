export interface DepartmentModel {
  id: number;
  departmentName: string;
  isActive?: boolean;
  modified ?: Date;
  deleted ?: boolean;
  modifiedBy ?: string;
  createdBy ?: string;
  created ?: Date;
}
