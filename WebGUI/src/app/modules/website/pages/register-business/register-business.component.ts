import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService, BusinessRegisterRequest } from '../../../../services/website/auth.service';

@Component({
  selector: 'app-register-business',
  templateUrl: './register-business.component.html',
  styleUrls: ['./register-business.component.scss']
})
export class RegisterBusinessComponent {

  registerData: BusinessRegisterRequest = {
    email: '',
    password: '',
    confirmPassword: '',
    representativeName: '',
    phone: '',
    companyName: '',
    address: '',
    industry: '',
    companySize: '',
    website: '',
    description: ''
  };

  isSubmitting = false;

  constructor(
    private router: Router,
    private toastr: ToastrService,
    private authService: AuthService
  ) {}

  register() {
    if (!this.validateInput()) {
      return;
    }

    this.isSubmitting = true;

    this.authService.registerBusiness(this.registerData).subscribe({
      next: (response) => {
        this.isSubmitting = false;
        if (response.isSuccess) {
          this.toastr.success(response.result.message);
          if (response.result.requiresApproval) {
            this.toastr.info('Tài khoản của bạn đang chờ phê duyệt. Chúng tôi sẽ thông báo qua email khi tài khoản được kích hoạt.');
          }
          this.router.navigate(['/auth']);
        } else {
          this.toastr.error(response.message || 'Đăng ký không thành công');
        }
      },
      error: (error) => {
        this.isSubmitting = false;
        console.error('Registration error:', error);
        this.toastr.error('Có lỗi xảy ra trong quá trình đăng ký. Vui lòng thử lại.');
      }
    });
  }

  private validateInput(): boolean {
    if (!this.registerData.email || !this.registerData.password || !this.registerData.confirmPassword || 
        !this.registerData.representativeName || !this.registerData.phone || !this.registerData.companyName || 
        !this.registerData.address) {
      this.toastr.error('Vui lòng điền đầy đủ thông tin bắt buộc');
      return false;
    }

    if (this.registerData.password !== this.registerData.confirmPassword) {
      this.toastr.error('Mật khẩu nhập lại không khớp');
      return false;
    }

    // Email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.registerData.email)) {
      this.toastr.error('Email không hợp lệ');
      return false;
    }

    // Phone validation
    const phoneRegex = /^[0-9]{10,11}$/;
    if (!phoneRegex.test(this.registerData.phone)) {
      this.toastr.error('Số điện thoại không hợp lệ (10-11 số)');
      return false;
    }

    // Password strength validation
    if (this.registerData.password.length < 6) {
      this.toastr.error('Mật khẩu phải có ít nhất 6 ký tự');
      return false;
    }

    return true;
  }
}
