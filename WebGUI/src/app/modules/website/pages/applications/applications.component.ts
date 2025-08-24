import { Component, OnInit, HostListener } from '@angular/core';
import { SplashScreenService } from '../../../../services/splash-screen.service';

interface InterviewCandidate {
  id: number;
  fullName: string;
  position: string;
  avatar: string;
  email: string;
  phone: string;
  stage: string;
  source: string;
  interviewDate: string;
  interviewTime: string;
}

interface CandidateFilters {
  fullName: string;
  contactInfo: string;
  stage: string;
  source: string;
}

@Component({
  selector: 'app-applications',
  templateUrl: './applications.component.html',
  styleUrls: ['./applications.component.scss']
})
export class ApplicationsComponent implements OnInit {

  // Interview candidates data
  interviewCandidates: InterviewCandidate[] = [
    {
      id: 1,
      fullName: 'Nguyễn Văn A',
      position: 'Frontend Developer',
      avatar: 'assets/vieclamlaocai/img/image 16.png',
      email: 'nguyenvana@email.com',
      phone: '0123456789',
      stage: 'pending',
      source: 'website',
      interviewDate: '22/08/2025',
      interviewTime: '09:00'
    },
    {
      id: 2,
      fullName: 'Trần Thị B',
      position: 'Kế toán tổng hợp',
      avatar: 'assets/vieclamlaocai/img/image 23.png',
      email: 'tranthib@email.com',
      phone: '0987654321',
      stage: 'interviewed',
      source: 'referral',
      interviewDate: '22/08/2025',
      interviewTime: '14:00'
    },
    {
      id: 3,
      fullName: 'Lê Văn C',
      position: 'Marketing Executive',
      avatar: 'assets/vieclamlaocai/img/image 16.png',
      email: 'levanc@email.com',
      phone: '0369852147',
      stage: 'passed',
      source: 'jobboard',
      interviewDate: '23/08/2025',
      interviewTime: '10:00'
    },
    {
      id: 4,
      fullName: 'Phạm Thị D',
      position: 'Sales Representative',
      avatar: 'assets/vieclamlaocai/img/image 23.png',
      email: 'phamthid@email.com',
      phone: '0521478963',
      stage: 'failed',
      source: 'social',
      interviewDate: '21/08/2025',
      interviewTime: '15:00'
    }
  ];

  // Filters
  filters: CandidateFilters = {
    fullName: '',
    contactInfo: '',
    stage: '',
    source: ''
  };

  // Filtered candidates
  filteredCandidates: InterviewCandidate[] = [];

  // Current date
  currentDate = new Date().toLocaleDateString('vi-VN');

  // Action menu state
  activeMenuId: number | null = null;

  // Selected candidate for modals
  selectedCandidate: InterviewCandidate | null = null;

  // Data objects for forms
  interviewData: any = {
    date: '',
    time: '',
    location: '',
    notes: ''
  };

  editData: any = {
    fullName: '',
    position: '',
    email: '',
    phone: '',
    stage: 'pending',
    source: 'website'
  };

  emailData: any = {
    subject: '',
    content: '',
    template: ''
  };

  constructor(
    private splashScreenService: SplashScreenService
  ) {
  }

  ngOnInit(): void {
    console.log('Applications component loaded - fresh data loading...');
    this.showLoadingSplash();
    this.loadApplicationsData();
    this.applyFilters(); // Initialize filtered candidates
  }

  showLoadingSplash(): void {
    this.splashScreenService.show({
      type: 'loading',
      title: 'Đang tải ứng viên phỏng vấn...',
      message: 'Vui lòng chờ trong giây lát',
      showProgress: true
    });
  }

  loadApplicationsData(): void {
    console.log('Loading fresh interview candidates data...');

    setTimeout(() => {
      console.log('Interview candidates data loaded successfully');

      this.splashScreenService.show({
        type: 'success',
        title: 'Tải thành công!',
        message: 'Danh sách ứng viên phỏng vấn đã được cập nhật'
      });

      setTimeout(() => {
        this.splashScreenService.hide();
      }, 1500);
      
    }, 2000);
  }

  applyFilters(): void {
    this.filteredCandidates = this.interviewCandidates.filter(candidate => {
      const nameMatch = !this.filters.fullName || 
        candidate.fullName.toLowerCase().includes(this.filters.fullName.toLowerCase());
      
      const contactMatch = !this.filters.contactInfo || 
        candidate.email.toLowerCase().includes(this.filters.contactInfo.toLowerCase()) ||
        candidate.phone.includes(this.filters.contactInfo);
      
      const stageMatch = !this.filters.stage || candidate.stage === this.filters.stage;
      const sourceMatch = !this.filters.source || candidate.source === this.filters.source;
      
      return nameMatch && contactMatch && stageMatch && sourceMatch;
    });
  }

  searchCandidates(): void {
    this.applyFilters();
  }

  clearFilters(): void {
    this.filters = {
      fullName: '',
      contactInfo: '',
      stage: '',
      source: ''
    };
    this.applyFilters();
  }

  getStageText(stage: string): string {
    switch(stage) {
      case 'pending': return 'Chờ phỏng vấn';
      case 'interviewed': return 'Đã phỏng vấn';
      case 'passed': return 'Đạt';
      case 'failed': return 'Không đạt';
      default: return stage;
    }
  }

