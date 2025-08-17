import { Pipe, PipeTransform } from '@angular/core';
import { GetAllInfoUserModel } from '../models/user.model';

@Pipe({
  name: 'filterUser',
})
export class FilterUserPipe implements PipeTransform {
  transform(value: any, listUser: GetAllInfoUserModel[], key?: string): GetAllInfoUserModel[] {
    if (!key) {
      return listUser;
    }
    return listUser.filter(user => user.userFullName.toLowerCase().includes(key.toLowerCase()));
  }
}
