export interface CandidateModel {
  id: number;
  name: string;
  email: string;
  phone: string;
  position: string;
  experience: string;
  education: string;
  skills: string[];
  location: string;
  salary: string;
  avatar: string;
  status: 'available' | 'open-to-work' | 'busy';
  lastActive: string;
  resume?: string;
  portfolio?: string;
  summary?: string;
}

export interface CandidateSearchModel {
  keyword?: string;
  skills?: string[];
  experience?: string;
  location?: string;
  salary?: string;
  status?: string;
  page: number;
  limit: number;
}

export interface CandidateRegistrationModel {
  fullName: string;
  email: string;
  phone: string;
  password: string;
  position?: string;
  experience?: string;
  education?: string;
  skills?: string[];
  location?: string;
  expectedSalary?: string;
}