  getStageClass(stage: string): string {
    switch(stage) {
      case 'pending': return 'warning';
      case 'interviewed': return 'info';
      case 'passed': return 'success';
      case 'failed': return 'danger';
      default: return 'secondary';
    }
  }

  getSourceText(source: string): string {
    switch(source) {
      case 'website': return 'Website';
      case 'referral': return 'Giới thiệu';
      case 'jobboard': return 'Job board';
      case 'social': return 'Mạng xã hội';
      default: return source;
    }
  }

  viewCandidate(candidate: InterviewCandidate): void {
    console.log('Viewing candidate:', candidate.fullName);
    this.selectedCandidate = candidate;
    this.closeActionMenu();
    this.showModal('viewCandidateModal');
  }

  scheduleInterview(candidate: InterviewCandidate): void {
    console.log('Scheduling interview for:', candidate.fullName);
    this.selectedCandidate = candidate;
    this.closeActionMenu();
    this.showModal('scheduleInterviewModal');
  }

  editCandidate(candidate: InterviewCandidate): void {
    console.log('Editing candidate:', candidate.fullName);
    this.selectedCandidate = candidate;
    this.populateEditForm(candidate);
    this.closeActionMenu();
    this.showModal('editCandidateModal');
  }

  sendEmail(candidate: InterviewCandidate): void {
    console.log('Sending email to:', candidate.fullName);
    this.selectedCandidate = candidate;
    this.closeActionMenu();
    this.showModal('sendEmailModal');
  }

  deleteCandidate(candidate: InterviewCandidate): void {
    console.log('Deleting candidate:', candidate.fullName);
    this.selectedCandidate = candidate;
    this.closeActionMenu();
    this.showModal('deleteCandidateModal');
  }

  populateEditForm(candidate: InterviewCandidate): void {
    this.editData = {
      fullName: candidate.fullName,
      position: candidate.position,
      email: candidate.email,
      phone: candidate.phone,
      stage: candidate.stage,
      source: candidate.source
    };
  }

  saveInterviewSchedule(): void {
    if (this.interviewData.date && this.interviewData.time) {
      console.log('Saving interview schedule:', this.interviewData);
      // TODO: Implement API call
      this.hideModal('scheduleInterviewModal');
      this.showSuccessMessage('Lịch phỏng vấn đã được lưu thành công!');
    }
  }

  saveCandidateEdit(): void {
    if (this.editData.fullName && this.editData.position && this.editData.email && this.editData.phone) {
      console.log('Saving candidate edit:', this.editData);
      // TODO: Implement API call
      this.hideModal('editCandidateModal');
      this.showSuccessMessage('Thông tin ứng viên đã được cập nhật!');
    }
  }

  loadEmailTemplate(): void {
    const template = this.emailData.template;
    if (template) {
      const templates = {
        interview_invitation: {
          subject: 'Mời phỏng vấn',
          content: 'Kính gửi ứng viên,\n\nChúng tôi xin mời bạn tham gia buổi phỏng vấn...'
        },
        interview_confirmation: {
          subject: 'Xác nhận lịch phỏng vấn',
          content: 'Kính gửi ứng viên,\n\nChúng tôi xác nhận lịch phỏng vấn của bạn...'
        },
        rejection: {
          subject: 'Thông báo kết quả phỏng vấn',
          content: 'Kính gửi ứng viên,\n\nSau khi xem xét kỹ lưỡng...'
        },
        offer: {
          subject: 'Thư mời nhận việc',
          content: 'Kính gửi ứng viên,\n\nChúng tôi rất vui mừng thông báo...'
        }
      };
      
      const selectedTemplate = templates[template as keyof typeof templates];
      if (selectedTemplate) {
        this.emailData.subject = selectedTemplate.subject;
        this.emailData.content = selectedTemplate.content;
      }
    }
  }

  previewEmail(): void {
    if (this.emailData.subject && this.emailData.content) {
      console.log('Email preview:', this.emailData);
      // TODO: Show email preview modal
    }
  }

  // Modal management
  showModal(modalId: string): void {
    // TODO: Implement Bootstrap modal show
    console.log('Showing modal:', modalId);
  }

  hideModal(modalId: string): void {
    // TODO: Implement Bootstrap modal hide
    console.log('Hiding modal:', modalId);
  }

  showSuccessMessage(message: string): void {
    this.splashScreenService.show({
      type: 'success',
      title: 'Thành công!',
      message: message
    });

    setTimeout(() => {
      this.splashScreenService.hide();
    }, 2000);
  }

  toggleActionMenu(candidateId: number): void {
    if (this.activeMenuId === candidateId) {
      this.activeMenuId = null;
    } else {
      this.activeMenuId = candidateId;
    }
  }

  closeActionMenu(): void {
    this.activeMenuId = null;
  }

  // Handle clicks outside dropdown
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.action-menu')) {
      this.closeActionMenu();
    }
  }

  sendEmailData(): void {
    if (this.emailData.subject && this.emailData.content) {
      console.log('Sending email:', this.emailData);
      // TODO: Implement API call
      this.hideModal('sendEmailModal');
      this.showSuccessMessage('Email đã được gửi thành công!');
    }
  }

  confirmDeleteCandidate(): void {
    if (this.selectedCandidate) {
      console.log('Confirming deletion of:', this.selectedCandidate.fullName);
      // TODO: Implement API call
      this.hideModal('deleteCandidateModal');
      this.showSuccessMessage('Ứng viên đã được xóa thành công!');
    }
  }
}