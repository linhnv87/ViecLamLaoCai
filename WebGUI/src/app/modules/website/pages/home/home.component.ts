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

    this.homeService.getHomePageData().subscribe({
      next: (response) => {
        if (response.isSuccess && response.result) {
          this.featuredJobs = response.result.featuredJobs;
          this.suggestedJobs = response.result.suggestedJobs;
          this.jobCategories = response.result.jobCategories;
          this.latestJobs = response.result.latestJobs;
          this.featuredCompanies = response.result.featuredCompanies;
          this.homeStats = response.result.stats;
          
          console.log('Home page data loaded successfully');
        } else {
          console.warn('Failed to load home page data:', response.message);
          this.loadDummyData();
        }
      },
      error: (error) => {
        console.error('Error loading home page data:', error);
        this.loadDummyData();
      },
      complete: () => {
        setTimeout(() => {
          this.splashScreenService.hide();
        }, 800);
      }
    });
  }

  private loadDummyData(): void {
    console.log('Loading dummy data as fallback...');
    
    this.loadFeaturedJobs();
    this.loadSuggestedJobs();
    this.loadJobCategories();
    this.loadLatestJobs();
    this.loadFeaturedCompanies();
    this.loadHomeStats();
  }

  loadFeaturedJobs(): void {
    setTimeout(() => {
      this.homeService.getFeaturedJobs(6).subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.featuredJobs = response.result;
            console.log('Featured jobs loaded:', this.featuredJobs.length);
          } else {
            
            this.homeService.getFeaturedJobsDummy(6).subscribe(res => {
              if (res.isSuccess) {
                this.featuredJobs = res.result;
                console.log('Featured jobs loaded (dummy):', this.featuredJobs.length);
              }
            });
          }
        },
        error: (error) => {
          console.error('Error loading featured jobs:', error);
          
          this.homeService.getFeaturedJobsDummy(6).subscribe(res => {
            if (res.isSuccess) {
              this.featuredJobs = res.result;
              console.log('Featured jobs loaded (dummy):', this.featuredJobs.length);
            }
          });
        }
      });
    }, 100);
  }

  loadSuggestedJobs(): void {
    setTimeout(() => {
      
      const userId = this.getCurrentUserId();
      console.log('Loading suggested jobs for user:', userId);
      
      this.homeService.getSuggestedJobs(userId, 6).subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.suggestedJobs = response.result;
            console.log('Suggested jobs loaded:', this.suggestedJobs.length);
          } else {
           
            this.homeService.getSuggestedJobsDummy(userId, 6).subscribe(res => {
              if (res.isSuccess) {
                this.suggestedJobs = res.result;
                console.log('Suggested jobs loaded (dummy):', this.suggestedJobs.length);
              }
            });
          }
        },
        error: (error) => {
          console.error('Error loading suggested jobs:', error);
          
          this.homeService.getSuggestedJobsDummy(userId, 6).subscribe(res => {
            if (res.isSuccess) {
              this.suggestedJobs = res.result;
              console.log('Suggested jobs loaded (dummy):', this.suggestedJobs.length);
            }
          });
        }
      });
    }, 200);
  }

  private getCurrentUserId(): string | undefined {
    try {
      
      const userInfo = localStorage.getItem('userInfo');
      if (userInfo) {
        const user = JSON.parse(userInfo);
        return user.userId || user.id;
      }
      const sessionUser = sessionStorage.getItem('currentUser');
      if (sessionUser) {
        const user = JSON.parse(sessionUser);
        return user.userId || user.id;
      }

      return undefined;
    } catch (error) {
      console.warn('Error getting current user ID:', error);
      return undefined;
    }
  }

  loadJobCategories(): void {
    setTimeout(() => {
      this.homeService.getJobCategories().subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.jobCategories = response.result;
            console.log('Job categories loaded:', this.jobCategories.length);
          } else {
            
            this.homeService.getJobCategoriesDummy().subscribe(res => {
              if (res.isSuccess) {
                this.jobCategories = res.result;
                console.log('Job categories loaded (dummy):', this.jobCategories.length);
              }
            });
          }
        },
        error: (error) => {
          console.error('Error loading job categories:', error);
         
          this.homeService.getJobCategoriesDummy().subscribe(res => {
            if (res.isSuccess) {
              this.jobCategories = res.result;
              console.log('Job categories loaded (dummy):', this.jobCategories.length);
            }
          });
        }
      });
    }, 300);
  }

  loadLatestJobs(): void {
    setTimeout(() => {
      this.homeService.getLatestJobs(8).subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.latestJobs = response.result;
            console.log('Latest jobs loaded:', this.latestJobs.length);
          } else {
            
            this.homeService.getLatestJobsDummy(8).subscribe(res => {
              if (res.isSuccess) {
                this.latestJobs = res.result;
                console.log('Latest jobs loaded (dummy):', this.latestJobs.length);
              }
            });
          }
        },
        error: (error) => {
          console.error('Error loading latest jobs:', error);
          this.homeService.getLatestJobsDummy(8).subscribe(res => {
            if (res.isSuccess) {
              this.latestJobs = res.result;
              console.log('Latest jobs loaded (dummy):', this.latestJobs.length);
            }
          });
        }
      });
    }, 400);
  }

  loadFeaturedCompanies(): void {
    setTimeout(() => {
      this.homeService.getFeaturedCompanies(7).subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.featuredCompanies = response.result;
            console.log('Featured companies loaded:', this.featuredCompanies.length);
            setTimeout(() => {
              this.initializeCarousels();
            }, 100);
          } else {
            this.homeService.getFeaturedCompaniesDummy(7).subscribe(res => {
              if (res.isSuccess) {
                this.featuredCompanies = res.result;
                console.log('Featured companies loaded (dummy):', this.featuredCompanies.length);
                setTimeout(() => {
                  this.initializeCarousels();
                }, 100);
              }
            });
          }
        },
        error: (error) => {
          console.error('Error loading featured companies:', error);
          this.homeService.getFeaturedCompaniesDummy(7).subscribe(res => {
            if (res.isSuccess) {
              this.featuredCompanies = res.result;
              console.log('Featured companies loaded (dummy):', this.featuredCompanies.length);
              setTimeout(() => {
                this.initializeCarousels();
              }, 100);
            }
          });
        }
      });
    }, 500);
  }

  loadHomeStats(): void {
    this.homeService.getHomeStats().subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.homeStats = response.result;
          console.log('Home stats loaded:', this.homeStats);
        } else {
          this.homeService.getHomeStatsDummy().subscribe(res => {
            if (res.isSuccess) {
              this.homeStats = res.result;
              console.log('Home stats loaded (dummy):', this.homeStats);
            }
          });
        }
      },
      error: (error) => {
        console.error('Error loading home stats:', error);
        this.homeService.getHomeStatsDummy().subscribe(res => {
          if (res.isSuccess) {
            this.homeStats = res.result;
            console.log('Home stats loaded (dummy):', this.homeStats);
          }
        });
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

