import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register-candidate',
  templateUrl: './register-candidate.component.html',
  styleUrls: ['./register-candidate.component.scss']
})
export class RegisterCandidateComponent {

  registerData = {
    fullName: '',
    phone: '',
    email: '',
    password: '',
    confirmPassword: ''
  };

  constructor(
    private router: Router,
    private toastr: ToastrService
  ) {}

  register() {
    if (!this.registerData.fullName || !this.registerData.phone || !this.registerData.email || !this.registerData.password || !this.registerData.confirmPassword) {
      this.toastr.error('Vui lòng điền đầy đủ thông tin bắt buộc');
      return;
    }

    if (this.registerData.password !== this.registerData.confirmPassword) {
      this.toastr.error('Mật khẩu nhập lại không khớp');
      return;
    }

    // TODO: Call API to register candidate
    console.log('Register candidate:', this.registerData);
    this.toastr.success('Đăng ký thành công! Vui lòng kiểm tra email để xác thực tài khoản.');
    this.router.navigate(['/auth']);
  }

  googleLogin() {
    // TODO: Implement Google OAuth login
    console.log('Google login clicked');
    this.toastr.info('Tính năng đăng nhập Google đang được phát triển');
  }
}
