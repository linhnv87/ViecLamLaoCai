import { Component, OnInit } from '@angular/core';
import { SplashScreenService } from '../../../../services/splash-screen.service';

@Component({
  selector: 'app-find-candidates',
  templateUrl: './find-candidates.component.html',
  styleUrls: ['./find-candidates.component.scss']
})
export class FindCandidatesComponent implements OnInit {

  // Dummy data
  candidates = [
    {
      id: 1,
      name: 'Nguyễn Minh Khôi',
      position: 'Frontend Developer',
      experience: '3 năm kinh nghiệm',
      education: 'Đại học Bách Khoa Hà Nội',
      skills: ['ReactJS', 'Angular', 'Vue.js', 'TypeScript'],
      location: 'Lào Cai',
      salary: '15-25 triệu',
      avatar: 'assets/vieclamlaocai/img/image 16.png',
      status: 'available',
      lastActive: '2024-01-15'
    },
    {
      id: 2,
      name: 'Trần Thùy Linh',
      position: 'UI/UX Designer',
      experience: '2 năm kinh nghiệm',
      education: 'Đại học Mỹ thuật Việt Nam',
      skills: ['Figma', 'Adobe XD', 'Sketch', 'Photoshop'],
      location: 'Lào Cai',
      salary: '12-20 triệu',
      avatar: 'assets/vieclamlaocai/img/image 23.png',
      status: 'open-to-work',
      lastActive: '2024-01-14'
    },
    {
      id: 3,
      name: 'Phạm Văn Hùng',
      position: 'Backend Developer',
      experience: '5 năm kinh nghiệm',
      education: 'Đại học Công nghệ - ĐHQG HN',
      skills: ['Node.js', 'Python', 'Java', 'MongoDB', 'PostgreSQL'],
      location: 'Lào Cai',
      salary: '20-35 triệu',
      avatar: 'assets/vieclamlaocai/img/image 16.png',
      status: 'busy',
      lastActive: '2024-01-13'
    },
    {
      id: 4,
      name: 'Lê Hương Giang',
      position: 'Digital Marketing Specialist',
      experience: '4 năm kinh nghiệm',
      education: 'Đại học Ngoại thương',
      skills: ['Google Ads', 'Facebook Ads', 'SEO', 'Content Marketing'],
      location: 'Lào Cai',
      salary: '13-22 triệu',
      avatar: 'assets/vieclamlaocai/img/image 23.png',
      status: 'available',
      lastActive: '2024-01-15'
    }
  ];

  searchFilters = {
    keyword: '',
    experience: '',
    skills: '',
    location: 'Lào Cai',
    salary: ''
  };

  filteredCandidates = [...this.candidates];

  constructor(private splashScreenService: SplashScreenService) {}

  ngOnInit(): void {
    console.log('Find Candidates component loaded - fresh data loading...');
    this.showLoadingSplash();
    this.loadCandidatesData();
  }

  showLoadingSplash(): void {
    this.splashScreenService.show({
      type: 'loading',
      title: 'Đang tìm ứng viên...',
      message: 'Đang tải danh sách ứng viên phù hợp',
      showProgress: true
    });
  }

  loadCandidatesData(): void {
    console.log('Loading fresh candidates data...');
    
    // Simulate API loading time
    setTimeout(() => {
      console.log('Candidates data loaded successfully');
      this.filteredCandidates = [...this.candidates];
      
      // Show success splash
      this.splashScreenService.show({
        type: 'success',
        title: 'Tìm thấy ứng viên!',
        message: `Đã tìm thấy ${this.candidates.length} ứng viên phù hợp`
      });

      // Hide splash after showing success
      setTimeout(() => {
        this.splashScreenService.hide();
      }, 1500);
      
    }, 2000);
  }

  searchCandidates(): void {
    this.showLoadingSplash();
    
    setTimeout(() => {
      this.filteredCandidates = this.candidates.filter(candidate => {
        const keywordMatch = !this.searchFilters.keyword || 
          candidate.name.toLowerCase().includes(this.searchFilters.keyword.toLowerCase()) ||
          candidate.position.toLowerCase().includes(this.searchFilters.keyword.toLowerCase());
        
        const experienceMatch = !this.searchFilters.experience ||
          candidate.experience.includes(this.searchFilters.experience);
        
        const skillsMatch = !this.searchFilters.skills ||
          candidate.skills.some(skill => 
            skill.toLowerCase().includes(this.searchFilters.skills.toLowerCase())
          );

        return keywordMatch && experienceMatch && skillsMatch;
      });

      this.splashScreenService.show({
        type: 'success',
        title: 'Tìm kiếm hoàn tất!',
        message: `Tìm thấy ${this.filteredCandidates.length} ứng viên phù hợp`
      });

      setTimeout(() => {
        this.splashScreenService.hide();
      }, 1500);
    }, 1500);
  }

  getStatusText(status: string): string {
    switch(status) {
      case 'available': return 'Sẵn sàng';
      case 'open-to-work': return 'Tìm việc';
      case 'busy': return 'Bận';
      default: return status;
    }
  }

  getStatusClass(status: string): string {
    switch(status) {
      case 'available': return 'success';
      case 'open-to-work': return 'warning';
      case 'busy': return 'danger';
      default: return 'secondary';
    }
  }
}
