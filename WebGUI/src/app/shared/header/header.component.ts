import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd, NavigationStart } from '@angular/router';
import { LOCAL_STORAGE_KEYS, ROLES } from '../../utils/constants';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit, OnDestroy {
  isLoggedIn = false;
  userInfo: any = null;
  isCandidatePage = false;
  private routerSubscription!: Subscription;

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.checkAuthStatus();
    this.checkCurrentRoute();
    this.routerSubscription = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd || event instanceof NavigationStart)
    ).subscribe((event) => {
      if (event instanceof NavigationEnd) {
        console.log('Navigation completed to:', event.url);
        this.checkAuthStatus();
        this.checkCurrentRoute();
      }
      if (event instanceof NavigationStart) {
        console.log('Navigation started to:', event.url);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  checkAuthStatus(): void {
    const userInfoString = localStorage.getItem(LOCAL_STORAGE_KEYS.USER_INFO);
    if (userInfoString) {
      this.userInfo = JSON.parse(userInfoString);
      this.isLoggedIn = true;
    } else {
      this.isLoggedIn = false;
      this.userInfo = null;
    }
  }

  checkCurrentRoute(): void {
    const currentUrl = this.router.url;
    this.isCandidatePage = currentUrl.includes('register-candidate') || currentUrl.includes('candidate');
  }

  navigateToHome(): void {
    console.log('Navigating to home with page reload');
    window.location.href = '/website/home';
  }

  navigateToAuth(): void {
    console.log('Navigating to auth with page reload');
    window.location.href = '/auth';
  }

  navigateToDashboard(): void {
    console.log('Navigating to dashboard with page reload');
    window.location.href = '/website/dashboard';
  }
  navigateToChangePassword(): void {
    console.log('Navigating to dashboard with page reload');
    window.location.href = '/website/change-password';
  }
 
  logout(): void {
    localStorage.removeItem(LOCAL_STORAGE_KEYS.USER_INFO);
    this.isLoggedIn = false;
    this.userInfo = null;
    window.location.href = '/website/home';
  }

  getRoleDisplayName(): string {
    if (!this.userInfo || !this.userInfo.roles || !this.userInfo.roles.length) {
      return 'Người dùng';
    }

    const mainRoles = [ROLES.UNG_VIEN, ROLES.DOANH_NGHIEP, ROLES.CO_QUAN_QUAN_LY, ROLES.ADMIN];
    const userRole = this.userInfo.roles.find((role: string) => mainRoles.includes(role));
    
    if (!userRole) {
      return 'Người dùng';
    }

    switch (userRole) {
      case ROLES.UNG_VIEN:
        return 'Ứng viên';
      case ROLES.DOANH_NGHIEP:
        return 'Doanh nghiệp';
      case ROLES.CO_QUAN_QUAN_LY:
        return 'Cơ quan quản lý';
      case ROLES.ADMIN:
        return 'Quản trị viên';
      default:
        return 'Người dùng';
    }
  }
}
