export interface JobDetailModel {
  id: number;
  title: string;
  company: string;
  logo: string;
  salary: string;
  location: string;
  views: number;
  postedDate: string;
  jobType: string;
  experience: string;
  quantity: number;
  deadline: string;
  position?: string;
  description: string;
  requirements: string[];
  benefits: string[];
  gender?: string;
  degree?: string;
  language?: string;
  status: string;
  contactName: string;
  contactPhone: string;
  contactEmail: string;
  companyAddress: string;
  companyDescription: string;
  website?: string;
  categories?: string[];
}

export interface HotJob {
  id: number;
  title: string;
  company: string;
  salary: string;
  location: string;
  logo?: string;
  deadline?: string;
}
