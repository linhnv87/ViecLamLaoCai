import { Component, OnInit } from '@angular/core';
import { LOCAL_STORAGE_KEYS } from '../../utils/constants';
import { WebsiteMenuModel, getActiveMenuItems, getMenuItemsByRole } from '../../config/website-menu';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { GetCurrentUserRoles, GetRoleInfo } from '../../utils/commonFunctions';

@Component({
  selector: 'app-dashboard-sidebar',
  templateUrl: './dashboard-sidebar.component.html',
  styleUrls: ['./dashboard-sidebar.component.scss']
})
export class DashboardSidebarComponent implements OnInit {
  userInfo: any = null;
  menuItems: WebsiteMenuModel[] = [];
  currentUrl = '';
  userRoleInfo: any = null;

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.loadMenuItems();
    this.currentUrl = this.router.url;
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.currentUrl = event.urlAfterRedirects || event.url || '';
      });
  }

  loadUserInfo(): void {
    const userInfoString = localStorage.getItem(LOCAL_STORAGE_KEYS.USER_INFO);
    if (userInfoString) {
      this.userInfo = JSON.parse(userInfoString);
    }
    this.userRoleInfo = GetRoleInfo();
  }

  loadMenuItems(): void {
    const userRoles = GetCurrentUserRoles();
    if (userRoles && userRoles.length > 0) {
      this.menuItems = getMenuItemsByRole(userRoles);
    } else {
      this.menuItems = getActiveMenuItems();
    }
  }

  isActive(path: string | undefined): boolean {
    if (!path) return false;
    const normalize = (u: string) => (u || '').split('?')[0].split('#')[0];
    const cur = normalize(this.currentUrl);
    const target = normalize(path);
    return cur === target || cur.startsWith(target + '/');
  }

  navigateToPage(path: string | undefined): void {
    if (path && path !== '#') {
      window.location.href = path;
    }
  }

  getUserMainRole(): string {
    if (this.userRoleInfo?.admin) return 'Admin';
    if (this.userRoleInfo?.ungVien) return 'Ứng viên';
    if (this.userRoleInfo?.doanhNghiep) return 'Doanh nghiệp';
    if (this.userRoleInfo?.coQuanQuanLy) return 'Cơ quan quản lý nhà nước';
    return 'Người dùng';
  }
}
