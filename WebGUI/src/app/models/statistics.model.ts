export interface StatisticsOverview {
  totalBusinesses: number;
  totalJobPostings: number;
  totalApplications: number;
  totalCandidates: number;
  pendingApprovals: number;
  activeJobs: number;
  monthlyGrowth: number;
  yearlyGrowth: number;
}

export interface TrendData {
  month: string;
  value: number;
  change: number;
}

export interface SearchTrendData {
  keyword: string;
  count: number;
  trend: 'up' | 'down' | 'stable';
  percentage: number;
}

export interface IndustryStatistic {
  name: string;
  businessCount: number;
  jobCount: number;
  percentage: number;
  color: string;
}
