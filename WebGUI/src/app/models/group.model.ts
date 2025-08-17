export interface GroupModel {
  id?: number;
  groupName: string;
  isActive?: boolean;
  isSMS?: boolean;
}

export interface GroupDetailInputModel extends GroupModel {
  groupDetails: { userId: string }[];
}
