import { Component, EventEmitter, Input, Output, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { EmailVerificationService, BusinessVerificationRequest, BusinessVerificationResponse, CompanyInfo } from '../../../../services/website/email-verification.service';
import { GetCurrentUserId } from '../../../../utils/commonFunctions';

export interface VerificationDocuments {
  businessLicense?: File;
  representativeId?: File;
  companyStamp?: File;
}

export interface VerificationData {
  basicInfo: any;
  documents: VerificationDocuments;
}

@Component({
  selector: 'app-email-verification',
  templateUrl: './email-verification.component.html',
  styleUrls: ['./email-verification.component.scss']
})
export class EmailVerificationComponent implements OnInit, OnChanges {
  @Input() isOpen: boolean = false;
  @Output() closeModalEvent = new EventEmitter<void>();
  @Output() verificationSubmitted = new EventEmitter<BusinessVerificationResponse>();

  currentStep: number = 1;
  basicInfoForm!: FormGroup;
  documents: VerificationDocuments = {};
  agreeToTerms: boolean = false;
  isSubmitting: boolean = false;
  verificationInProgress: boolean = false;
  companyInfo: CompanyInfo | null = null;
  isLoadingCompanyInfo: boolean = false;

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private emailVerificationService: EmailVerificationService
  ) {
    this.initForm();
  }

  ngOnInit(): void {
    
    if (this.isOpen) {
      this.initializeModal();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['isOpen'] && changes['isOpen'].currentValue && !changes['isOpen'].previousValue) {
      this.initializeModal();
    }
  }

  private initializeModal(): void {
    this.currentStep = 1;
    this.resetForm();
    this.loadCompanyInfo();
  }

  private initForm(): void {
    this.basicInfoForm = this.fb.group({
      companyName: ['', [Validators.required, Validators.minLength(2)]],
      taxNumber: ['', [Validators.required, Validators.pattern(/^\d{10,13}$/)]],
      position: ['', [Validators.required]],
      phone: ['', [Validators.required, Validators.pattern(/^[0-9]{10,11}$/)]],
      email: ['', [Validators.required, Validators.email]],
      address: ['', [Validators.required, Validators.minLength(10)]],
      website: ['', [Validators.pattern(/^https?:\/\/.+/)]],
      companySize: [''],
      description: ['']
    });
  }

  openModal(): void {
    this.isOpen = true;
    this.initializeModal();
  }

  closeModal(): void {
    if (this.verificationInProgress) {
      this.toastr.warning('Đang xử lý xác thực. Vui lòng chờ...');
      return;
    }
    
    this.isOpen = false;
    this.closeModalEvent.emit();
  }

  resetForm(): void {
    this.basicInfoForm.reset();
    this.documents = {};
    this.agreeToTerms = false;
    this.currentStep = 1;
    this.verificationInProgress = false;
  }

  nextStep(): void {
    if (this.canProceed()) {
      this.currentStep++;
    }
  }

  previousStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  canProceed(): boolean {
    switch (this.currentStep) {
      case 1:
        return this.basicInfoForm.valid;
      case 2:
        return this.hasRequiredDocuments();
      case 3:
        return this.agreeToTerms;
      default:
        return false;
    }
  }

  canSubmit(): boolean {
    return this.currentStep === 3 && 
           this.agreeToTerms && 
           this.hasRequiredDocuments() && 
           !this.isSubmitting &&
           !this.verificationInProgress;
  }

  hasRequiredDocuments(): boolean {
    return !!(this.documents.businessLicense && this.documents.representativeId);
  }

  triggerFileUpload(documentType: keyof VerificationDocuments, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    
    const inputElement = document.querySelector(`input[type="file"]`) as HTMLInputElement;
    if (inputElement) {
      inputElement.click();
    }
  }

  getFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  onFileSelected(event: any, documentType: keyof VerificationDocuments): void {
    const file = event.target.files[0];
    if (file) {
      // Validate file size (5MB)
      if (file.size > 5 * 1024 * 1024) {
        this.toastr.error('File quá lớn. Vui lòng chọn file nhỏ hơn 5MB.');
        return;
      }

      // Validate file type
      const allowedTypes = ['.pdf', '.jpg', '.jpeg', '.png'];
      const fileExtension = '.' + file.name.split('.').pop()?.toLowerCase();
      
      if (!allowedTypes.includes(fileExtension)) {
        this.toastr.error('Chỉ chấp nhận file PDF, JPG, JPEG, PNG.');
        return;
      }

      this.documents[documentType] = file;
      this.toastr.success(`Đã upload ${file.name}`);
    }
  }

  removeFile(documentType: keyof VerificationDocuments, event: Event): void {
    event.stopPropagation();
    delete this.documents[documentType];
    this.toastr.info('Đã xóa file');
  }

  async submitVerification(): Promise<void> {
    if (!this.canSubmit()) {
      return;
    }

    // Validate required documents
    if (!this.documents.businessLicense) {
      this.toastr.error('Vui lòng upload Giấy phép kinh doanh');
      return;
    }

    if (!this.documents.representativeId) {
      this.toastr.error('Vui lòng upload CMND/CCCD người đại diện');
      return;
    }

    console.log('📋 Documents to submit:', this.documents);

    this.isSubmitting = true;
    this.verificationInProgress = true;

    try {
      const userId = GetCurrentUserId();
      
      const verificationRequest: BusinessVerificationRequest = {
        companyName: this.basicInfoForm.value.companyName,
        taxNumber: this.basicInfoForm.value.taxNumber,
        position: this.basicInfoForm.value.position,
        phone: this.basicInfoForm.value.phone,
        email: this.basicInfoForm.value.email,
        address: this.basicInfoForm.value.address,
        website: this.basicInfoForm.value.website,
        companySize: this.basicInfoForm.value.companySize,
        description: this.basicInfoForm.value.description,
        documents: this.documents
      };

      console.log('🚀 Submitting verification request...');

      this.emailVerificationService.submitVerification(verificationRequest).subscribe({
        next: (response) => {
          console.log('📦 API Response:', response);
          if (response.isSuccess && response.result) {
            this.toastr.success('Đã gửi yêu cầu xác thực thành công! Mã xác thực: ' + response.result.verificationCode);
            this.verificationSubmitted.emit(response.result);
            this.closeModal();
          } else {
            this.toastr.error(response.message || 'Có lỗi xảy ra khi gửi yêu cầu xác thực');
          }
        },
        error: (error) => {
          console.error('❌ Error submitting verification:', error);
          this.toastr.error('Có lỗi xảy ra khi gửi yêu cầu xác thực. Vui lòng thử lại.');
        },
        complete: () => {
          this.isSubmitting = false;
          this.verificationInProgress = false;
        }
      });

    } catch (error) {
      console.error('❌ Error in submitVerification:', error);
      this.toastr.error('Có lỗi xảy ra. Vui lòng thử lại.');
      this.isSubmitting = false;
      this.verificationInProgress = false;
    }
  }

  getFieldError(fieldName: string): string {
    const field = this.basicInfoForm.get(fieldName);
    if (field?.invalid && field?.touched) {
      if (field.errors?.['required']) {
        return 'Trường này là bắt buộc';
      }
      if (field.errors?.['email']) {
        return 'Email không hợp lệ';
      }
      if (field.errors?.['pattern']) {
        if (fieldName === 'taxNumber') {
          return 'Mã số thuế phải có 10-13 số';
        }
        if (fieldName === 'phone') {
          return 'Số điện thoại phải có 10-11 số';
        }
        if (fieldName === 'website') {
          return 'Website phải bắt đầu bằng http:// hoặc https://';
        }
      }
      if (field.errors?.['minlength']) {
        return `Tối thiểu ${field.errors['minlength'].requiredLength} ký tự`;
      }
    }
    return '';
  }

  private loadCompanyInfo(): void {
    console.log('🔄 Loading company info...');
    const userId = GetCurrentUserId();
    if (!userId) {
      console.warn('❌ No user ID found');
      return;
    }

    console.log('👤 User ID:', userId);
    this.isLoadingCompanyInfo = true;
    this.emailVerificationService.getCompanyInfo(userId.toString()).subscribe({
      next: (response) => {
        console.log('📦 API Response:', response);
        if (response.isSuccess && response.result) {
          this.companyInfo = response.result;
          console.log('🏢 Company Info:', this.companyInfo);
          this.populateFormWithCompanyInfo();
        } else {
          console.warn('⚠️ API response not successful or no result');
        }
      },
      error: (error) => {
        console.error('❌ Error loading company info:', error);
      },
      complete: () => {
        this.isLoadingCompanyInfo = false;
        console.log('✅ Company info loading completed');
      }
    });
  }

  private populateFormWithCompanyInfo(): void {
    if (!this.companyInfo) {
      console.warn('⚠️ No company info to populate');
      return;
    }

    console.log('📝 Populating form with company info...');
    const formData = {
      companyName: this.companyInfo.companyName || '',
      taxNumber: this.companyInfo.taxNumber || '',
      position: this.companyInfo.position || '',
      phone: this.companyInfo.phoneNumber || '',
      email: this.companyInfo.email || '',
      address: this.companyInfo.address || '',
      website: this.companyInfo.website || '',
      companySize: this.companyInfo.companySize || '',
      description: this.companyInfo.description || ''
    };

    console.log('📊 Form data to patch:', formData);
    this.basicInfoForm.patchValue(formData);
    console.log('✅ Form patched successfully');

    if (this.companyInfo.companyName && this.companyInfo.companyName.trim() !== '') {
      this.toastr.info('Đã tải thông tin doanh nghiệp hiện có. Bạn có thể cập nhật thông tin bổ sung.', 'Thông báo');
      console.log('🎉 Showed success toast - data loaded');
    } else {
      this.toastr.info('Vui lòng nhập đầy đủ thông tin doanh nghiệp để xác thực', 'Thông báo');
      console.log('ℹ️ Showed info toast - no existing data');
    }
  }

  getCurrentDate(): string {
    return new Date().toLocaleDateString('vi-VN', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}
