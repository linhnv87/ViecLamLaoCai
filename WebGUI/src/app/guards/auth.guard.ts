import { inject, Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { LOCAL_STORAGE_KEYS } from '../utils/constants';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  router = inject(Router);
  canActivate(): boolean {
    const userInfo = localStorage.getItem(LOCAL_STORAGE_KEYS.USER_INFO);
    if (!userInfo) {
      this.router.navigate(['/auth']);
    }
    return !!userInfo;
  }
}
