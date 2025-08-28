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
    this.businessService.getAllDummy().subscribe(res => {
      if (res.isSuccess) {
        this.businesses = res.result;
        this.filteredBusinesses = [...this.businesses];
        this.totalItems = this.businesses.length;
      }
    });
  }

  loadStatistics(): void {
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
    // For now, simulate the API call since we're using dummy data
    // In production, uncomment the real API call below
    
    // Real API call (for production):
    // const currentUserId = 'current-user-id'; // Get from auth service
    // this.businessService.approve(business.id, currentUserId, 'Approved by admin').subscribe(res => {
    //   if (res.isSuccess) {
    //     business.status = 'approved';
    //     this.updateStats();
    //     this.splashScreenService.showQuickFeedback('success', 'Phê duyệt thành công!', `Doanh nghiệp ${business.businessName} đã được phê duyệt`);
    //     setTimeout(() => this.closeDetailModal(), 1200);
    //   } else {
    //     this.splashScreenService.showQuickFeedback('error', 'Lỗi phê duyệt!', res.message);
    //   }
    // });

    // Dummy implementation (for development):
    business.status = 'approved';
    this.splashScreenService.showQuickFeedback(
      'success',
      'Phê duyệt thành công!',
      `Doanh nghiệp ${business.businessName} đã được phê duyệt`
    );
    setTimeout(() => {
      this.closeDetailModal();
    }, 1200);
    this.updateStats();
  }

  rejectBusiness(business: BusinessApprovalData): void {
    // const currentUserId = 'current-user-id';
    // const reason = 'Không đủ điều kiện';
    // this.businessService.reject(business.id, currentUserId, reason).subscribe(res => {
    //   if (res.isSuccess) {
    //     business.status = 'rejected';
    //     this.updateStats();
    //     this.splashScreenService.showQuickFeedback('error', 'Đã từ chối!', `Doanh nghiệp ${business.businessName} đã bị từ chối`);
    //     setTimeout(() => this.closeDetailModal(), 1200);
    //   } else {
    //     this.splashScreenService.showQuickFeedback('error', 'Lỗi từ chối!', res.message);
    //   }
    // });

    // Dummy implementation (for development):
    business.status = 'rejected';
    this.splashScreenService.showQuickFeedback(
      'error',
      'Đã từ chối!',
      `Doanh nghiệp ${business.businessName} đã bị từ chối`
    );
    setTimeout(() => {
      this.closeDetailModal();
    }, 1200);
    this.updateStats();
  }

  reviewBusiness(business: BusinessApprovalData): void {
    // const currentUserId = 'current-user-id'; // Get from auth service
    // this.businessService.setReviewing(business.id, currentUserId).subscribe(res => {
    //   if (res.isSuccess) {
    //     business.status = 'reviewing';
    //     this.updateStats();
    //     this.splashScreenService.showBriefLoading('Đang xem xét...', `Doanh nghiệp ${business.businessName} đang được xem xét`, 600, 'Đã chuyển trạng thái!', 'Doanh nghiệp đang được xem xét');
    //     setTimeout(() => this.closeDetailModal(), 1600);
    //   } else {
    //     this.splashScreenService.showQuickFeedback('error', 'Lỗi cập nhật!', res.message);
    //   }
    // });

    // Dummy implementation (for development):
    business.status = 'reviewing';
    this.splashScreenService.showBriefLoading(
      'Đang xem xét...',
      `Doanh nghiệp ${business.businessName} đang được xem xét`,
      600,
      'Đã chuyển trạng thái!',
      'Doanh nghiệp đang được xem xét'
    );
    setTimeout(() => {
      this.closeDetailModal();
    }, 1600);
    this.updateStats();
  }

  viewDocument(document: DocumentInfo): void {
    console.log('Viewing document:', document.name);
  }

  downloadDocument(document: DocumentInfo): void {
    console.log('Downloading document:', document.name);
   
  }

  filterBusinesses(): void {
    this.filteredBusinesses = this.businesses.filter(business => {
      const statusMatch = this.statusFilter === 'all' || business.status === this.statusFilter;
      const typeMatch = this.businessTypeFilter === 'all' || business.businessType === this.businessTypeFilter;
      const keywordMatch = !this.searchKeyword || 
        business.businessName.toLowerCase().includes(this.searchKeyword.toLowerCase()) ||
        business.contactPerson.toLowerCase().includes(this.searchKeyword.toLowerCase()) ||
        business.taxCode.includes(this.searchKeyword);
      
      return statusMatch && typeMatch && keywordMatch;
    });
    
    this.totalItems = this.filteredBusinesses.length;
    this.currentPage = 1;
  }

  updateStats(): void {
    this.approvalStats = {
      pending: this.businesses.filter(b => b.status === 'pending').length,
      approved: this.businesses.filter(b => b.status === 'approved').length,
      rejected: this.businesses.filter(b => b.status === 'rejected').length,
      reviewing: this.businesses.filter(b => b.status === 'reviewing').length
    };
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
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.filteredBusinesses.slice(startIndex, endIndex);
  }

  getTotalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.getTotalPages()) {
      this.currentPage = page;
    }
  }

  goToPreviousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  goToNextPage(): void {
    if (this.currentPage < this.getTotalPages()) {
      this.currentPage++;
    }
  }

  getTodaySubmissions(): number {
    const today = new Date().toDateString();
    return this.businesses.filter(business => 
      new Date(business.registrationDate).toDateString() === today
    ).length;
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