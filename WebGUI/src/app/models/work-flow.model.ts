import { SimplerRoleTempModel } from './user.model';

export interface ReviewerWorkflowModel {
  id?: number;
  usersDto?: UserWorkFlowModel[];
  name?: string;
  prevWorkflowId?: number;
  nextWorkflowId?: number;
  defaultUserId?: string;
  isSign?: boolean;
  description?: string;
  statusId?: number;
  state?: string;
  userId?: string;
}

export interface UserWorkFlowModel {
  userId?: string;
  userFullName?: string;
  isDefault?: boolean;
  roles?: SimplerRoleTempModel[];
  roleDescription?: string;
}
export interface WorkFLowDetailInputModel extends ReviewerWorkflowModel {
  usersDto: { userId: string }[];
}
