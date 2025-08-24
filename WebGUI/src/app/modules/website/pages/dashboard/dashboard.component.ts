import { Component, OnInit } from '@angular/core';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { GetRoleInfo, GetCurrentUserId } from '../../../../utils/commonFunctions';
import { DashboardService } from '../../../../services/website/dashboard.service';
import { BusinessDashboardModel, CandidateDashboardModel, AdminDashboardModel } from '../../../../models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  roleInfo: any;
  
  businessDashboardData: BusinessDashboardModel | null = null;
  candidateDashboardData: CandidateDashboardModel | null = null;
  adminDashboardData: AdminDashboardModel | null = null;

  doanhNghiepStats = {
    totalProfileViews: 245,
    totalJobs: 3,
    totalApplications: 55,
    todayApplications: 23
  };

  get doanhNghiepTotal() {
    return this.doanhNghiepStats.totalProfileViews + this.doanhNghiepStats.totalJobs + this.doanhNghiepStats.totalApplications + this.doanhNghiepStats.todayApplications;
  }

  ungVienStats = {
    suitableJobs: 74,
    profileViews: 0,
    employerEmails: 0,
    totalCVs: 0
  };

  coQuanQuanLyStats = {
    pendingApprovals: 12,
    approvedBusinesses: 45,
    rejectedBusinesses: 3,
    totalBusinesses: 60
  };

  // Dummy data
  dashboardStats = {
    totalJobs: 1247,
    newApplications: 89,
    activeJobs: 156,
    totalViews: 12450,
    todayApplications: 23,
    todayViews: 567
  };

  // Dummy applied jobs data for testing
  dummyAppliedJobs = [
    {
      id: 1,
      jobTitle: 'Frontend Developer',
      company: 'Công ty ABC Tech',
      logo: 'assets/vieclamlaocai/img/company-logo-1.png',
      salary: '15 - 25 triệu',
      status: 'pending',
      appliedDate: '2024-01-15',
      location: 'Hà Nội'
    },
    {
      id: 2,
      jobTitle: 'Backend Developer',
      company: 'Công ty XYZ Software',
      logo: 'assets/vieclamlaocai/img/company-logo-2.png',
      salary: '20 - 35 triệu',
      status: 'reviewing',
      appliedDate: '2024-01-14',
      location: 'TP. Hồ Chí Minh'
    },
    {
      id: 3,
      jobTitle: 'UI/UX Designer',
      company: 'Công ty Design Pro',
      logo: 'assets/vieclamlaocai/img/company-logo-3.png',
      salary: '18 - 30 triệu',
      status: 'accepted',
      appliedDate: '2024-01-13',
      location: 'Đà Nẵng'
    }
  ];



  recentJobs = [
    {
      id: 1,
      title: 'Frontend Developer',
      company: 'Công ty ABC Tech',
      applications: 45,
      views: 234,
      postedDate: '2024-01-15',
      status: 'active',
      urgent: true
    },
    {
      id: 2,
      title: 'Backend Developer',
      company: 'Công ty XYZ Software',
      applications: 28,
      views: 189,
      postedDate: '2024-01-14',
      status: 'active',
      urgent: false
    },
    {
      id: 3,
      title: 'UI/UX Designer',
      company: 'Công ty Design Pro',
      applications: 67,
      views: 345,
      postedDate: '2024-01-13',
      status: 'paused',
      urgent: false
    }
  ];

  recentCandidates = [
    {
      id: 1,
      name: 'Nguyễn Văn A',
      position: 'Frontend Developer',
      experience: '3 năm',
      education: 'Đại học Bách Khoa',
      appliedDate: '2024-01-15',
      status: 'new',
      avatar: 'assets/vieclamlaocai/img/image 16.png'
    },
    {
      id: 2,
      name: 'Trần Thị B',
      position: 'Backend Developer',
      experience: '5 năm',
      education: 'Đại học Công nghệ',
      appliedDate: '2024-01-14',
      status: 'reviewed',
      avatar: 'assets/vieclamlaocai/img/image 23.png'
    }
  ];

  constructor(
    private splashScreenService: SplashScreenService,
    private dashboardService: DashboardService
  ) {}

  ngOnInit(): void {
    console.log('Dashboard component loaded - fresh data loading...');
    this.roleInfo = GetRoleInfo();
    this.showLoadingSplash();
    this.loadDashboardData();
  }

  // Check if user has specific role
  isDoanhNghiep(): boolean {
    return this.roleInfo?.doanhNghiep || false;
  }

  isUngVien(): boolean {
    return this.roleInfo?.ungVien || false;
  }

  isCoQuanQuanLy(): boolean {
    return this.roleInfo?.coQuanQuanLy || false;
  }

  // Get appropriate statistics based on role
  getStatisticsForRole() {
    if (this.isDoanhNghiep()) {
      return this.doanhNghiepStats;
    } else if (this.isUngVien()) {
      return this.ungVienStats;
    } else if (this.isCoQuanQuanLy()) {
      return this.coQuanQuanLyStats;
    }
    return this.dashboardStats; // Default fallback
  }

  // Safe getters for template
  get recentCandidatesData() {
    return this.businessDashboardData?.recentCandidates || [];
  }

  get savedJobsData() {
    return this.candidateDashboardData?.savedJobs || [];
  }

  get hasRecentCandidates(): boolean {
    return !!(this.businessDashboardData?.recentCandidates && this.businessDashboardData.recentCandidates.length > 0);
  }

  get hasSavedJobs(): boolean {
    return !!(this.candidateDashboardData?.savedJobs && this.candidateDashboardData.savedJobs.length > 0);
  }

  get appliedJobsData() {
    return this.candidateDashboardData?.appliedJobs || this.dummyAppliedJobs;
  }

  get hasAppliedJobs(): boolean {
    return !!(this.candidateDashboardData?.appliedJobs && this.candidateDashboardData.appliedJobs.length > 0);
  }

  // Get section title based on role
  getSectionTitle(): string {
    if (this.isDoanhNghiep()) {
      return 'Danh sách ứng viên phù hợp';
    } else if (this.isUngVien()) {
      return '';
    } else if (this.isCoQuanQuanLy()) {
      return 'Thống kê phê duyệt hôm nay';
    }
    return 'Danh sách ứng viên phù hợp';
  }

  showLoadingSplash(): void {
    this.splashScreenService.show({
      type: 'loading',
      title: 'Đang tải dashboard...',
      message: 'Đang cập nhật thống kê mới nhất',
      showProgress: true
    });
  }

  loadDashboardData(): void {
    console.log('Loading fresh dashboard data...');
    
    const userId = GetCurrentUserId();
    
    // Load data based on user role
    if (this.isDoanhNghiep()) {
      this.loadBusinessDashboard(userId);
    } else if (this.isUngVien()) {
      this.loadCandidateDashboard(userId);
    } else if (this.isCoQuanQuanLy()) {
      this.loadAdminDashboard();
    } else {
      // Default fallback
      this.loadBusinessDashboard(userId);
    }
  }

  private loadBusinessDashboard(businessId: number): void {
    this.dashboardService.getBusinessDashboardDummy().subscribe({
      next: (response) => {
        if (response.isSuccess && response.result) {
          this.businessDashboardData = response.result;
          this.updateDoanhNghiepStats();
          this.showSuccessSplash();
        }
      },
      error: (error) => {
        console.error('Error loading business dashboard:', error);
        this.showSuccessSplash();
      }
    });
  }

  private loadCandidateDashboard(candidateId: number): void {
    this.dashboardService.getCandidateDashboardDummy().subscribe({
      next: (response) => {
        if (response.isSuccess && response.result) {
          this.candidateDashboardData = response.result;
          this.updateUngVienStats();
          this.showSuccessSplash();
        }
      },
      error: (error) => {
        console.error('Error loading candidate dashboard:', error);
        this.showSuccessSplash();
      }
    });
  }

  private loadAdminDashboard(): void {
    this.dashboardService.getAdminDashboardDummy().subscribe({
      next: (response) => {
        if (response.isSuccess && response.result) {
          this.adminDashboardData = response.result;
          this.updateCoQuanQuanLyStats();
          this.showSuccessSplash();
        }
      },
      error: (error) => {
        console.error('Error loading admin dashboard:', error);
        this.showSuccessSplash();
      }
    });
  }

  private updateDoanhNghiepStats(): void {
    if (this.businessDashboardData) {
      this.doanhNghiepStats = {
        totalProfileViews: this.businessDashboardData.totalViews || 245,
        totalJobs: this.businessDashboardData.totalJobs || 3,
        totalApplications: this.businessDashboardData.totalApplications || 55,
        todayApplications: this.businessDashboardData.todayApplications || 23
      };
    }
  }

  private updateUngVienStats(): void {
    if (this.candidateDashboardData) {
      this.ungVienStats = {
        suitableJobs: this.candidateDashboardData.suitableJobs || 74,
        profileViews: this.candidateDashboardData.profileViews || 0,
        employerEmails: this.candidateDashboardData.employerEmails || 0,
        totalCVs: this.candidateDashboardData.totalCVs || 0
      };
    }
  }

  private updateCoQuanQuanLyStats(): void {
    if (this.adminDashboardData) {
      this.coQuanQuanLyStats = {
        pendingApprovals: this.adminDashboardData.pendingApprovals || 12,
        approvedBusinesses: this.adminDashboardData.approvedBusinesses || 45,
        rejectedBusinesses: this.adminDashboardData.rejectedBusinesses || 3,
        totalBusinesses: this.adminDashboardData.totalBusinesses || 60
      };
    }
  }

  private showSuccessSplash(): void {
    this.splashScreenService.show({
      type: 'success',
      title: 'Dashboard đã sẵn sàng!',
      message: 'Thống kê và dữ liệu đã được cập nhật'
    });

    setTimeout(() => {
      this.splashScreenService.hide();
    }, 1500);
  }

  getJobStatusText(status: string): string {
    switch(status) {
      case 'active': return 'Đang tuyển';
      case 'paused': return 'Tạm dừng';
      case 'closed': return 'Đã đóng';
      default: return status;
    }
  }

  getCandidateStatusText(status: string): string {
    switch(status) {
      case 'new': return 'Mới';
      case 'reviewed': return 'Đã xem';
      case 'contacted': return 'Đã liên hệ';
      default: return status;
    }
  }

  getStatusText(status: string): string {
    switch(status) {
      case 'pending': return 'Chờ xử lý';
      case 'reviewing': return 'Đang xem xét';
      case 'accepted': return 'Đã chấp nhận';
      case 'rejected': return 'Đã từ chối';
      default: return status;
    }
  }

  // Chart helper methods
  getBarHeight(value: number, total: number): number {
    if (total === 0) return 0;
    return Math.max((value / total) * 100, 10);
  }

  // Date helper methods
  getCurrentDate(): string {
    const today = new Date();
    const options: Intl.DateTimeFormatOptions = { 
      weekday: 'long', 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    };
    return today.toLocaleDateString('vi-VN', options);
  }


}