import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { Router } from '@angular/router';
import * as common from 'src/app/utils/commonFunctions';
import { LoadingService } from 'src/app/services/loading.service';
@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
})
export class AdminComponent implements OnInit {
  constructor(
    private userService: UserService,
    private router: Router,
    private loadingService: LoadingService,
  ) {}
  showMobileMenu = false;
  showDetails = false;
  userRoles = common.GetRoleInfo();
  userId = common.GetCurrentUserId();
  isLoading = false;
  counter = {
    all: 0,
    draft: 0,
    pending: 0,
    approved: 0,
    approvedBonus: 0,
    declined: 0,
    overdued: 0,
  };
  public currentYear = new Date().getFullYear();

  userInfo = common.GetCurrentUserInfo();

  ngOnInit(): void {
    this.loadingService.getLoading().subscribe(status => {
      this.isLoading = status;
    });
    this.reloadOnDemand();
  }

  reloadOnDemand(): void {
    const pageReloaded = localStorage.getItem('pageReloaded');
    if (pageReloaded == 'false' || !pageReloaded) {
      window.location.reload();
      localStorage.setItem('pageReloaded', 'true');
    }
  }

  toggleMobileMenu() {
    this.showMobileMenu = !this.showMobileMenu;
  }

  logOut() {
    this.userService.LogOut();
    this.router.navigate(['/auth']);
  }

  toggle() {
    this.showDetails = !this.showDetails;
  }
}
