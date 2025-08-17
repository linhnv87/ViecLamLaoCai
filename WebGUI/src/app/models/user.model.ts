export interface UserLogInModel {
  userName: string;
  password: string;
}

export interface UserSignUpModel {
  username: string;
  password: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  roleIds: string[];
  fieldIds: number[];
  departmentId?: number | null;
  unitId?: number | null;
}
export interface GetAllInfoUserModel {
  userId: string;
  userName: string;
  userFullName: string;
  email: string;
  phoneNumber: string;
  createDate: Date;
  lastloginDate: Date;
  isApproved: boolean;
  isLockedout: boolean;
  roles?: SimplerRoleTempModel[];
  fieldIds?: number[];
  roleDescription?: string;
  departmentId?: number | null;
  unitId?: number | null;
}

export interface SimplerRoleTempModel {
  roleId: string;
  description: string;
}

export interface CreateAccount {
  username: string;
  password: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  roleId: [];
}
export interface TokenResponseModel {
  userName: string;
  userId: string;
  accessToken: string;
  displayName: string;
  tokenExpiration: Date;
  roles: string[];
  selectedRole: string[];
  fieldIds: number[];
}
