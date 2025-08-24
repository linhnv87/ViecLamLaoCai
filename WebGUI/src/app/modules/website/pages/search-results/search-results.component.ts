import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { HomeService } from '../../../../services/website/home.service';
import { FeaturedJob } from '../../../../models/home.model';

export interface SearchFilters {
  location: string;
  salaryRange: string;
  experience: string;
  jobType: string;
}

@Component({
  selector: 'app-search-results',
  templateUrl: './search-results.component.html',
  styleUrls: ['./search-results.component.scss']
})
export class SearchResultsComponent implements OnInit {

  // Search state
  searchQuery: string = '';
  currentSearchQuery: string = '';
  searchResults: FeaturedJob[] = [];
  totalResults: number = 0;
  searchTime: number = 0;
  isLoading: boolean = false;

  // Filters
  filters: SearchFilters = {
    location: '',
    salaryRange: '',
    experience: '',
    jobType: ''
  };

  // Pagination
  currentPage: number = 1;
  itemsPerPage: number = 10;
  paginatedResults: FeaturedJob[] = [];

  sortBy: string = 'relevance';

  savedJobIds: Set<number> = new Set();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private splashScreenService: SplashScreenService,
    private homeService: HomeService
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.searchQuery = params['q'] || '';
      this.currentSearchQuery = this.searchQuery;
      this.filters.location = params['location'] || '';
      this.filters.salaryRange = params['salary'] || '';
      this.filters.experience = params['experience'] || '';
      this.filters.jobType = params['jobType'] || '';
      this.currentPage = parseInt(params['page']) || 1;
      this.sortBy = params['sort'] || 'relevance';
      
      if (this.searchQuery) {
        this.performSearch();
      }
    });
  }

  performSearch(): void {
    if (!this.currentSearchQuery.trim()) {
      return;
    }

    this.isLoading = true;
    this.searchQuery = this.currentSearchQuery;
    const startTime = Date.now();

    this.updateUrl();
    const searchParams = {
      keyword: this.searchQuery,
      location: this.filters.location,
      salaryRange: this.filters.salaryRange,
      experience: this.filters.experience,
      jobType: this.filters.jobType
    };

    this.homeService.searchJobsDummy(searchParams).subscribe(res => {
      this.isLoading = false;
      this.searchTime = Date.now() - startTime;
      
      if (res.isSuccess) {
        this.searchResults = res.result as FeaturedJob[];
        this.totalResults = this.searchResults.length;
        this.sortResults();
        this.updatePagination();
      } else {
        this.searchResults = [];
        this.totalResults = 0;
        this.paginatedResults = [];
      }
    });
  }

  sortResults(): void {
    switch (this.sortBy) {
      case 'date':
        this.searchResults.sort((a, b) => 
          new Date(b.postedDate || '').getTime() - new Date(a.postedDate || '').getTime()
        );
        break;
      case 'salary':
        this.searchResults.sort((a, b) => {
          const salaryA = this.extractMaxSalary(a.salary);
          const salaryB = this.extractMaxSalary(b.salary);
          return salaryB - salaryA;
        });
        break;
      case 'company':
        this.searchResults.sort((a, b) => a.company.localeCompare(b.company));
        break;
      default:
        break;
    }
    this.updatePagination();
    this.updateUrl();
  }

  updatePagination(): void {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    this.paginatedResults = this.searchResults.slice(startIndex, endIndex);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.getTotalPages()) {
      this.currentPage = page;
      this.updatePagination();
      this.updateUrl();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  getTotalPages(): number {
    return Math.ceil(this.totalResults / this.itemsPerPage);
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

  getStartIndex(): number {
    return (this.currentPage - 1) * this.itemsPerPage;
  }

  getEndIndex(): number {
    return Math.min(this.getStartIndex() + this.itemsPerPage, this.totalResults);
  }

  saveJob(job: FeaturedJob): void {
    if (this.savedJobIds.has(job.id)) {
      this.savedJobIds.delete(job.id);
      this.splashScreenService.showQuickFeedback('success', 'Đã bỏ lưu', `Đã bỏ lưu việc làm "${job.title}"`);
    } else {
      this.savedJobIds.add(job.id);
      this.splashScreenService.showQuickFeedback('success', 'Đã lưu', `Đã lưu việc làm "${job.title}"`);
    }
  }

  isJobSaved(jobId: number): boolean {
    return this.savedJobIds.has(jobId);
  }

  applyJob(job: FeaturedJob): void {
    this.splashScreenService.showQuickFeedback('success', 'Chuyển hướng', `Đang chuyển đến trang ứng tuyển cho "${job.title}"`);
    setTimeout(() => {
      this.router.navigate(['/website/register-candidate'], { 
        queryParams: { jobId: job.id, jobTitle: job.title } 
      });
    }, 1000);
  }

  searchSuggestion(keyword: string): void {
    this.currentSearchQuery = keyword;
    this.performSearch();
  }

  private updateUrl(): void {
    const queryParams: any = {};
    
    if (this.searchQuery) queryParams.q = this.searchQuery;
    if (this.filters.location) queryParams.location = this.filters.location;
    if (this.filters.salaryRange) queryParams.salary = this.filters.salaryRange;
    if (this.filters.experience) queryParams.experience = this.filters.experience;
    if (this.filters.jobType) queryParams.jobType = this.filters.jobType;
    if (this.currentPage > 1) queryParams.page = this.currentPage;
    if (this.sortBy !== 'relevance') queryParams.sort = this.sortBy;

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: queryParams,
      replaceUrl: true
    });
  }

  onSearchInputChange(): void {
    if (!this.currentSearchQuery.trim()) {
      this.clearSearch();
    }
  }

  clearSearch(): void {
    this.currentSearchQuery = '';
    this.searchQuery = '';
    this.searchResults = [];
    this.paginatedResults = [];
    this.totalResults = 0;
    this.currentPage = 1;
    
    this.updateUrl();
  }

  clearAllFilters(): void {
    this.filters = {
      location: '',
      salaryRange: '',
      experience: '',
      jobType: ''
    };
    this.currentPage = 1;
    
    if (this.currentSearchQuery.trim()) {
      this.performSearch();
    } else {
      this.updateUrl();
    }
  }

  hasActiveFilters(): boolean {
    return !!(this.filters.location || 
              this.filters.salaryRange || 
              this.filters.experience || 
              this.filters.jobType ||
              this.currentSearchQuery);
  }

  private extractMaxSalary(salaryString: string): number {
    const matches = salaryString.match(/(\d+)\s*-\s*(\d+)/);
    if (matches) {
      return parseInt(matches[2]);
    }
    const singleMatch = salaryString.match(/(\d+)/);
    return singleMatch ? parseInt(singleMatch[1]) : 0;
  }
}
