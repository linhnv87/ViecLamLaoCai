import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { HomeService } from '../../../../services/website/home.service';
import { FeaturedJob, PaginatedResponse } from '../../../../models/home.model';

export interface SearchFilters {
  location: string;
  category: string;
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
    category: '',
    salaryRange: '',
    experience: '',
    jobType: ''
  };

  // Pagination - SERVER-SIDE
  currentPage: number = 1;
  pageSize: number = 10;
  totalPages: number = 1;
  hasPreviousPage: boolean = false;
  hasNextPage: boolean = false;

  sortBy: string = 'relevance';
  sortOrder: string = 'desc';

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
      this.filters.category = params['category'] || '';
      this.filters.salaryRange = params['salary'] || '';
      this.filters.experience = params['experience'] || '';
      this.filters.jobType = params['jobType'] || '';
      this.currentPage = parseInt(params['page']) || 1;
      this.pageSize = parseInt(params['size']) || 10;
      this.sortBy = params['sort'] || 'relevance';
      this.sortOrder = params['order'] || 'desc';
      
      if (this.searchQuery || Object.values(this.filters).some(f => f)) {
        this.performSearch();
      }
    });
  }

  performSearch(): void {
    // Reset to empty if no search criteria
    if (!this.currentSearchQuery.trim() && !Object.values(this.filters).some(f => f)) {
      this.resetSearchResults();
      return;
    }

    this.isLoading = true;
    this.searchQuery = this.currentSearchQuery;

    this.updateUrl();
    const searchParams = {
      keyword: this.searchQuery,
      location: this.filters.location,
      category: this.filters.category,
      salaryRange: this.filters.salaryRange,
      experience: this.filters.experience,
      jobType: this.filters.jobType,
      page: this.currentPage,
      pageSize: this.pageSize,
      sortBy: this.sortBy,
      sortOrder: this.sortOrder
    };

    console.log('🔍 Performing search with params:', searchParams);

    this.homeService.searchJobs(searchParams).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        
        if (res.isSuccess && res.result) {
          const paginatedResult = res.result as PaginatedResponse<FeaturedJob>;
          
          this.searchResults = paginatedResult.data;
          this.totalResults = paginatedResult.totalItems;
          this.totalPages = paginatedResult.totalPages;
          this.currentPage = paginatedResult.currentPage;
          this.hasPreviousPage = paginatedResult.hasPreviousPage;
          this.hasNextPage = paginatedResult.hasNextPage;
          this.searchTime = paginatedResult.searchTime;
          
          console.log('✅ Search results:', {
            items: this.searchResults.length,
            totalItems: this.totalResults,
            currentPage: this.currentPage,
            totalPages: this.totalPages,
            searchTime: this.searchTime
          });
        } else {
          this.resetSearchResults();
          console.warn('❌ Search failed:', res.message);
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.resetSearchResults();
        console.error('❌ Search error:', error);
      }
    });
  }

  private resetSearchResults(): void {
    this.searchResults = [];
    this.totalResults = 0;
    this.totalPages = 1;
    this.hasPreviousPage = false;
    this.hasNextPage = false;
    this.searchTime = 0;
  }

  onSortChange(): void {
    // Reset to first page when sorting changes
    this.currentPage = 1;
    this.performSearch();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.performSearch();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  getTotalPages(): number {
    return this.totalPages;
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
    return (this.currentPage - 1) * this.pageSize;
  }

  getEndIndex(): number {
    return Math.min(this.getStartIndex() + this.pageSize, this.totalResults);
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
    if (this.filters.category) queryParams.category = this.filters.category;
    if (this.filters.salaryRange) queryParams.salary = this.filters.salaryRange;
    if (this.filters.experience) queryParams.experience = this.filters.experience;
    if (this.filters.jobType) queryParams.jobType = this.filters.jobType;
    if (this.currentPage > 1) queryParams.page = this.currentPage;
    if (this.pageSize !== 10) queryParams.size = this.pageSize;
    if (this.sortBy !== 'relevance') queryParams.sort = this.sortBy;
    if (this.sortOrder !== 'desc') queryParams.order = this.sortOrder;

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
    this.currentPage = 1;
    this.resetSearchResults();
    this.updateUrl();
  }

  clearAllFilters(): void {
    this.filters = {
      location: '',
      category: '',
      salaryRange: '',
      experience: '',
      jobType: ''
    };
    this.currentPage = 1;
    
    if (this.currentSearchQuery.trim()) {
      this.performSearch();
    } else {
      this.resetSearchResults();
      this.updateUrl();
    }
  }

  hasActiveFilters(): boolean {
    return !!(this.filters.location || 
              this.filters.category ||
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
