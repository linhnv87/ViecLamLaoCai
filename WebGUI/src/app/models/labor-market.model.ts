export interface MarketOverview {
  totalActiveJobs: number;
  totalCompaniesHiring: number;
  avgTimeToHire: number;
  jobGrowthRate: number;
  candidateCompetition: number;
  salaryGrowth: number;
}

export interface JobTrend {
  industry: string;
  demandLevel: 'high' | 'medium' | 'low';
  growth: number;
  avgSalary: string;
  topPositions: string[];
  color: string;
}

export interface SkillDemand {
  skill: string;
  demand: number;
  growth: number;
  category: string;
  level: 'hot' | 'stable' | 'declining';
}

export interface SalaryRange {
  position: string;
  minSalary: number;
  maxSalary: number;
  avgSalary: number;
  experience: string;
  count: number;
}

export interface MarketInsight {
  title: string;
  description: string;
  impact: 'positive' | 'negative' | 'neutral';
  category: string;
  date: string;
}
