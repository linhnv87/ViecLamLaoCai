import { Pipe, PipeTransform } from '@angular/core';
import { GetAllInfoUserModel } from 'src/app/models/user.model';

@Pipe({
  name: 'filterSelectedUser',
})
export class FilterSelectedUserPipe implements PipeTransform {
  transform(
    users: GetAllInfoUserModel[],
    selectedUser: GetAllInfoUserModel[],
  ): GetAllInfoUserModel[] {
    return users.filter(
      user => !selectedUser.some(selectedUser => selectedUser.userId === user.userId),
    );
  }
}
