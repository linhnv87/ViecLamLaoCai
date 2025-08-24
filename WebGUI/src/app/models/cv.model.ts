export interface CVModel {
  id: number;
  candidateId: number;
  title: string;
  template: 'modern' | 'classic' | 'creative' | 'professional';
  personalInfo: PersonalInfo;
  summary: string;
  experience: WorkExperience[];
  education: Education[];
  skills: Skill[];
  languages: Language[];
  certifications: Certification[];
  projects: Project[];
  references: Reference[];
  interests?: string;
  computerSkills?: string;
  createdDate: string;
  updatedDate: string;
  isActive: boolean;
  isPublic: boolean;
  downloadCount: number;
  viewCount: number;
}

export interface PersonalInfo {
  fullName: string;
  email: string;
  phone: string;
  address: string;
  dateOfBirth?: string;
  nationality?: string;
  profileImage?: string;
  linkedIn?: string;
  github?: string;
  portfolio?: string;
  jobTitle?: string;
  gender?: string;
}

export interface WorkExperience {
  id?: number;
  jobTitle: string;
  companyName: string;
  location: string;
  startDate: string;
  endDate?: string;
  isCurrentJob: boolean;
  description: string;
  achievements: string[];
}

export interface Education {
  id?: number;
  degree: string;
  institution: string;
  school?: string;
  fieldOfStudy?: string;
  location: string;
  startDate: string;
  endDate?: string;
  isCurrentStudy: boolean;
  gpa?: string;
  grade?: string;
  description?: string;
}

export interface Skill {
  id?: number;
  name: string;
  level: 'Beginner' | 'Intermediate' | 'Advanced' | 'Expert' | '';
  category: 'Technical' | 'Soft' | 'Language' | 'Other' | '';
}

export interface Language {
  id?: number;
  name: string;
  proficiency: 'Basic' | 'Conversational' | 'Fluent' | 'Native';
  level?: string;
  certification?: string;
}

export interface Certification {
  id?: number;
  name: string;
  organization: string;
  issuer?: string;
  issueDate: string;
  expiryDate?: string;
  credentialId?: string;
  credentialUrl?: string;
}

export interface Project {
  id?: number;
  name: string;
  description: string;
  technologies: string[];
  startDate: string;
  endDate?: string;
  projectUrl?: string;
  githubUrl?: string;
  role: string;
}

export interface Reference {
  id?: number;
  name: string;
  position: string;
  company: string;
  email: string;
  phone?: string;
  relationship: string;
}

export interface CVTemplate {
  id: string;
  name: string;
  description: string;
  preview: string;
  category: 'modern' | 'classic' | 'creative' | 'professional';
  isPremium: boolean;
}

export interface CVSearchModel {
  keyword?: string;
  skills?: string[];
  location?: string;
  experience?: string;
  page: number;
  limit: number;
}
