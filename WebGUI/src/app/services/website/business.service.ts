import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { BusinessApprovalData, BusinessApprovalStats, BusinessRegistrationModel, DocumentInfo } from '../../models/business.model';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class BusinessService {
  private baseUrl = `${environment.apiUrl}/Business`;

  constructor(private http: HttpClient) {}

  // GET /api/Business/GetAll
  getAll(): Observable<BaseResponseModel<BusinessApprovalData[]>> {
    const apiUrl = `${this.baseUrl}/GetAll`;
    return this.http.get<BaseResponseModel<BusinessApprovalData[]>>(apiUrl);
  }

  // GET /api/Business/GetPendingApprovals
  getPendingApprovals(): Observable<BaseResponseModel<BusinessApprovalData[]>> {
    const apiUrl = `${this.baseUrl}/GetPendingApprovals`;
    return this.http.get<BaseResponseModel<BusinessApprovalData[]>>(apiUrl);
  }

  // GET /api/Business/GetById/{id}
  getById(id: number): Observable<BaseResponseModel<BusinessApprovalData>> {
    const apiUrl = `${this.baseUrl}/GetById/${id}`;
    return this.http.get<BaseResponseModel<BusinessApprovalData>>(apiUrl);
  }

  // POST /api/Business/Create
  create(business: BusinessRegistrationModel): Observable<BaseResponseModel<BusinessApprovalData>> {
    const formData = new FormData();
    formData.append('businessName', business.businessName);
    formData.append('taxCode', business.taxCode);
    formData.append('contactPerson', business.contactPerson);
    formData.append('email', business.email);
    formData.append('phone', business.phone);
    formData.append('address', business.address);
    formData.append('businessType', business.businessType);
    
    business.documents.forEach((file, index) => {
      formData.append('documents', file);
    });

    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<BusinessApprovalData>>(apiUrl, formData);
  }

  // PUT /api/Business/Update/{id}
  update(id: number, business: BusinessApprovalData): Observable<BaseResponseModel<BusinessApprovalData>> {
    const apiUrl = `${this.baseUrl}/Update/${id}`;
    return this.http.put<BaseResponseModel<BusinessApprovalData>>(apiUrl, business);
  }

  // PUT /api/Business/Approve/{id}
  approve(id: number, approvedBy: string, notes?: string): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/Approve/${id}`;
    return this.http.put<BaseResponseModel<boolean>>(apiUrl, { approvedBy, notes });
  }

  // PUT /api/Business/Reject/{id}
  reject(id: number, rejectedBy: string, reason: string): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/Reject/${id}`;
    return this.http.put<BaseResponseModel<boolean>>(apiUrl, { rejectedBy, reason });
  }

  // PUT /api/Business/SetReviewing/{id}
  setReviewing(id: number, reviewedBy: string): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/SetReviewing/${id}`;
    return this.http.put<BaseResponseModel<boolean>>(apiUrl, { reviewedBy });
  }

  // GET /api/Business/GetStatistics
  getStatistics(): Observable<BaseResponseModel<BusinessApprovalStats>> {
    const apiUrl = `${this.baseUrl}/GetStatistics`;
    return this.http.get<BaseResponseModel<BusinessApprovalStats>>(apiUrl);
  }

  // GET /api/Business/GetDocuments/{businessId}
  getDocuments(businessId: number): Observable<BaseResponseModel<DocumentInfo[]>> {
    const apiUrl = `${this.baseUrl}/GetDocuments/${businessId}`;
    return this.http.get<BaseResponseModel<DocumentInfo[]>>(apiUrl);
  }

  // POST /api/Business/UploadDocument/{businessId}
  uploadDocument(businessId: number, file: File, documentType: string): Observable<BaseResponseModel<DocumentInfo>> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('documentType', documentType);
    
    const apiUrl = `${this.baseUrl}/UploadDocument/${businessId}`;
    return this.http.post<BaseResponseModel<DocumentInfo>>(apiUrl, formData);
  }

  // GET /api/Business/DownloadDocument/{documentId}
  downloadDocument(documentId: number): Observable<Blob> {
    const apiUrl = `${this.baseUrl}/DownloadDocument/${documentId}`;
    return this.http.get(apiUrl, { responseType: 'blob' });
  }

  verifyDocument(documentId: number): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/VerifyDocument/${documentId}`;
    return this.http.put<BaseResponseModel<boolean>>(apiUrl, {});
  }

  // ===== DUMMY DATA METHODS (for development) =====
  
  getAllDummy(): Observable<BaseResponseModel<BusinessApprovalData[]>> {
    const dummyData: BusinessApprovalData[] = [
      // Pending businesses
      {
        id: 1,
        businessName: 'Công ty TNHH Công nghệ ABC',
        taxCode: '0123456789',
        contactPerson: 'Nguyễn Văn A',
        email: 'contact@abc-tech.com',
        phone: '0987654321',
        address: '123 Đường Lào Cai, Phường 1, TP. Lào Cai',
        businessType: 'Công nghệ thông tin',
        registrationDate: '2024-01-15',
        status: 'pending',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 1, name: 'Giấy phép kinh doanh', type: 'PDF', url: '#', uploadDate: '2024-01-15', verified: true },
          { id: 2, name: 'Giấy chứng nhận đăng ký thuế', type: 'PDF', url: '#', uploadDate: '2024-01-15', verified: false }
        ]
      },
      {
        id: 2,
        businessName: 'Công ty Cổ phần Thương mại XYZ',
        taxCode: '0987654321',
        contactPerson: 'Trần Thị B',
        email: 'info@xyz-trading.com',
        phone: '0123456789',
        address: '456 Đường Hoàng Liên, Phường 2, TP. Lào Cai',
        businessType: 'Thương mại dịch vụ',
        registrationDate: '2024-01-14',
        status: 'reviewing',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 3, name: 'Giấy phép kinh doanh', type: 'PDF', url: '#', uploadDate: '2024-01-14', verified: true },
          { id: 4, name: 'Báo cáo tài chính', type: 'Excel', url: '#', uploadDate: '2024-01-14', verified: true }
        ]
      },
      {
        id: 3,
        businessName: 'Công ty TNHH Sản xuất DEF',
        taxCode: '0111222333',
        contactPerson: 'Lê Văn C',
        email: 'contact@def-manufacturing.com',
        phone: '0912345678',
        address: '789 Đường Fansipan, Phường 3, TP. Lào Cai',
        businessType: 'Sản xuất chế biến',
        registrationDate: '2024-01-13',
        status: 'pending',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 5, name: 'Giấy phép kinh doanh', type: 'PDF', url: '#', uploadDate: '2024-01-13', verified: false },
          { id: 6, name: 'Giấy phép môi trường', type: 'PDF', url: '#', uploadDate: '2024-01-13', verified: true }
        ]
      },
      {
        id: 4,
        businessName: 'Công ty CP Du lịch Sapa Green',
        taxCode: '0444555666',
        contactPerson: 'Phạm Thị D',
        email: 'info@sapagreen.com',
        phone: '0923456789',
        address: '321 Đường Muong Hoa, Phường Sa Pa, TP. Sa Pa',
        businessType: 'Du lịch dịch vụ',
        registrationDate: '2024-01-12',
        status: 'approved',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 7, name: 'Giấy phép kinh doanh lữ hành', type: 'PDF', url: '#', uploadDate: '2024-01-12', verified: true },
          { id: 8, name: 'Chứng chỉ an toàn thực phẩm', type: 'PDF', url: '#', uploadDate: '2024-01-12', verified: true }
        ]
      },
      {
        id: 5,
        businessName: 'Công ty TNHH Đầu tư Tài chính GHI',
        taxCode: '0777888999',
        contactPerson: 'Hoàng Văn E',
        email: 'invest@ghi-finance.com',
        phone: '0934567890',
        address: '654 Đường Trần Hưng Đạo, Phường 4, TP. Lào Cai',
        businessType: 'Đầu tư tài chính',
        registrationDate: '2024-01-11',
        status: 'rejected',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 9, name: 'Giấy phép hoạt động tài chính', type: 'PDF', url: '#', uploadDate: '2024-01-11', verified: false },
          { id: 10, name: 'Báo cáo tài chính', type: 'Excel', url: '#', uploadDate: '2024-01-11', verified: false }
        ]
      },
      {
        id: 6,
        businessName: 'Công ty TNHH TechSoft Solutions',
        taxCode: '0101010101',
        contactPerson: 'Vũ Thị F',
        email: 'hello@techsoft.vn',
        phone: '0945678901',
        address: '987 Đường Nguyễn Huệ, Phường 5, TP. Lào Cai',
        businessType: 'Công nghệ thông tin',
        registrationDate: '2024-01-10',
        status: 'pending',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 11, name: 'Giấy phép kinh doanh', type: 'PDF', url: '#', uploadDate: '2024-01-10', verified: true },
          { id: 12, name: 'Chứng nhận ISO 9001', type: 'PDF', url: '#', uploadDate: '2024-01-10', verified: true }
        ]
      },
      {
        id: 7,
        businessName: 'Công ty CP Nông nghiệp Organic',
        taxCode: '0202020202',
        contactPerson: 'Đỗ Văn G',
        email: 'organic@agri.com',
        phone: '0956789012',
        address: '147 Đường Điện Biên Phủ, Phường 6, TP. Lào Cai',
        businessType: 'Sản xuất chế biến',
        registrationDate: '2024-01-09',
        status: 'approved',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 13, name: 'Giấy chứng nhận hữu cơ', type: 'PDF', url: '#', uploadDate: '2024-01-09', verified: true },
          { id: 14, name: 'Giấy phép vệ sinh an toàn thực phẩm', type: 'PDF', url: '#', uploadDate: '2024-01-09', verified: true }
        ]
      },
      {
        id: 8,
        businessName: 'Công ty TNHH Logistics Express',
        taxCode: '0303030303',
        contactPerson: 'Bùi Thị H',
        email: 'logistics@express.vn',
        phone: '0967890123',
        address: '258 Đường Lý Thường Kiệt, Phường 7, TP. Lào Cai',
        businessType: 'Thương mại dịch vụ',
        registrationDate: '2024-01-08',
        status: 'reviewing',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 15, name: 'Giấy phép vận tải', type: 'PDF', url: '#', uploadDate: '2024-01-08', verified: true },
          { id: 16, name: 'Chứng nhận kho bãi', type: 'PDF', url: '#', uploadDate: '2024-01-08', verified: false }
        ]
      },
      {
        id: 9,
        businessName: 'Công ty CP Giáo dục EduTech',
        taxCode: '0404040404',
        contactPerson: 'Ngô Văn I',
        email: 'info@edutech.edu.vn',
        phone: '0978901234',
        address: '369 Đường Hai Bà Trưng, Phường 8, TP. Lào Cai',
        businessType: 'Công nghệ thông tin',
        registrationDate: '2024-01-07',
        status: 'pending',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 17, name: 'Giấy phép hoạt động giáo dục', type: 'PDF', url: '#', uploadDate: '2024-01-07', verified: false },
          { id: 18, name: 'Chương trình đào tạo', type: 'Word', url: '#', uploadDate: '2024-01-07', verified: true }
        ]
      },
      {
        id: 10,
        businessName: 'Công ty TNHH Y tế HealthCare Plus',
        taxCode: '0505050505',
        contactPerson: 'Trịnh Thị K',
        email: 'contact@healthcare-plus.com',
        phone: '0989012345',
        address: '741 Đường Võ Thị Sáu, Phường 9, TP. Lào Cai',
        businessType: 'Thương mại dịch vụ',
        registrationDate: '2024-01-06',
        status: 'approved',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 19, name: 'Giấy phép hoạt động y tế', type: 'PDF', url: '#', uploadDate: '2024-01-06', verified: true },
          { id: 20, name: 'Chứng nhận GMP', type: 'PDF', url: '#', uploadDate: '2024-01-06', verified: true }
        ]
      },
      {
        id: 11,
        businessName: 'Công ty CP Xây dựng Modern Build',
        taxCode: '0606060606',
        contactPerson: 'Lý Văn L',
        email: 'build@modern.construction',
        phone: '0990123456',
        address: '852 Đường Lê Lợi, Phường 10, TP. Lào Cai',
        businessType: 'Sản xuất chế biến',
        registrationDate: '2024-01-05',
        status: 'reviewing',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 21, name: 'Giấy phép xây dựng', type: 'PDF', url: '#', uploadDate: '2024-01-05', verified: true },
          { id: 22, name: 'Chứng chỉ năng lực', type: 'PDF', url: '#', uploadDate: '2024-01-05', verified: false }
        ]
      },
      {
        id: 12,
        businessName: 'Công ty TNHH Thời trang Fashion House',
        taxCode: '0707070707',
        contactPerson: 'Mai Thị M',
        email: 'fashion@house.vn',
        phone: '0901234567',
        address: '963 Đường Ngô Quyền, Phường 11, TP. Lào Cai',
        businessType: 'Thương mại dịch vụ',
        registrationDate: '2024-01-04',
        status: 'pending',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 23, name: 'Giấy phép kinh doanh', type: 'PDF', url: '#', uploadDate: '2024-01-04', verified: false },
          { id: 24, name: 'Chứng nhận chất lượng sản phẩm', type: 'PDF', url: '#', uploadDate: '2024-01-04', verified: true }
        ]
      },
      {
        id: 13,
        businessName: 'Công ty CP Năng lượng Green Energy',
        taxCode: '0808080808',
        contactPerson: 'Phan Văn N',
        email: 'green@energy.vn',
        phone: '0912345678',
        address: '159 Đường Quang Trung, Phường 12, TP. Lào Cai',
        businessType: 'Công nghệ thông tin',
        registrationDate: '2024-01-03',
        status: 'rejected',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 25, name: 'Giấy phép hoạt động năng lượng', type: 'PDF', url: '#', uploadDate: '2024-01-03', verified: false },
          { id: 26, name: 'Báo cáo tác động môi trường', type: 'PDF', url: '#', uploadDate: '2024-01-03', verified: false }
        ]
      },
      {
        id: 14,
        businessName: 'Công ty TNHH Thực phẩm Fresh Foods',
        taxCode: '0909090909',
        contactPerson: 'Quách Thị O',
        email: 'fresh@foods.com',
        phone: '0923456789',
        address: '357 Đường Trường Chinh, Phường 13, TP. Lào Cai',
        businessType: 'Sản xuất chế biến',
        registrationDate: '2024-01-02',
        status: 'approved',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 27, name: 'Giấy chứng nhận ATTP', type: 'PDF', url: '#', uploadDate: '2024-01-02', verified: true },
          { id: 28, name: 'Chứng nhận HACCP', type: 'PDF', url: '#', uploadDate: '2024-01-02', verified: true }
        ]
      },
      {
        id: 15,
        businessName: 'Công ty CP Bất động sản Dream Home',
        taxCode: '1010101010',
        contactPerson: 'Râu Văn P',
        email: 'dream@home.realestate',
        phone: '0934567890',
        address: '468 Đường Bạch Đằng, Phường 14, TP. Lào Cai',
        businessType: 'Đầu tư tài chính',
        registrationDate: '2024-01-01',
        status: 'reviewing',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 29, name: 'Giấy phép kinh doanh bất động sản', type: 'PDF', url: '#', uploadDate: '2024-01-01', verified: true },
          { id: 30, name: 'Chứng chỉ hành nghề', type: 'PDF', url: '#', uploadDate: '2024-01-01', verified: false }
        ]
      },
      {
        id: 16,
        businessName: 'Công ty TNHH Dược phẩm PharmaCare',
        taxCode: '1111111111',
        contactPerson: 'Sầm Thị Q',
        email: 'pharma@care.vn',
        phone: '0945678901',
        address: '579 Đường Lê Duẩn, Phường 15, TP. Lào Cai',
        businessType: 'Thương mại dịch vụ',
        registrationDate: '2023-12-31',
        status: 'pending',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 31, name: 'Giấy phép kinh doanh dược', type: 'PDF', url: '#', uploadDate: '2023-12-31', verified: false },
          { id: 32, name: 'Chứng chỉ dược sĩ', type: 'PDF', url: '#', uploadDate: '2023-12-31', verified: true }
        ]
      },
      {
        id: 17,
        businessName: 'Công ty CP Vận tải Lào Cai Express',
        taxCode: '1212121212',
        contactPerson: 'Tạ Văn R',
        email: 'express@laocai.transport',
        phone: '0956789012',
        address: '680 Đường Hùng Vương, Phường 16, TP. Lào Cai',
        businessType: 'Thương mại dịch vụ',
        registrationDate: '2023-12-30',
        status: 'approved',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 33, name: 'Giấy phép vận tải hành khách', type: 'PDF', url: '#', uploadDate: '2023-12-30', verified: true },
          { id: 34, name: 'Chứng nhận đăng kiểm xe', type: 'PDF', url: '#', uploadDate: '2023-12-30', verified: true }
        ]
      },
      {
        id: 18,
        businessName: 'Công ty TNHH Điện tử Smart Tech',
        taxCode: '1313131313',
        contactPerson: 'Uyên Thị S',
        email: 'smart@tech.electronics',
        phone: '0967890123',
        address: '791 Đường Pasteur, Phường 17, TP. Lào Cai',
        businessType: 'Công nghệ thông tin',
        registrationDate: '2023-12-29',
        status: 'reviewing',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 35, name: 'Giấy phép kinh doanh', type: 'PDF', url: '#', uploadDate: '2023-12-29', verified: true },
          { id: 36, name: 'Chứng nhận CE', type: 'PDF', url: '#', uploadDate: '2023-12-29', verified: false }
        ]
      },
      {
        id: 19,
        businessName: 'Công ty CP Khách sạn Mountain View',
        taxCode: '1414141414',
        contactPerson: 'Vương Văn T',
        email: 'mountain@view.hotel',
        phone: '0978901234',
        address: '802 Đường Cầu Giấy, Phường 18, TP. Lào Cai',
        businessType: 'Du lịch dịch vụ',
        registrationDate: '2023-12-28',
        status: 'pending',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 37, name: 'Giấy phép kinh doanh lưu trú', type: 'PDF', url: '#', uploadDate: '2023-12-28', verified: false },
          { id: 38, name: 'Chứng nhận phòng cháy chữa cháy', type: 'PDF', url: '#', uploadDate: '2023-12-28', verified: true }
        ]
      },
      {
        id: 20,
        businessName: 'Công ty TNHH Hóa chất Industrial Chem',
        taxCode: '1515151515',
        contactPerson: 'Xuân Thị U',
        email: 'industrial@chem.vn',
        phone: '0989012345',
        address: '913 Đường Nguyễn Thái Học, Phường 19, TP. Lào Cai',
        businessType: 'Sản xuất chế biến',
        registrationDate: '2023-12-27',
        status: 'rejected',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 39, name: 'Giấy phép sản xuất hóa chất', type: 'PDF', url: '#', uploadDate: '2023-12-27', verified: false },
          { id: 40, name: 'Báo cáo đánh giá tác động môi trường', type: 'PDF', url: '#', uploadDate: '2023-12-27', verified: false }
        ]
      },
      {
        id: 21,
        businessName: 'Công ty CP Truyền thông Digital Media',
        taxCode: '1616161616',
        contactPerson: 'Yến Văn V',
        email: 'digital@media.com',
        phone: '0990123456',
        address: '124 Đường Nguyễn Du, Phường 20, TP. Lào Cai',
        businessType: 'Công nghệ thông tin',
        registrationDate: '2023-12-26',
        status: 'approved',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 41, name: 'Giấy phép hoạt động báo chí', type: 'PDF', url: '#', uploadDate: '2023-12-26', verified: true },
          { id: 42, name: 'Chứng nhận bản quyền phần mềm', type: 'PDF', url: '#', uploadDate: '2023-12-26', verified: true }
        ]
      },
      {
        id: 22,
        businessName: 'Công ty TNHH Mỹ phẩm Beauty Care',
        taxCode: '1717171717',
        contactPerson: 'Zung Thị W',
        email: 'beauty@care.cosmetics',
        phone: '0901234567',
        address: '235 Đường Lý Tự Trọng, Phường 21, TP. Lào Cai',
        businessType: 'Thương mại dịch vụ',
        registrationDate: '2023-12-25',
        status: 'reviewing',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 43, name: 'Giấy phép kinh doanh mỹ phẩm', type: 'PDF', url: '#', uploadDate: '2023-12-25', verified: true },
          { id: 44, name: 'Chứng nhận FDA', type: 'PDF', url: '#', uploadDate: '2023-12-25', verified: false }
        ]
      },
      {
        id: 23,
        businessName: 'Công ty CP Công nghiệp Machinery Works',
        taxCode: '1818181818',
        contactPerson: 'An Văn X',
        email: 'machinery@works.industrial',
        phone: '0912345678',
        address: '346 Đường Hoàng Hoa Thám, Phường 22, TP. Lào Cai',
        businessType: 'Sản xuất chế biến',
        registrationDate: '2023-12-24',
        status: 'pending',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 45, name: 'Giấy phép sản xuất máy móc', type: 'PDF', url: '#', uploadDate: '2023-12-24', verified: false },
          { id: 46, name: 'Chứng nhận ISO 14001', type: 'PDF', url: '#', uploadDate: '2023-12-24', verified: true }
        ]
      },
      {
        id: 24,
        businessName: 'Công ty TNHH Tư vấn Legal Advisor',
        taxCode: '1919191919',
        contactPerson: 'Bình Thị Y',
        email: 'legal@advisor.law',
        phone: '0923456789',
        address: '457 Đường Nguyễn Văn Cừ, Phường 23, TP. Lào Cai',
        businessType: 'Thương mại dịch vụ',
        registrationDate: '2023-12-23',
        status: 'approved',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 47, name: 'Giấy phép hành nghề luật sư', type: 'PDF', url: '#', uploadDate: '2023-12-23', verified: true },
          { id: 48, name: 'Chứng chỉ tư vấn pháp luật', type: 'PDF', url: '#', uploadDate: '2023-12-23', verified: true }
        ]
      },
      {
        id: 25,
        businessName: 'Công ty CP Fintech Innovation',
        taxCode: '2020202020',
        contactPerson: 'Cường Văn Z',
        email: 'fintech@innovation.finance',
        phone: '0934567890',
        address: '568 Đường Đinh Tiên Hoàng, Phường 24, TP. Lào Cai',
        businessType: 'Đầu tư tài chính',
        registrationDate: '2023-12-22',
        status: 'reviewing',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 49, name: 'Giấy phép hoạt động fintech', type: 'PDF', url: '#', uploadDate: '2023-12-22', verified: true },
          { id: 50, name: 'Chứng nhận bảo mật thông tin', type: 'PDF', url: '#', uploadDate: '2023-12-22', verified: false }
        ]
      }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }

  getStatisticsDummy(): Observable<BaseResponseModel<BusinessApprovalStats>> {
    const dummyStats: BusinessApprovalStats = {
      pending: 8,    // IDs: 1, 3, 6, 9, 12, 16, 19, 23
      approved: 6,   // IDs: 4, 7, 10, 14, 17, 21, 24  
      rejected: 3,   // IDs: 5, 13, 20
      reviewing: 8   // IDs: 2, 8, 11, 15, 18, 22, 25
    };

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyStats
    });
  }
}
