export interface FeaturedJob {
  id: number;
  title: string;
  company: string;
  logo: string;
  salary: string;
  location: string;
  urgent: boolean;
  daysLeft: number;
  featured: boolean;
  description?: string;
  requirements?: string[];
  benefits?: string[];
  jobType?: string;
  experience?: string;
  postedDate?: string;
}

export interface SuggestedJob {
  id: number;
  title: string;
  company: string;
  logo: string;
  salary: string;
  experience: string;
  location: string;
  description?: string;
  requirements?: string[];
  jobType?: string;
  postedDate?: string;
}

export interface JobCategory {
  icon: string;
  title: string;
  count: number;
  description?: string;
  growthRate?: number;
  averageSalary?: string;
}

export interface HomePageStats {
  totalJobs: number;
  totalCompanies: number;
  totalCandidates: number;
  newJobsThisWeek: number;
  featuredJobsCount: number;
  urgentJobsCount: number;
}

export interface LatestJob {
  id: number;
  title: string;
  company: string;
  logo: string;
  salary: string;
  location: string;
  urgent: boolean;
  daysLeft: number;
  postedDate: string;
  jobType?: string;
  experience?: string;
}

export interface FeaturedCompany {
  id: number;
  name: string;
  logo: string;
  jobCount: number;
  industry: string;
  size: string;
  location: string;
  verified: boolean;
  description?: string;
  website?: string;
}

export interface HomePageData {
  stats: HomePageStats;
  featuredJobs: FeaturedJob[];
  suggestedJobs: SuggestedJob[];
  jobCategories: JobCategory[];
  recentActivities: any[];
}
