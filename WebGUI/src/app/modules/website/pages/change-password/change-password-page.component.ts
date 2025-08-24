import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-change-password-page',
  templateUrl: './change-password-page.component.html',
  styleUrls: ['./change-password-page.component.scss']
})
export class ChangePasswordPageComponent implements OnInit {
  isChangingPassword: boolean = false;
  
  passwordData = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  };

  constructor() {}

  ngOnInit(): void {}

  togglePasswordChange(): void {
    this.isChangingPassword = !this.isChangingPassword;
  }

  savePasswordChanges(): void {
    // Validate passwords
    if (this.passwordData.newPassword !== this.passwordData.confirmPassword) {
      alert('Mật khẩu mới và xác nhận mật khẩu không khớp!');
      return;
    }

    if (this.passwordData.newPassword.length < 6) {
      alert('Mật khẩu mới phải có ít nhất 6 ký tự!');
      return;
    }
    console.log('Đổi mật khẩu:', this.passwordData);
    this.passwordData = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    };
    this.isChangingPassword = false;
    alert('Đổi mật khẩu thành công!');
  }

  cancelPasswordChange(): void {
    this.isChangingPassword = false;
    this.passwordData = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    };
  }
}
