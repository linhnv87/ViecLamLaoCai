import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register-business',
  templateUrl: './register-business.component.html',
  styleUrls: ['./register-business.component.scss']
})
export class RegisterBusinessComponent {

  registerData = {
    email: '',
    password: '',
    confirmPassword: '',
    representativeName: '',
    phone: '',
    companyName: '',
    address: ''
  };

  constructor(
    private router: Router,
    private toastr: ToastrService
  ) {}

  register() {
    if (!this.registerData.email || !this.registerData.password || !this.registerData.confirmPassword) {
      this.toastr.error('Vui lòng điền đầy đủ thông tin bắt buộc');
      return;
    }

    if (this.registerData.password !== this.registerData.confirmPassword) {
      this.toastr.error('Mật khẩu nhập lại không khớp');
      return;
    }

    // TODO: Call API to register business
    console.log('Register business:', this.registerData);
    this.toastr.success('Đăng ký thành công! Vui lòng kiểm tra email để xác thực tài khoản.');
    this.router.navigate(['/auth']);
  }
}
