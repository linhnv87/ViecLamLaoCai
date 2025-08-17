import { Pipe, PipeTransform } from '@angular/core';
import { GetCurrentUserRoles } from '../utils/commonFunctions';

@Pipe({
  name: 'permission',
})
export class PermissionPipe implements PipeTransform {
  transform(permissionList?: string | string[]): boolean {
    const currentRoles = GetCurrentUserRoles();
    if (permissionList) {
      if (Array.isArray(permissionList)) {
        return permissionList.some(p => currentRoles.includes(p));
      }
      return currentRoles.includes(permissionList);
    }

    return true;
  }
}
