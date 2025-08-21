import { Component, OnInit } from '@angular/core';
import { SplashScreenService } from '../../../../services/splash-screen.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  // Dummy data
  dashboardStats = {
    totalJobs: 1247,
    newApplications: 89,
    activeJobs: 156,
    totalViews: 12450,
    todayApplications: 23,
    todayViews: 567
  };

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

  constructor(private splashScreenService: SplashScreenService) {}

  ngOnInit(): void {
    console.log('Dashboard component loaded');
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    console.log('Loading dashboard data...');
    // Simplified loading - data loads instantly for better UX
    // In real app, only show splash for actual API calls that take time
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
}