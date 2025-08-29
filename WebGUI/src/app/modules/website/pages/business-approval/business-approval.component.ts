import { Component, OnInit } from '@angular/core';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { BusinessService } from '../../../../services/website/business.service';
import { BusinessApprovalData, DocumentInfo, BusinessApprovalStats } from '../../../../models/business.model';

@Component({
  selector: 'app-business-approval',
  templateUrl: './business-approval.component.html',
  styleUrls: ['./business-approval.component.scss']
})
export class BusinessApprovalComponent implements OnInit {

  showDetailModal = false;
  selectedBusiness: BusinessApprovalData | null = null;

  businesses: BusinessApprovalData[] = [];
  approvalStats: BusinessApprovalStats = {
    pending: 0,
    approved: 0,
    rejected: 0,
    reviewing: 0
  };

  // Filtered businesses for display
  filteredBusinesses: BusinessApprovalData[] = [];
  
  // Filter options
  statusFilter = 'all';
  businessTypeFilter = 'all';
  searchKeyword = '';

  // Pagination
  currentPage = 1;
  itemsPerPage = 5;
  totalItems = 0;

  constructor(
    private splashScreenService: SplashScreenService,
    private businessService: BusinessService
  ) {}

  ngOnInit(): void {
    console.log('Business approval component loaded');
    this.loadBusinessData();
    this.loadStatistics();
  }

  loadBusinessData(): void {
    console.log('Loading business approval data...');
    const filter = {
      status: this.statusFilter,
      businessType: this.businessTypeFilter,
      searchKeyword: this.searchKeyword,
      page: this.currentPage,
      pageSize: this.itemsPerPage,
      sortBy: 'registrationDate',
      sortOrder: 'desc'
    };

    this.businessService.getBusinessApprovals(filter).subscribe(res => {
      if (res.isSuccess) {
        this.businesses = res.result.data;
        this.filteredBusinesses = [...this.businesses];
        this.totalItems = res.result.totalItems;
        this.currentPage = res.result.currentPage;
      } else {
        console.error('Failed to load business data:', res.message);
        // Fallback to dummy data if API fails
        this.loadBusinessDataDummy();
      }
    }, error => {
      console.error('Error loading business data:', error);
      // Fallback to dummy data if API fails
      this.loadBusinessDataDummy();
    });
  }

  loadStatistics(): void {
    this.businessService.getApprovalStatistics().subscribe(res => {
      if (res.isSuccess) {
        this.approvalStats = res.result;
      } else {
        console.error('Failed to load statistics:', res.message);
        // Fallback to dummy data if API fails
        this.loadStatisticsDummy();
      }
    }, error => {
      console.error('Error loading statistics:', error);
      // Fallback to dummy data if API fails
      this.loadStatisticsDummy();
    });
  }

  // Fallback methods for development
  private loadBusinessDataDummy(): void {
    this.businessService.getAllDummy().subscribe(res => {
      if (res.isSuccess) {
        this.businesses = res.result;
        this.filteredBusinesses = [...this.businesses];
        this.totalItems = this.businesses.length;
      }
    });
  }

  private loadStatisticsDummy(): void {
    this.businessService.getStatisticsDummy().subscribe(res => {
      if (res.isSuccess) {
        this.approvalStats = res.result;
      }
    });
  }

  viewBusinessDetails(business: BusinessApprovalData): void {
    this.selectedBusiness = business;
    this.showDetailModal = true;
  }

  closeDetailModal(): void {
    this.showDetailModal = false;
    this.selectedBusiness = null;
  }

  approveBusiness(business: BusinessApprovalData): void {
    const currentUserId = 'current-admin-user'; // TODO: Get from auth service
    
    this.businessService.approveBusinessApproval(business.id, currentUserId, 'Approved by admin').subscribe(res => {
      if (res.isSuccess) {
        business.status = 'approved';
        business.approvedBy = currentUserId;
        business.approvalDate = new Date().toISOString();
        this.loadStatistics(); // Refresh statistics
        this.splashScreenService.showQuickFeedback('success', 'Phê duyệt thành công!', `Doanh nghiệp ${business.businessName} đã được phê duyệt`);
        setTimeout(() => this.closeDetailModal(), 1200);
      } else {
        this.splashScreenService.showQuickFeedback('error', 'Lỗi phê duyệt!', res.message);
      }
    }, error => {
      console.error('Error approving business:', error);
      this.splashScreenService.showQuickFeedback('error', 'Lỗi phê duyệt!', 'Không thể kết nối đến server');
    });
  }

