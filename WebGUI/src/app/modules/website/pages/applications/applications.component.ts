import { Component, OnInit } from '@angular/core';
import { SplashScreenService } from '../../../../services/splash-screen.service';

@Component({
  selector: 'app-applications',
  templateUrl: './applications.component.html',
  styleUrls: ['./applications.component.scss']
})
export class ApplicationsComponent implements OnInit {

  // Dummy data
  applications = [
    {
      id: 1,
      jobTitle: 'Frontend Developer - React/Angular',
      company: 'Công ty TNHH Công nghệ ABC',
      logo: 'assets/vieclamlaocai/img/image 16.png',
      salary: '15 - 25 triệu',
      status: 'pending',
      appliedDate: '2024-01-15',
      location: 'Lào Cai'
    },
    {
      id: 2,
      jobTitle: 'Kế toán tổng hợp',
      company: 'Công ty Cổ phần Đầu tư XYZ',
      logo: 'assets/vieclamlaocai/img/image 23.png',
      salary: '12 - 18 triệu',
      status: 'approved',
      appliedDate: '2024-01-10',
      location: 'Lào Cai'
    },
    {
      id: 3,
      jobTitle: 'Nhân viên kinh doanh',
      company: 'Công ty TNHH Thương mại DEF',
      logo: 'assets/vieclamlaocai/img/image 16.png',
      salary: '10 - 20 triệu',
      status: 'rejected',
      appliedDate: '2024-01-08',
      location: 'Lào Cai'
    }
  ];

  savedJobs = [
    {
      id: 1,
      jobTitle: 'Full Stack Developer',
      company: 'Công ty Cổ phần Phần mềm GHI',
      logo: 'assets/vieclamlaocai/img/image 23.png',
      salary: '20 - 35 triệu',
      savedDate: '2024-01-12',
      location: 'Lào Cai',
      urgent: true
    },
    {
      id: 2,
      jobTitle: 'Digital Marketing Specialist',
      company: 'Công ty TNHH Marketing JKL',
      logo: 'assets/vieclamlaocai/img/image 16.png',
      salary: '12 - 22 triệu',
      savedDate: '2024-01-14',
      location: 'Lào Cai',
      urgent: false
    }
  ];

  statistics = {
    postedJobs: 3,
    applications: 55,
    savedProfiles: 2,
    profileViews: 245,
    refreshCount: 14,
    jobViews: 666
  };

  constructor(private splashScreenService: SplashScreenService) {}

  ngOnInit(): void {
    console.log('Applications component loaded');
    this.loadApplicationsData();
  }

  loadApplicationsData(): void {
    console.log('Loading applications data...');
    // Simplified loading - data loads instantly for better UX
    // In real app, only show splash for actual API calls that take time
  }

  getStatusText(status: string): string {
    switch(status) {
      case 'pending': return 'Đang chờ';
      case 'approved': return 'Được duyệt';
      case 'accepted': return 'Được chấp nhận';
      case 'rejected': return 'Từ chối';
      case 'reviewing': return 'Đang xem xét';
      default: return status;
    }
  }

  getStatusClass(status: string): string {
    switch(status) {
      case 'pending': return 'pending';
      case 'approved': return 'accepted';
      case 'rejected': return 'rejected';
      case 'reviewing': return 'reviewing';
      default: return 'pending';
    }
  }
}