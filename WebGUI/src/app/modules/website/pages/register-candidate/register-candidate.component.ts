import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService, CandidateRegisterRequest } from '../../../../services/website/auth.service';

@Component({
  selector: 'app-register-candidate',
  templateUrl: './register-candidate.component.html',
  styleUrls: ['./register-candidate.component.scss']
})
export class RegisterCandidateComponent {

  registerData: CandidateRegisterRequest = {
    fullName: '',
    phone: '',
    email: '',
    password: '',
    confirmPassword: '',
    dateOfBirth: undefined,
    gender: '',
    address: '',
    districtId: undefined,
    communeId: undefined,
    educationLevelId: undefined,
    careerId: undefined
  };

  isSubmitting = false;
  showPassword = false;
  showConfirmPassword = false;

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

    this.authService.registerCandidate(this.registerData).subscribe({
      next: (response) => {
        this.isSubmitting = false;
        if (response.isSuccess) {
          this.toastr.success(response.result.message);
          if (response.result.requiresEmailVerification) {
            this.toastr.info('Vui lòng kiểm tra email để xác thực tài khoản.');
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

  googleLogin() {
    // TODO: Implement Google OAuth login
    console.log('Google login clicked');
    this.toastr.info('Tính năng đăng nhập Google đang được phát triển');
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  private validateInput(): boolean {
    if (!this.registerData.fullName || !this.registerData.phone || !this.registerData.email || 
        !this.registerData.password || !this.registerData.confirmPassword) {
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
