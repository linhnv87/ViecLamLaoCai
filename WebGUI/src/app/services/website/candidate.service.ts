import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { environment } from '../../../environments/environments';

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

@Injectable({
  providedIn: 'root'
})
export class CandidateService {
  private baseUrl = `${environment.apiUrl}/Candidate`;

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
}
