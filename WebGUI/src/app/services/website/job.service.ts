import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { JobDetailModel, HotJob } from '../../models/job.model';

@Injectable({ providedIn: 'root' })
export class JobService {
  getByIdDummy(id: number): Observable<BaseResponseModel<JobDetailModel>> {
    const job: JobDetailModel = {
      id,
      title: 'Nhân viên Sale Tour Quốc Tế/Nội Địa',
      company: 'Công ty Du lịch ABC Travel',
      logo: 'assets/vieclamlaocai/img/image 16.png',
      salary: '12 - 20 triệu',
      location: 'Lào Cai',
      views: 1234,
      postedDate: '2024-01-15',
      jobType: 'Full-time',
      experience: '1-2 năm',
      quantity: 3,
      deadline: '2024-02-15',
      position: 'Nhân viên',
      description: 'Tư vấn và bán các tour du lịch trong nước và quốc tế cho khách hàng. Chăm sóc khách hàng và theo dõi hành trình.',
      requirements: [
        'Tốt nghiệp Cao đẳng/Đại học chuyên ngành Du lịch, Kinh tế hoặc liên quan',
        'Kỹ năng giao tiếp tốt, tự tin',
        'Ưu tiên có kinh nghiệm trong lĩnh vực du lịch/lữ hành'
      ],
      benefits: [
        'Lương cứng + hoa hồng theo doanh số',
        'Thưởng quý/năm và du lịch công ty',
        'BHXH, BHYT đầy đủ'
      ],
      gender: 'Không yêu cầu',
      degree: 'Cao đẳng',
      language: 'Tiếng Việt',
      status: 'Đang tuyển',
      contactName: 'Nguyễn Văn A',
      contactPhone: '0901 234 567',
      contactEmail: 'hr@abctravel.com',
      companyAddress: '123 Đường Lào Cai, TP. Lào Cai',
      companyDescription: 'ABC Travel là công ty du lịch với hơn 10 năm kinh nghiệm, cung cấp tour trong nước và quốc tế.',
      website: 'https://abctravel.vn',
      categories: ['Hành chính', 'Thư ký', 'Trợ lý', 'Tài chính', 'Kế toán', 'Kiểm toán', 'Nhân sự']
    };

    return of({
      isSuccess: true,
      statusCode: 200,
      message: 'Success',
      result: job
    });
  }

  getHotJobsDummy(count: number = 5): Observable<BaseResponseModel<HotJob[]>> {
    const jobs: HotJob[] = [
      { id: 11, title: 'Chuyên viên Kinh doanh', company: 'Công ty ABC', salary: '15 - 25 triệu', location: 'Lào Cai', logo: 'assets/vieclamlaocai/img/image 19.png', deadline: '8' },
      { id: 12, title: 'Nhân viên Kế toán', company: 'Tập đoàn XYZ', salary: '12 - 18 triệu', location: 'Hà Nội', logo: 'assets/vieclamlaocai/img/image 21.png', deadline: '5' },
      { id: 13, title: 'Marketing Executive', company: 'Công ty DEF', salary: '10 - 20 triệu', location: 'Lào Cai', logo: 'assets/vieclamlaocai/img/image 22.png', deadline: '12' },
      { id: 14, title: 'Frontend Developer', company: 'Tech JKL', salary: '20 - 35 triệu', location: 'Hà Nội', logo: 'assets/vieclamlaocai/img/image 16.png', deadline: '3' },
      { id: 15, title: 'Chăm sóc khách hàng', company: 'Công ty MNO', salary: '8 - 12 triệu', location: 'Lào Cai', logo: 'assets/vieclamlaocai/img/image 23.png', deadline: '7' },
    ].slice(0, count);

    return of({
      isSuccess: true,
      statusCode: 200,
      message: 'Success',
      result: jobs
    });
  }
}


