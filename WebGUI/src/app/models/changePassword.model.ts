type Guid = string;
export interface ChangePasswordModel {
    userId: Guid;
    oldPassword : string;
    newPassword : string;
}