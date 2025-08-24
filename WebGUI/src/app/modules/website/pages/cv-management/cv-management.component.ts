import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CandidateService } from '../../../../services/website/candidate.service';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { CVModel } from '../../../../models/cv.model';

@Component({
  selector: 'app-cv-management',
  templateUrl: './cv-management.component.html',
  styleUrls: ['./cv-management.component.scss']
})
export class CvManagementComponent implements OnInit {

  cvs: CVModel[] = [];
  viewMode: 'grid' | 'list' = 'grid';
  isLoading = false;
  currentCandidateId = 1; // TODO: Get from auth service

  constructor(
    private candidateService: CandidateService,
    private splashScreenService: SplashScreenService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCVs();
  }

  loadCVs(): void {
    this.isLoading = true;
    // Using dummy data for development
    this.candidateService.getCVsDummy(this.currentCandidateId).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.cvs = response.result;
        } else {
          this.splashScreenService.showQuickFeedback('error', 'Lỗi tải dữ liệu!', response.message);
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading CVs:', error);
        this.splashScreenService.showQuickFeedback('error', 'Lỗi tải dữ liệu!', 'Không thể tải danh sách CV');
        this.isLoading = false;
      }
    });
  }

  // View mode methods
  setViewMode(mode: 'grid' | 'list'): void {
    this.viewMode = mode;
  }

  // Statistics methods
  getActiveCVCount(): number {
    return this.cvs.filter(cv => cv.isActive).length;
  }

  getTotalViews(): number {
    return this.cvs.reduce((total, cv) => total + cv.viewCount, 0);
  }

  getTotalDownloads(): number {
    return this.cvs.reduce((total, cv) => total + cv.downloadCount, 0);
  }

  // Template methods
  getTemplateName(template: string): string {
    switch (template) {
      case 'modern': return 'Hiện đại';
      case 'classic': return 'Cổ điển';
      case 'creative': return 'Sáng tạo';
      case 'professional': return 'Chuyên nghiệp';
      default: return template;
    }
  }

  // CV Actions
  createNewCV(): void {
    console.log('Creating new CV - navigating to cv-builder');
    this.router.navigate(['/website/cv-builder']);
  }

  viewCV(cv: CVModel): void {
    // TODO: Navigate to CV preview page
    console.log('Viewing CV:', cv.title);
    this.splashScreenService.showQuickFeedback('success', 'Xem CV', `Đang mở CV: ${cv.title}`);
  }

  editCV(cv: CVModel): void {
    // Store CV data for editing
    this.candidateService.setCurrentEditingCV(cv);
    this.router.navigate(['/website/cv-builder', cv.id]);
  }

  downloadCV(cv: CVModel): void {
    this.splashScreenService.showBriefLoading(
      'Đang tải xuống...',
      `Đang tạo file CV: ${cv.title}`,
      1000,
      'Tải xuống thành công!',
      'CV đã được tải xuống'
    );
    
    // TODO: Implement actual download
    setTimeout(() => {
      // Simulate download
      cv.downloadCount++;
    }, 1000);
  }

  duplicateCV(cv: CVModel): void {
    const duplicatedCV: CVModel = {
      ...cv,
      id: Math.max(...this.cvs.map(c => c.id)) + 1,
      title: `${cv.title} - Copy`,
      isActive: false,
      createdDate: new Date().toISOString().split('T')[0],
      updatedDate: new Date().toISOString().split('T')[0],
      viewCount: 0,
      downloadCount: 0
    };

    this.cvs.push(duplicatedCV);
    this.splashScreenService.showQuickFeedback('success', 'Sao chép thành công!', `CV "${cv.title}" đã được sao chép`);
  }

  setActiveCV(cv: CVModel): void {
    // Deactivate all CVs first
    this.cvs.forEach(c => c.isActive = false);
    
    // Set selected CV as active
    cv.isActive = true;
    
    this.splashScreenService.showQuickFeedback(
      'success',
      'Đặt CV chính thành công!',
      `CV "${cv.title}" đã được đặt làm CV chính`
    );

    // TODO: Call API to update active CV
    // this.candidateService.setActiveCV(cv.id).subscribe(...);
  }

  deleteCV(cv: CVModel): void {
    if (cv.isActive) {
      this.splashScreenService.showQuickFeedback(
        'error',
        'Không thể xóa!',
        'Không thể xóa CV đang được sử dụng. Vui lòng đặt CV khác làm CV chính trước.'
      );
      return;
    }

    // Show confirmation
    if (confirm(`Bạn có chắc chắn muốn xóa CV "${cv.title}"?`)) {
      const index = this.cvs.findIndex(c => c.id === cv.id);
      if (index > -1) {
        this.cvs.splice(index, 1);
        this.splashScreenService.showQuickFeedback('success', 'Xóa thành công!', `CV "${cv.title}" đã được xóa`);
        
        // TODO: Call API to delete CV
        // this.candidateService.deleteCV(cv.id).subscribe(...);
      }
    }
  }
}