  rejectBusiness(business: BusinessApprovalData): void {
    const currentUserId = 'current-admin-user'; // TODO: Get from auth service
    const reason = 'Không đủ điều kiện phê duyệt';
    
    this.businessService.rejectBusinessApproval(business.id, currentUserId, reason).subscribe(res => {
      if (res.isSuccess) {
        business.status = 'rejected';
        business.approvedBy = currentUserId;
        business.approvalDate = new Date().toISOString();
        business.notes = reason;
        this.loadStatistics(); // Refresh statistics
        this.splashScreenService.showQuickFeedback('error', 'Đã từ chối!', `Doanh nghiệp ${business.businessName} đã bị từ chối`);
        setTimeout(() => this.closeDetailModal(), 1200);
      } else {
        this.splashScreenService.showQuickFeedback('error', 'Lỗi từ chối!', res.message);
      }
    }, error => {
      console.error('Error rejecting business:', error);
      this.splashScreenService.showQuickFeedback('error', 'Lỗi từ chối!', 'Không thể kết nối đến server');
    });
  }

  reviewBusiness(business: BusinessApprovalData): void {
    const currentUserId = 'current-admin-user'; // TODO: Get from auth service
    
    this.businessService.setReviewingBusinessApproval(business.id, currentUserId, 'Đang xem xét hồ sơ').subscribe(res => {
      if (res.isSuccess) {
        business.status = 'reviewing';
        business.approvedBy = currentUserId;
        this.loadStatistics(); // Refresh statistics
        this.splashScreenService.showBriefLoading('Đang xem xét...', `Doanh nghiệp ${business.businessName} đang được xem xét`, 600, 'Đã chuyển trạng thái!', 'Doanh nghiệp đang được xem xét');
        setTimeout(() => this.closeDetailModal(), 1600);
      } else {
        this.splashScreenService.showQuickFeedback('error', 'Lỗi cập nhật!', res.message);
      }
    }, error => {
      console.error('Error setting business to reviewing:', error);
      this.splashScreenService.showQuickFeedback('error', 'Lỗi cập nhật!', 'Không thể kết nối đến server');
    });
  }

  viewDocument(document: DocumentInfo): void {
    console.log('Viewing document:', document.name);
    // Open document in new tab or modal
    if (document.url) {
      window.open(document.url, '_blank');
    } else {
      // If no direct URL, use download endpoint to view
      this.downloadDocument(document);
    }
  }

  downloadDocument(document: DocumentInfo): void {
    console.log('Downloading document:', document.name);
    
    this.businessService.downloadBusinessDocument(document.id).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const link = window.document.createElement('a');
      link.href = url;
      link.download = document.name;
      link.click();
      window.URL.revokeObjectURL(url);
    }, error => {
      console.error('Error downloading document:', error);
      this.splashScreenService.showQuickFeedback('error', 'Lỗi tải xuống!', 'Không thể tải xuống tài liệu');
    });
  }

  filterBusinesses(): void {
    // Instead of client-side filtering, reload data with new filters
    this.currentPage = 1; // Reset to first page when filtering
    this.loadBusinessData();
  }

  updateStats(): void {
    // Reload statistics from server instead of client-side calculation
    this.loadStatistics();
  }

  getStatusText(status: string): string {
    switch(status) {
      case 'pending': return 'Chờ duyệt';
      case 'approved': return 'Đã duyệt';
      case 'rejected': return 'Từ chối';
      case 'reviewing': return 'Đang xem xét';
      default: return status;
    }
  }

  getStatusClass(status: string): string {
    switch(status) {
      case 'pending': return 'status-pending';
      case 'approved': return 'status-approved';
      case 'rejected': return 'status-rejected';
      case 'reviewing': return 'status-reviewing';
      default: return 'status-secondary';
    }
  }

  getPaginatedBusinesses(): BusinessApprovalData[] {
    // Since we're using server-side pagination, just return filtered businesses
    return this.filteredBusinesses;
  }

  getTotalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.getTotalPages()) {
      this.currentPage = page;
      this.loadBusinessData(); // Reload data with new page
    }
  }

  goToPreviousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadBusinessData(); // Reload data with new page
    }
  }

  goToNextPage(): void {
    if (this.currentPage < this.getTotalPages()) {
      this.currentPage++;
      this.loadBusinessData(); // Reload data with new page
    }
  }

  getTodaySubmissions(): number {
    // Use the value from statistics API instead of client-side calculation
    return this.approvalStats.todaySubmissions || 0;
  }

  clearSearch(): void {
    this.searchKeyword = '';
    this.filterBusinesses();
  }

  getVisiblePages(): number[] {
    const totalPages = this.getTotalPages();
    const currentPage = this.currentPage;
    const visiblePages: number[] = [];
    const maxVisible = 5;
    let startPage = Math.max(1, currentPage - Math.floor(maxVisible / 2));
    let endPage = Math.min(totalPages, startPage + maxVisible - 1);
    if (endPage - startPage + 1 < maxVisible) {
      startPage = Math.max(1, endPage - maxVisible + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      visiblePages.push(i);
    }
    
    return visiblePages;
  }
}