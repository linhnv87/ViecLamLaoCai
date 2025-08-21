import { Component, OnInit } from '@angular/core';
import { LOCAL_STORAGE_KEYS } from '../../utils/constants';
import { WebsiteMenuModel, getActiveMenuItems } from '../../config/website-menu';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard-sidebar',
  templateUrl: './dashboard-sidebar.component.html',
  styleUrls: ['./dashboard-sidebar.component.scss']
})
export class DashboardSidebarComponent implements OnInit {
  userInfo: any = null;
  menuItems: WebsiteMenuModel[] = [];
  currentUrl = '';

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
  }

  loadMenuItems(): void {
    this.menuItems = getActiveMenuItems();
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
}
