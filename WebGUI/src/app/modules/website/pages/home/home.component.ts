import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { HomeService, FeaturedJob, SuggestedJob, JobCategory, HomePageStats } from '../../../../services/website/home.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  featuredJobs: FeaturedJob[] = [];
  suggestedJobs: SuggestedJob[] = [];
  jobCategories: JobCategory[] = [];
  homeStats: HomePageStats = {
    totalJobs: 0,
    totalCompanies: 0,
    totalCandidates: 0,
    newJobsThisWeek: 0,
    featuredJobsCount: 0,
    urgentJobsCount: 0
  };

  searchQuery = {
    keyword: '',
    location: 'Lào Cai',
    category: '',
    salaryRange: '',
    experience: ''
  };

  constructor(
    private router: Router,
    private splashScreenService: SplashScreenService,
    private homeService: HomeService
  ) { }

  ngOnInit(): void {
    console.log('Home component initialized');
    this.loadHomePageData();
  }

  loadHomePageData(): void {
    this.splashScreenService.show({
      type: 'loading',
      title: 'Chào mừng đến Việc Làm Lào Cai!',
      message: 'Đang tải việc làm mới nhất cho bạn...',
      showProgress: true
    });

    // Load all home page data in parallel
    this.loadFeaturedJobs();
    this.loadSuggestedJobs();
    this.loadJobCategories();
    this.loadHomeStats();

    // Hide splash screen quickly since we have loading states in template
    setTimeout(() => {
      this.splashScreenService.hide();
    }, 800);
  }

  loadFeaturedJobs(): void {
    // Load data immediately (synchronously for demo)
    setTimeout(() => {
      this.homeService.getFeaturedJobsDummy(6).subscribe(res => {
        if (res.isSuccess) {
          this.featuredJobs = res.result;
          console.log('Featured jobs loaded:', this.featuredJobs.length);
        }
      });
    }, 100);
  }

  loadSuggestedJobs(): void {
    // Load data immediately (synchronously for demo)
    setTimeout(() => {
      this.homeService.getSuggestedJobsDummy(undefined, 6).subscribe(res => {
        if (res.isSuccess) {
          this.suggestedJobs = res.result;
          console.log('Suggested jobs loaded:', this.suggestedJobs.length);
        }
      });
    }, 200);
  }

  loadJobCategories(): void {
    // Load data immediately (synchronously for demo)
    setTimeout(() => {
      this.homeService.getJobCategoriesDummy().subscribe(res => {
        if (res.isSuccess) {
          this.jobCategories = res.result;
          console.log('Job categories loaded:', this.jobCategories.length);
        }
      });
    }, 300);
  }

  loadHomeStats(): void {
    this.homeService.getHomeStatsDummy().subscribe(res => {
      if (res.isSuccess) {
        this.homeStats = res.result;
        console.log('Home stats loaded:', this.homeStats);
      }
    });
  }

  searchJobs(searchQuery: any = {}): void {
    // Prepare search parameters
    const queryParams: any = {};
    
    if (searchQuery.keyword) queryParams.q = searchQuery.keyword;
    if (searchQuery.location) queryParams.location = searchQuery.location;
    if (searchQuery.salaryRange) queryParams.salary = searchQuery.salaryRange;
    if (searchQuery.experience) queryParams.experience = searchQuery.experience;
    if (searchQuery.category) queryParams.category = searchQuery.category;

    // Navigate to search results page
    this.router.navigate(['/website/search'], { queryParams });
  }

  // Additional helper methods for the template
  getUrgentJobsCount(): number {
    return this.featuredJobs.filter(job => job.urgent).length;
  }

  getNewJobsCount(): number {
    return this.homeStats.newJobsThisWeek;
  }

  getTotalJobsCount(): number {
    return this.homeStats.totalJobs;
  }

  // Helper method to group jobs for carousel display
  getJobGroups<T>(jobs: T[], groupSize: number): T[][] {
    const groups: T[][] = [];
    for (let i = 0; i < jobs.length; i += groupSize) {
      groups.push(jobs.slice(i, i + groupSize));
    }
    return groups;
  }

  // Format job count for display (e.g., 5365 -> "5.365")
  formatJobCount(count: number): string {
    return count.toLocaleString('vi-VN');
  }

  // TrackBy function for better performance
  trackByIndex(index: number, item: any): number {
    return index;
  }
}
