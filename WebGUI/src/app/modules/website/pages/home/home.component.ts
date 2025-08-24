import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { HomeService } from '../../../../services/website/home.service';
import { FeaturedJob, SuggestedJob, JobCategory, HomePageStats, LatestJob, FeaturedCompany } from '../../../../models/home.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, AfterViewInit {

  featuredJobs: FeaturedJob[] = [];
  suggestedJobs: SuggestedJob[] = [];
  jobCategories: JobCategory[] = [];
  latestJobs: LatestJob[] = [];
  featuredCompanies: FeaturedCompany[] = [];
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

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.initializeCarousels();
    }, 1000);
  }

  private initializeCarousels(): void {
    if (typeof (window as any).$ !== 'undefined' && (window as any).$.fn.owlCarousel) {
      console.log('Initializing carousels...');
      
      // Initialize company slide carousel
      if ((window as any).$('.js-company-slide').length && !(window as any).$('.js-company-slide').hasClass('owl-loaded')) {
        try {
          (window as any).$('.js-company-slide').owlCarousel({
            items: 2,
            slideSpeed: 1000,
            autoplay: true,
            loop: true,
            smartSpeed: 1000,
            margin: 0,
            fluidSpeed: 500,
            autoplayTimeout: 5000,
            dots: true,
            nav: false,
            responsive: {
              576: { items: 2 },
              992: { items: 3 },
              1200: { items: 5 }
            },
            animateIn: 'fadeIn',
            animateOut: 'fadeOut'
          });
          console.log('Company carousel initialized successfully');
        } catch (e) {
          console.error('Error initializing company carousel:', e);
        }
      } else {
        console.log('Company carousel already initialized or not found');
      }
    } else {
      console.log('jQuery or owlCarousel not available, retrying...');
      setTimeout(() => {
        this.initializeCarousels();
      }, 500);
    }
  }

  public reinitializeCarousels(): void {
    console.log('Manually reinitializing carousels...');
    this.initializeCarousels();
  }

  loadHomePageData(): void {
    this.splashScreenService.show({
      type: 'loading',
      title: 'Chào mừng đến Việc Làm Lào Cai!',
      message: 'Đang tải việc làm mới nhất cho bạn...',
      showProgress: true
    });

    this.loadFeaturedJobs();
    this.loadSuggestedJobs();
    this.loadJobCategories();
    this.loadLatestJobs();
    this.loadFeaturedCompanies();
    this.loadHomeStats();

    setTimeout(() => {
      this.splashScreenService.hide();
    }, 800);
  }

  loadFeaturedJobs(): void {
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
    setTimeout(() => {
      this.homeService.getJobCategoriesDummy().subscribe(res => {
        if (res.isSuccess) {
          this.jobCategories = res.result;
          console.log('Job categories loaded:', this.jobCategories.length);
        }
      });
    }, 300);
  }

  loadLatestJobs(): void {
    setTimeout(() => {
      this.homeService.getLatestJobsDummy(8).subscribe(res => {
        if (res.isSuccess) {
          this.latestJobs = res.result;
          console.log('Latest jobs loaded:', this.latestJobs.length);
        }
      });
    }, 400);
  }

  loadFeaturedCompanies(): void {
    setTimeout(() => {
      this.homeService.getFeaturedCompaniesDummy(7).subscribe(res => {
        if (res.isSuccess) {
          this.featuredCompanies = res.result;
          console.log('Featured companies loaded:', this.featuredCompanies.length);
          setTimeout(() => {
            this.initializeCarousels();
          }, 100);
        }
      });
    }, 500);
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
    const queryParams: any = {};
    if (searchQuery.keyword) queryParams.q = searchQuery.keyword;
    if (searchQuery.location) queryParams.location = searchQuery.location;
    if (searchQuery.salaryRange) queryParams.salary = searchQuery.salaryRange;
    if (searchQuery.experience) queryParams.experience = searchQuery.experience;
    if (searchQuery.category) queryParams.category = searchQuery.category;

    this.router.navigate(['/website/search'], { queryParams });
  }

  getUrgentJobsCount(): number {
    return this.featuredJobs.filter(job => job.urgent).length;
  }

  getNewJobsCount(): number {
    return this.homeStats.newJobsThisWeek;
  }

  getTotalJobsCount(): number {
    return this.homeStats.totalJobs;
  }

  getJobGroups<T>(jobs: T[], groupSize: number): T[][] {
    const groups: T[][] = [];
    for (let i = 0; i < jobs.length; i += groupSize) {
      groups.push(jobs.slice(i, i + groupSize));
    }
    return groups;
  }

  formatJobCount(count: number): string {
    return count.toLocaleString('vi-VN');
  }

  trackByIndex(index: number, item: any): number {
    return index;
  }


}

