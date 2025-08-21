import { Component, OnInit } from '@angular/core';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { CandidateService, CandidateModel } from '../../../../services/website/candidate.service';

@Component({
  selector: 'app-find-candidates',
  templateUrl: './find-candidates.component.html',
  styleUrls: ['./find-candidates.component.scss']
})
export class FindCandidatesComponent implements OnInit {

  candidates: CandidateModel[] = [];

  searchFilters = {
    keyword: '',
    experience: '',
    skills: '',
    location: 'Lào Cai',
    salary: ''
  };

  filteredCandidates = [...this.candidates];

  constructor(
    private splashScreenService: SplashScreenService,
    private candidateService: CandidateService
  ) {}

  ngOnInit(): void {
    console.log('Find Candidates component loaded');
    this.loadCandidatesData();
  }

  loadCandidatesData(): void {
    console.log('Loading candidates data...');
    this.candidateService.getAllDummy().subscribe(res => {
      if (res.isSuccess) {
        this.candidates = res.result;
        this.filteredCandidates = [...this.candidates];
      }
    });
  }

  searchCandidates(): void {
    this.splashScreenService.showBriefLoading(
      'Đang tìm kiếm...',
      'Vui lòng chờ trong giây lát',
      600,
      'Tìm kiếm hoàn tất!',
      `Tìm thấy ${this.getFilteredCandidatesCount()} ứng viên phù hợp`
    );
    
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
    }, 600);
  }

  getFilteredCandidatesCount(): number {
    return this.candidates.filter(candidate => {
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
    }).length;
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
