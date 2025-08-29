import { Component, OnInit } from '@angular/core';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { GetRoleInfo, GetCurrentUserId } from '../../../../utils/commonFunctions';
import { DashboardService } from '../../../../services/website/dashboard.service';
import { BusinessDashboardModel, CandidateDashboardModel, AdminDashboardModel } from '../../../../models/dashboard.model';
import { EmailVerificationService, BusinessVerificationResponse } from '../../../../services/website/email-verification.service';
import Swal from 'sweetalert2';

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

  // Email verification modal state
  showEmailVerificationModal: boolean = false;
  isEmailVerified: boolean = false;
  verificationStatus: 'none' | 'pending' | 'approved' | 'rejected' = 'none';
  verificationMessage: string = '';

  constructor(
    private splashScreenService: SplashScreenService,
    private dashboardService: DashboardService,
    private emailVerificationService: EmailVerificationService
  ) {}

  ngOnInit(): void {
    this.roleInfo = GetRoleInfo();
    this.showLoadingSplash();
    this.loadDashboardData();
    this.checkEmailVerificationStatus();
  }

  // Email verification methods
  openEmailVerification(): void {
    console.log('🚀 Opening email verification modal from dashboard');
    this.showEmailVerificationModal = true;
    console.log('📧 showEmailVerificationModal set to:', this.showEmailVerificationModal);
  }

  closeEmailVerificationModal(): void {
    this.showEmailVerificationModal = false;
  }

  onVerificationSubmitted(verificationResponse: BusinessVerificationResponse): void {
    console.log('Verification submitted:', verificationResponse);
    
    this.verificationStatus = 'pending';
    this.isEmailVerified = false;
    this.verificationMessage = 'Yêu cầu xác thực đang chờ phê duyệt';
    
    console.log('🔄 Dashboard verification status updated to:', this.verificationStatus);

    Swal.fire({
      title: 'Thành công!',
      text: `Yêu cầu xác thực đã được gửi thành công! Mã xác thực: ${verificationResponse.verificationCode}`,
      icon: 'success',
      confirmButtonText: 'Đóng',
      confirmButtonColor: '#3085d6',
      timer: 5000,
      timerProgressBar: true
    });

    this.closeEmailVerificationModal();

    if (this.isDoanhNghiep()) {
      console.log('🔄 Reloading dashboard data after verification submission...');
      
      this.forceUIRefresh();
      
      setTimeout(() => {
        this.loadDashboardData();
      }, 500);
    }
  }

  private forceUIRefresh(): void {
    console.log('🔄 Forcing UI refresh...');
    
    const currentStatus = this.verificationStatus;
    this.verificationStatus = 'none';
    
    setTimeout(() => {
      this.verificationStatus = currentStatus;
      console.log('✅ UI refresh completed, verification status:', this.verificationStatus);
    }, 50);
  }

  private checkEmailVerificationStatus(): void {
    if (this.isDoanhNghiep()) {
      const userId = GetCurrentUserId();
      console.log('🔍 Checking verification status for user:', userId);
      
      this.emailVerificationService.getVerificationStatusByUserId(userId.toString()).subscribe({
        next: (response: any) => {
          console.log('📊 Full verification status response:', response);
          if (response.isSuccess && response.result) {
            const verificationData = response.result;
            const status = verificationData.status;
            const wasVerified = this.isEmailVerified;
            
            switch (status?.toLowerCase()) {
              case 'pending':
                this.verificationStatus = 'pending';
                this.isEmailVerified = false;
                this.verificationMessage = 'Đang chờ xác thực doanh nghiệp';
                break;
              case 'approved':
                this.verificationStatus = 'approved';
                this.isEmailVerified = true;
                this.verificationMessage = 'Doanh nghiệp đã được xác thực';
                break;
              case 'rejected':
                this.verificationStatus = 'rejected';
                this.isEmailVerified = false;
                this.verificationMessage = 'Doanh nghiệp bị từ chối xác thực';
                break;
              case 'not_verified':
              default:
                this.verificationStatus = 'none';
                this.isEmailVerified = false;
                this.verificationMessage = 'Chưa xác thực doanh nghiệp';
                break;
            }
            
            console.log('✅ Full verification status:', status, 'mapped to:', this.verificationStatus);
            
            if (wasVerified !== this.isEmailVerified) {
              console.log('🔄 Verification status changed, reloading dashboard data...');
    
              this.loadDashboardData();
            } else if (this.verificationStatus === 'pending' || this.verificationStatus === 'rejected' || this.verificationStatus === 'none') {
              console.log('🔄 Verification status is', this.verificationStatus, '- resetting dashboard data...');
              this.businessDashboardData = null;
              this.resetDoanhNghiepStats();
            }
          } else {
            this.fallbackToSimpleVerificationCheck(userId);
          }
        },
        error: (error: any) => {
          console.error('❌ Error checking full verification status:', error);
          this.fallbackToSimpleVerificationCheck(userId);
        }
      });
    }
  }

  private fallbackToSimpleVerificationCheck(userId: number): void {
    this.emailVerificationService.isCompanyVerifiedByUserId(userId.toString()).subscribe({
      next: (response: any) => {
        if (response.isSuccess) {
          const wasVerified = this.isEmailVerified;
          this.isEmailVerified = response.result;
          this.verificationStatus = this.isEmailVerified ? 'approved' : 'none';
          this.verificationMessage = this.isEmailVerified ? 'Doanh nghiệp đã được xác thực' : 'Chưa xác thực doanh nghiệp';
          console.log('✅ Fallback verification status:', this.isEmailVerified);

          if (wasVerified !== this.isEmailVerified) {
            console.log('🔄 Verification status changed, reloading dashboard data...');
            this.loadDashboardData();
          }
        }
      },
      error: (error: any) => {
        console.error('❌ Error in fallback verification check:', error);
        this.isEmailVerified = false;
        this.verificationStatus = 'none';
        this.verificationMessage = 'Chưa xác thực doanh nghiệp';
      }
    });
  }

  isDoanhNghiep(): boolean {
    return this.roleInfo?.doanhNghiep || false;
  }

  isUngVien(): boolean {
    return this.roleInfo?.ungVien || false;
  }

  isCoQuanQuanLy(): boolean {
    return this.roleInfo?.coQuanQuanLy || false;
  }

  getStatisticsForRole() {
    if (this.isDoanhNghiep()) {
      return this.doanhNghiepStats;
    } else if (this.isUngVien()) {
      return this.ungVienStats;
    } else if (this.isCoQuanQuanLy()) {
      return this.coQuanQuanLyStats;
    }
    return this.dashboardStats; 
  }

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
    
    if (this.isDoanhNghiep()) {
      this.loadBusinessDashboard(userId);
    } else if (this.isUngVien()) {
      this.loadCandidateDashboard(userId);
    } else if (this.isCoQuanQuanLy()) {
      this.loadAdminDashboard();
    } else {
      this.loadBusinessDashboard(userId);
    }
  }

  private loadBusinessDashboard(businessId: number): void {
    console.log('🔄 Loading business dashboard for ID:', businessId, 'verificationStatus:', this.verificationStatus);
    
    if (this.verificationStatus === 'approved') {
      console.log('✅ User is verified, loading real business dashboard data...');
      this.dashboardService.getBusinessDashboard(businessId.toString()).subscribe({
        next: (response) => {
          console.log('📊 Business dashboard response:', response);
          if (response.isSuccess && response.result) {
            this.businessDashboardData = response.result;
            this.updateDoanhNghiepStats();
            console.log('✅ Business dashboard data updated successfully');
            this.showSuccessSplash();
          } else {
            console.warn('⚠️ Business dashboard response not successful:', response);
            this.showSuccessSplash();
          }
        },
        error: (error) => {
          console.error('❌ Error loading business dashboard:', error);
          console.log('❌ Real API failed for verified user, not using dummy data');
          this.showSuccessSplash();
        }
      });
    } else {
      console.log('⚠️ User verification status:', this.verificationStatus, '- not loading dashboard data');
      this.businessDashboardData = null;
      this.resetDoanhNghiepStats();
      this.showSuccessSplash();
    }
  }

  private loadCandidateDashboard(candidateId: number): void {
    console.log('🔄 Loading candidate dashboard for ID:', candidateId);
    
    const dashboardCall = candidateId > 0 
      ? this.dashboardService.getCandidateDashboard(candidateId)
      : this.dashboardService.getCandidateDashboardDummy();
    
    dashboardCall.subscribe({
      next: (response) => {
        console.log('📊 Candidate dashboard response:', response);
        if (response.isSuccess && response.result) {
          this.candidateDashboardData = response.result;
          this.updateUngVienStats();
          console.log('✅ Candidate dashboard data updated successfully');
          this.showSuccessSplash();
        } else {
          console.warn('⚠️ Candidate dashboard response not successful:', response);
          this.showSuccessSplash();
        }
      },
      error: (error) => {
        console.error('❌ Error loading candidate dashboard:', error);
        
        if (candidateId <= 0) {
          console.log('🔄 Using dummy data for invalid candidateId...');
          this.dashboardService.getCandidateDashboardDummy().subscribe({
            next: (fallbackResponse) => {
              if (fallbackResponse.isSuccess && fallbackResponse.result) {
                this.candidateDashboardData = fallbackResponse.result;
                this.updateUngVienStats();
              }
              this.showSuccessSplash();
            },
            error: () => this.showSuccessSplash()
          });
        } else {
          console.log('❌ Real API failed for valid candidateId, not using dummy data');
          this.showSuccessSplash();
        }
      }
    });
  }

  private loadAdminDashboard(): void {
    console.log('🔄 Loading admin dashboard...');
    this.dashboardService.getAdminDashboard().subscribe({
      next: (response) => {
        console.log('📊 Admin dashboard response:', response);
        if (response.isSuccess && response.result) {
          this.adminDashboardData = response.result;
          this.updateCoQuanQuanLyStats();
          console.log('✅ Admin dashboard data updated successfully');
          this.showSuccessSplash();
        } else {
          console.warn('⚠️ Admin dashboard response not successful:', response);
          this.showSuccessSplash();
        }
      },
      error: (error) => {
        console.error('❌ Error loading admin dashboard:', error);
        console.log('❌ Admin API failed, not using dummy data');
        this.showSuccessSplash();
      }
    });
  }

  private updateDoanhNghiepStats(): void {
    if (this.businessDashboardData) {
      this.doanhNghiepStats = {
        totalProfileViews: this.businessDashboardData.totalViews ?? 0,
        totalJobs: this.businessDashboardData.totalJobs ?? 0,
        totalApplications: this.businessDashboardData.totalApplications ?? 0,
        todayApplications: this.businessDashboardData.todayApplications ?? 0
      };
      console.log('📊 Updated stats with real data:', this.doanhNghiepStats);
    }
  }

  private resetDoanhNghiepStats(): void {
    this.doanhNghiepStats = {
      totalProfileViews: 0,
      totalJobs: 0,
      totalApplications: 0,
      todayApplications: 0
    };
    console.log('🔄 Reset business stats to 0 for unverified user');
  }

  private updateUngVienStats(): void {
    if (this.candidateDashboardData) {
      this.ungVienStats = {
        suitableJobs: this.candidateDashboardData.suitableJobs ?? 0,
        profileViews: this.candidateDashboardData.profileViews ?? 0,
        employerEmails: this.candidateDashboardData.employerEmails ?? 0,
        totalCVs: this.candidateDashboardData.totalCVs ?? 0
      };
      console.log('📊 Updated candidate stats with real data:', this.ungVienStats);
    }
  }

  private updateCoQuanQuanLyStats(): void {
    if (this.adminDashboardData) {
      this.coQuanQuanLyStats = {
        pendingApprovals: this.adminDashboardData.pendingApprovals ?? 0,
        approvedBusinesses: this.adminDashboardData.approvedBusinesses ?? 0,
        rejectedBusinesses: this.adminDashboardData.rejectedBusinesses ?? 0,
        totalBusinesses: this.adminDashboardData.totalBusinesses ?? 0
      };
      console.log('📊 Updated admin stats with real data:', this.coQuanQuanLyStats);
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

  getBarHeight(value: number, total: number): number {
    if (total === 0) return 0;
    return Math.max((value / total) * 100, 10);
  }

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