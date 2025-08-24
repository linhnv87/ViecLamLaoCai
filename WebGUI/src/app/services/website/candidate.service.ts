import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { CandidateModel, CandidateSearchModel, CandidateRegistrationModel } from '../../models/candidate.model';
import { CVModel, CVTemplate } from '../../models/cv.model';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class CandidateService {
  private baseUrl = `${environment.apiUrl}/Candidate`;
  private currentEditingCV: CVModel | null = null;

  constructor(private http: HttpClient) {}

  // GET /api/Candidate/GetAll
  getAll(page: number = 1, limit: number = 10, filters?: any): Observable<BaseResponseModel<CandidateModel[]>> {
    let apiUrl = `${this.baseUrl}/GetAll?page=${page}&limit=${limit}`;
    if (filters?.skills) apiUrl += `&skills=${filters.skills}`;
    if (filters?.experience) apiUrl += `&experience=${filters.experience}`;
    if (filters?.location) apiUrl += `&location=${filters.location}`;
    
    return this.http.get<BaseResponseModel<CandidateModel[]>>(apiUrl);
  }

  // GET /api/Candidate/GetById/{id}
  getById(id: number): Observable<BaseResponseModel<CandidateModel>> {
    const apiUrl = `${this.baseUrl}/GetById/${id}`;
    return this.http.get<BaseResponseModel<CandidateModel>>(apiUrl);
  }

  // POST /api/Candidate/Create
  create(candidate: CandidateRegistrationModel): Observable<BaseResponseModel<CandidateModel>> {
    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<CandidateModel>>(apiUrl, candidate);
  }

  // PUT /api/Candidate/Update/{id}
  update(id: number, candidate: CandidateModel): Observable<BaseResponseModel<CandidateModel>> {
    const apiUrl = `${this.baseUrl}/Update/${id}`;
    return this.http.put<BaseResponseModel<CandidateModel>>(apiUrl, candidate);
  }

  // POST /api/Candidate/Search
  search(searchModel: CandidateSearchModel): Observable<BaseResponseModel<CandidateModel[]>> {
    const apiUrl = `${this.baseUrl}/Search`;
    return this.http.post<BaseResponseModel<CandidateModel[]>>(apiUrl, searchModel);
  }

  // POST /api/Candidate/UploadResume/{candidateId}
  uploadResume(candidateId: number, file: File): Observable<BaseResponseModel<string>> {
    const formData = new FormData();
    formData.append('resume', file);
    
    const apiUrl = `${this.baseUrl}/UploadResume/${candidateId}`;
    return this.http.post<BaseResponseModel<string>>(apiUrl, formData);
  }

  // ===== CV MANAGEMENT METHODS =====

  // GET /api/Candidate/{candidateId}/CVs
  getCVs(candidateId: number): Observable<BaseResponseModel<CVModel[]>> {
    const apiUrl = `${this.baseUrl}/${candidateId}/CVs`;
    return this.http.get<BaseResponseModel<CVModel[]>>(apiUrl);
  }

  // GET /api/Candidate/CV/{cvId}
  getCV(cvId: number): Observable<BaseResponseModel<CVModel>> {
    const apiUrl = `${this.baseUrl}/CV/${cvId}`;
    return this.http.get<BaseResponseModel<CVModel>>(apiUrl);
  }

  // POST /api/Candidate/CV
  createCV(cvData: CVModel): Observable<BaseResponseModel<CVModel>> {
    const apiUrl = `${this.baseUrl}/CV`;
    return this.http.post<BaseResponseModel<CVModel>>(apiUrl, cvData);
  }

  // PUT /api/Candidate/CV/{cvId}
  updateCV(cvId: number, cvData: CVModel): Observable<BaseResponseModel<CVModel>> {
    const apiUrl = `${this.baseUrl}/CV/${cvId}`;
    return this.http.put<BaseResponseModel<CVModel>>(apiUrl, cvData);
  }

  // DELETE /api/Candidate/CV/{cvId}
  deleteCV(cvId: number): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/CV/${cvId}`;
    return this.http.delete<BaseResponseModel<boolean>>(apiUrl);
  }

  // GET /api/Candidate/CVTemplates
  getCVTemplates(): Observable<BaseResponseModel<CVTemplate[]>> {
    const apiUrl = `${this.baseUrl}/CVTemplates`;
    return this.http.get<BaseResponseModel<CVTemplate[]>>(apiUrl);
  }

  // POST /api/Candidate/CV/{cvId}/Download
  downloadCV(cvId: number, format: 'pdf' | 'word'): Observable<Blob> {
    const apiUrl = `${this.baseUrl}/CV/${cvId}/Download?format=${format}`;
    return this.http.post(apiUrl, {}, { responseType: 'blob' });
  }

  // POST /api/Candidate/CV/{cvId}/SetActive
  setActiveCV(cvId: number): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/CV/${cvId}/SetActive`;
    return this.http.post<BaseResponseModel<boolean>>(apiUrl, {});
  }

  // ===== CV EDITING METHODS =====
  setCurrentEditingCV(cv: CVModel): void {
    this.currentEditingCV = cv;
  }

  getCurrentEditingCV(): CVModel | null {
    return this.currentEditingCV;
  }

  clearCurrentEditingCV(): void {
    this.currentEditingCV = null;
  }

  // ===== DUMMY DATA METHODS (for development) =====

  getAllDummy(): Observable<BaseResponseModel<CandidateModel[]>> {
    const dummyData: CandidateModel[] = [
      {
        id: 1,
        name: 'Nguyễn Minh Khôi',
        email: 'khoi.nguyen@email.com',
        phone: '0987654321',
        position: 'Frontend Developer',
        experience: '3 năm kinh nghiệm',
        education: 'Đại học Bách Khoa Hà Nội',
        skills: ['ReactJS', 'Angular', 'Vue.js', 'TypeScript'],
        location: 'Lào Cai',
        salary: '15-25 triệu',
        avatar: 'assets/vieclamlaocai/img/image 16.png',
        status: 'available',
        lastActive: '2024-01-15'
      },
      {
        id: 2,
        name: 'Trần Thùy Linh',
        email: 'linh.tran@email.com',
        phone: '0123456789',
        position: 'UI/UX Designer',
        experience: '2 năm kinh nghiệm',
        education: 'Đại học Mỹ thuật Việt Nam',
        skills: ['Figma', 'Adobe XD', 'Sketch', 'Photoshop'],
        location: 'Lào Cai',
        salary: '12-20 triệu',
        avatar: 'assets/vieclamlaocai/img/image 23.png',
        status: 'open-to-work',
        lastActive: '2024-01-14'
      }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }

  // ===== CV DUMMY DATA METHODS (for development) =====

  getCVsDummy(candidateId: number): Observable<BaseResponseModel<CVModel[]>> {
    const dummyCVs: CVModel[] = [
      {
        id: 1,
        candidateId: candidateId,
        title: 'Frontend Developer CV',
        template: 'modern',
        personalInfo: {
          fullName: 'Nguyễn Minh Khôi',
          email: 'khoi.nguyen@email.com',
          phone: '0987654321',
          address: 'Lào Cai, Việt Nam',
          dateOfBirth: '1995-05-15',
          nationality: 'Việt Nam',
          profileImage: 'assets/vieclamlaocai/img/image 16.png',
          linkedIn: 'linkedin.com/in/khoi-nguyen',
          github: 'github.com/khoi-nguyen',
          portfolio: 'khoi-portfolio.com'
        },
        summary: 'Frontend Developer với 3+ năm kinh nghiệm phát triển ứng dụng web sử dụng React, Angular và Vue.js. Có khả năng làm việc nhóm tốt và đam mê học hỏi công nghệ mới.',
        experience: [
          {
            id: 1,
            jobTitle: 'Senior Frontend Developer',
            companyName: 'TechViet Solutions',
            location: 'Hà Nội',
            startDate: '2022-01-01',
            endDate: '',
            isCurrentJob: true,
            description: 'Phát triển và maintain các ứng dụng web sử dụng React và TypeScript',
            achievements: [
              'Tăng performance ứng dụng 40% thông qua code optimization',
              'Lead team 5 developers trong dự án e-commerce',
              'Implement CI/CD pipeline giảm deployment time 60%'
            ]
          },
          {
            id: 2,
            jobTitle: 'Frontend Developer',
            companyName: 'StartupXYZ',
            location: 'TP.HCM',
            startDate: '2020-06-01',
            endDate: '2021-12-31',
            isCurrentJob: false,
            description: 'Phát triển giao diện người dùng cho ứng dụng mobile và web',
            achievements: [
              'Phát triển 15+ components tái sử dụng',
              'Collaborate với UX team để cải thiện user experience',
              'Implement responsive design cho mobile devices'
            ]
          }
        ],
        education: [
          {
            id: 1,
            degree: 'Cử nhân Công nghệ Thông tin',
            institution: 'Đại học Bách Khoa Hà Nội',
            location: 'Hà Nội',
            startDate: '2016-09-01',
            endDate: '2020-05-31',
            isCurrentStudy: false,
            gpa: '3.7/4.0',
            description: 'Chuyên ngành Kỹ thuật Phần mềm'
          }
        ],
        skills: [
          { id: 1, name: 'ReactJS', level: 'Expert', category: 'Technical' },
          { id: 2, name: 'Angular', level: 'Advanced', category: 'Technical' },
          { id: 3, name: 'Vue.js', level: 'Intermediate', category: 'Technical' },
          { id: 4, name: 'TypeScript', level: 'Advanced', category: 'Technical' },
          { id: 5, name: 'HTML/CSS', level: 'Expert', category: 'Technical' },
          { id: 6, name: 'Team Leadership', level: 'Advanced', category: 'Soft' },
          { id: 7, name: 'Problem Solving', level: 'Expert', category: 'Soft' }
        ],
        languages: [
          { id: 1, name: 'Tiếng Việt', proficiency: 'Native' },
          { id: 2, name: 'English', proficiency: 'Fluent' },
          { id: 3, name: 'Tiếng Trung', proficiency: 'Basic' }
        ],
        certifications: [
          {
            id: 1,
            name: 'AWS Certified Developer',
            organization: 'Amazon Web Services',
            issueDate: '2023-03-15',
            expiryDate: '2026-03-15',
            credentialId: 'AWS-123456789',
            credentialUrl: 'aws.amazon.com/verification'
          },
          {
            id: 2,
            name: 'React Developer Certificate',
            organization: 'Meta',
            issueDate: '2022-08-20',
            credentialId: 'META-987654321'
          }
        ],
        projects: [
          {
            id: 1,
            name: 'E-commerce Platform',
            description: 'Nền tảng thương mại điện tử với React, Node.js và MongoDB',
            technologies: ['React', 'Node.js', 'MongoDB', 'Redux', 'Material-UI'],
            startDate: '2023-01-01',
            endDate: '2023-06-30',
            projectUrl: 'ecommerce-demo.com',
            githubUrl: 'github.com/khoi-nguyen/ecommerce',
            role: 'Lead Frontend Developer'
          },
          {
            id: 2,
            name: 'Task Management App',
            description: 'Ứng dụng quản lý công việc với tính năng real-time collaboration',
            technologies: ['Vue.js', 'Firebase', 'Vuetify', 'Socket.io'],
            startDate: '2022-06-01',
            endDate: '2022-10-31',
            projectUrl: 'taskmanager-demo.com',
            githubUrl: 'github.com/khoi-nguyen/taskmanager',
            role: 'Full-stack Developer'
          }
        ],
        references: [
          {
            id: 1,
            name: 'Trần Văn Minh',
            position: 'Technical Lead',
            company: 'TechViet Solutions',
            email: 'minh.tran@techviet.com',
            phone: '0912345678',
            relationship: 'Direct Manager'
          },
          {
            id: 2,
            name: 'Lê Thị Hương',
            position: 'Project Manager',
            company: 'StartupXYZ',
            email: 'huong.le@startupxyz.com',
            phone: '0987654321',
            relationship: 'Former Colleague'
          }
        ],
        createdDate: '2024-01-15',
        updatedDate: '2024-01-20',
        isActive: true,
        isPublic: true,
        downloadCount: 25,
        viewCount: 150
      },
      {
        id: 2,
        candidateId: candidateId,
        title: 'Junior Developer CV',
        template: 'classic',
        personalInfo: {
          fullName: 'Nguyễn Minh Khôi',
          email: 'khoi.nguyen@email.com',
          phone: '0987654321',
          address: 'Lào Cai, Việt Nam',
          profileImage: 'assets/vieclamlaocai/img/image 16.png'
        },
        summary: 'Sinh viên mới tốt nghiệp với nền tảng vững về lập trình web. Eager to learn và đóng góp vào team development.',
        experience: [],
        education: [
          {
            id: 1,
            degree: 'Cử nhân Công nghệ Thông tin',
            institution: 'Đại học Bách Khoa Hà Nội',
            location: 'Hà Nội',
            startDate: '2016-09-01',
            endDate: '2020-05-31',
            isCurrentStudy: false,
            gpa: '3.7/4.0'
          }
        ],
        skills: [
          { id: 1, name: 'HTML/CSS', level: 'Advanced', category: 'Technical' },
          { id: 2, name: 'JavaScript', level: 'Intermediate', category: 'Technical' },
          { id: 3, name: 'React', level: 'Beginner', category: 'Technical' }
        ],
        languages: [
          { id: 1, name: 'Tiếng Việt', proficiency: 'Native' },
          { id: 2, name: 'English', proficiency: 'Conversational' }
        ],
        certifications: [],
        projects: [
          {
            id: 1,
            name: 'Personal Portfolio',
            description: 'Website portfolio cá nhân showcase các dự án học tập',
            technologies: ['HTML', 'CSS', 'JavaScript'],
            startDate: '2020-03-01',
            endDate: '2020-05-31',
            projectUrl: 'khoi-portfolio.com',
            githubUrl: 'github.com/khoi-nguyen/portfolio',
            role: 'Developer'
          }
        ],
        references: [],
        createdDate: '2024-01-10',
        updatedDate: '2024-01-10',
        isActive: false,
        isPublic: false,
        downloadCount: 5,
        viewCount: 30
      }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyCVs
    });
  }

  getCVTemplatesDummy(): Observable<BaseResponseModel<CVTemplate[]>> {
    const templates: CVTemplate[] = [
      {
        id: 'modern',
        name: 'Modern',
        description: 'Template hiện đại với design sạch sẽ và professional',
        preview: 'assets/cv-templates/modern-preview.png',
        category: 'modern',
        isPremium: false
      },
      {
        id: 'classic',
        name: 'Classic',
        description: 'Template cổ điển, phù hợp cho mọi ngành nghề',
        preview: 'assets/cv-templates/classic-preview.png',
        category: 'classic',
        isPremium: false
      },
      {
        id: 'creative',
        name: 'Creative',
        description: 'Template sáng tạo cho các ngành nghề thiết kế, marketing',
        preview: 'assets/cv-templates/creative-preview.png',
        category: 'creative',
        isPremium: true
      },
      {
        id: 'professional',
        name: 'Professional',
        description: 'Template chuyên nghiệp cho các vị trí quản lý, điều hành',
        preview: 'assets/cv-templates/professional-preview.png',
        category: 'professional',
        isPremium: true
      }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: templates
    });
  }
}
