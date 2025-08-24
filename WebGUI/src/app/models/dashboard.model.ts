export interface BusinessDashboardModel {
  totalJobs: number;
  activeJobs: number;
  totalApplications: number;
  totalViews: number;
  todayApplications: number;
  todayViews: number;
  recentJobs: RecentJobModel[];
  recentCandidates: RecentCandidateModel[];
}

export interface CandidateDashboardModel {
  totalApplications: number;
  pendingApplications: number;
  approvedApplications: number;
  rejectedApplications: number;
  profileViews: number;
  suitableJobs: number;
  employerEmails: number;
  totalCVs: number;
  savedJobs: SavedJobModel[];
  recentApplications: RecentApplicationModel[];
  appliedJobs: AppliedJobModel[];
}

export interface AdminDashboardModel {
  totalBusinesses: number;
  pendingApprovals: number;
  approvedBusinesses: number;
  rejectedBusinesses: number;
  totalJobs: number;
  totalCandidates: number;
  todayRegistrations: number;
  systemHealth: SystemHealthModel;
}

export interface RecentJobModel {
  id: number;
  title: string;
  company: string;
  applications: number;
  views: number;
  postedDate: string;
  status: string;
  urgent: boolean;
}

export interface RecentCandidateModel {
  id: number;
  name: string;
  position: string;
  experience: string;
  education: string;
  appliedDate: string;
  status: string;
  avatar: string;
  age: number;
  location: string;
  level: string;
  industry: string;
  previousCompany: string;
  salaryExpectation: string;
}

export interface SavedJobModel {
  id: number;
  jobTitle: string;
  company: string;
  logo: string;
  salary: string;
  savedDate: string;
  location: string;
  urgent: boolean;
}

export interface RecentApplicationModel {
  id: number;
  jobTitle: string;
  company: string;
  logo: string;
  salary: string;
  status: string;
  appliedDate: string;
  location: string;
}

export interface AppliedJobModel {
  id: number;
  jobTitle: string;
  company: string;
  logo: string;
  salary: string;
  status: string;
  appliedDate: string;
  location: string;
}

export interface ActivityModel {
  id: number;
  type: 'application' | 'job_view' | 'profile_update' | 'message';
  title: string;
  description: string;
  timestamp: string;
  userId: string;
}

export interface SystemHealthModel {
  status: 'healthy' | 'warning' | 'error';
  uptime: string;
  responseTime: number;
  activeUsers: number;
}
