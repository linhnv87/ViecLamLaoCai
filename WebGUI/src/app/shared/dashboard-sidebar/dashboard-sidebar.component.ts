import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { LOCAL_STORAGE_KEYS } from '../../utils/constants';
import { WebsiteMenuModel, getActiveMenuItems, getMenuItemsByRole } from '../../config/website-menu';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { GetCurrentUserRoles, GetRoleInfo, GetCurrentUserId } from '../../utils/commonFunctions';
import { EmailVerificationService } from '../../services/website/email-verification.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-dashboard-sidebar',
  templateUrl: './dashboard-sidebar.component.html',
  styleUrls: ['./dashboard-sidebar.component.scss']
})
export class DashboardSidebarComponent implements OnInit, OnChanges {
  @Input() dashboardVerificationStatus: 'none' | 'pending' | 'approved' | 'rejected' = 'none';
  
  userInfo: any = null;
  menuItems: WebsiteMenuModel[] = [];
  currentUrl = '';
  userRoleInfo: any = null;
  
  isBusinessUser: boolean = false;
  isVerified: boolean = false;
  verificationStatus: 'none' | 'pending' | 'approved' | 'rejected' = 'none';

  constructor(
    private router: Router,
    private emailVerificationService: EmailVerificationService
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.loadMenuItems();
    this.currentUrl = this.router.url;
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.currentUrl = event.urlAfterRedirects || event.url || '';
      });
    
    this.isBusinessUser = this.userRoleInfo?.doanhNghiep || false;
    if (this.isBusinessUser) {
      this.checkVerificationStatus();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['dashboardVerificationStatus'] && this.dashboardVerificationStatus) {
      console.log('🔄 Sidebar received verification status update:', this.dashboardVerificationStatus);
      this.verificationStatus = this.dashboardVerificationStatus;

      switch (this.dashboardVerificationStatus) {
        case 'pending':
          this.isVerified = false;
          break;
        case 'rejected':
          this.isVerified = false;
          break;
        case 'approved':
          this.isVerified = true;
          break;
        default:
          this.isVerified = false;
          break;
      }
      
      console.log('✅ Sidebar verification status updated:', this.verificationStatus, 'isVerified:', this.isVerified);
    }
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

  private checkVerificationStatus(): void {
    const userId = GetCurrentUserId();
    if (userId) {
      this.emailVerificationService.getVerificationStatusByUserId(userId.toString()).subscribe({
        next: (response: any) => {
          if (response.isSuccess && response.result) {
            const verificationData = response.result;
            const status = verificationData.status;
            
            switch (status?.toLowerCase()) {
              case 'pending':
                this.verificationStatus = 'pending';
                this.isVerified = false;
                break;
              case 'approved':
                this.verificationStatus = 'approved';
                this.isVerified = true;
                break;
              case 'rejected':
                this.verificationStatus = 'rejected';
                this.isVerified = false;
                break;
              case 'not_verified':
              default:
                this.verificationStatus = 'none';
                this.isVerified = false;
                break;
            }
            
            console.log('🔍 Full verification status from API:', status, 'mapped to:', this.verificationStatus);
          } else {
            this.fallbackToSimpleVerificationCheck(userId);
          }
        },
        error: (error: any) => {
          console.error('Error checking full verification status:', error);
          this.fallbackToSimpleVerificationCheck(userId);
        }
      });
    }
  }

  private fallbackToSimpleVerificationCheck(userId: number): void {
    this.emailVerificationService.isCompanyVerifiedByUserId(userId.toString()).subscribe({
      next: (verifiedResponse: any) => {
        if (verifiedResponse.isSuccess) {
          this.isVerified = verifiedResponse.result;
          this.verificationStatus = this.isVerified ? 'approved' : 'none';
          console.log('🔍 Fallback verification check result:', this.isVerified);
        }
      },
      error: (error: any) => {
        console.error('Error in fallback verification check:', error);
        this.isVerified = false;
        this.verificationStatus = 'none';
      }
    });
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
      if (this.isBusinessUser && !this.isVerified && this.shouldShowWarning(path)) {
        this.showVerificationWarning();
        return;
      }
      
      window.location.href = path;
    }
  }

  private shouldShowWarning(path: string | undefined): boolean {
    if (!path) return false;
    
    const restrictedPaths = [
      '/website/find-candidates',
      '/website/applications',
      '/website/labor-market-report1',
      '/website/labor-market-report3'
    ];
    return restrictedPaths.includes(path);
  }

  private showVerificationWarning(): void {
    console.log('⚠️ Showing verification warning');
    
    Swal.fire({
      title: '⚠️ Cần xác thực doanh nghiệp',
      text: 'Tài khoản của bạn đang chờ xác thực thông tin doanh nghiệp. Vui lòng hoàn tất quá trình xác thực trước khi sử dụng tính năng này!',
      icon: 'warning',
      confirmButtonText: 'Đóng',
      confirmButtonColor: '#3085d6',
      timer: 8000,
      timerProgressBar: true,
      showCloseButton: true
    });
  }

  getVerificationStatusText(): string {
    switch (this.verificationStatus) {
      case 'approved':
        return '✅ Đã xác thực';
      case 'pending':
        return '⏳ Đang chờ xác thực';
      case 'rejected':
        return '❌ Bị từ chối';
      default:
        return '⚠️ Chưa xác thực';
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
