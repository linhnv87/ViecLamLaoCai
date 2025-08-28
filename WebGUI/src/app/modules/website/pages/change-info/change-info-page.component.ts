import { Component, OnInit } from '@angular/core';

export interface UserInfo {
  fullName: string;
  email: string;
  phone: string;
  address: string;
  avatar: string;
}

@Component({
  selector: 'app-change-info-page',
  templateUrl: './change-info-page.component.html',
  styleUrls: ['./change-info-page.component.scss']
})
export class ChangeInfoPageComponent implements OnInit {
  isEditingProfile: boolean = false;
  currentUserInfo: UserInfo = {
    fullName: 'Nguyễn Văn A',
    email: 'nguyenvana@email.com',
    phone: '0123456789',
    address: '123 Đường ABC, Quận 1, TP. Hồ Chí Minh',
    avatar: 'assets/vieclamlaocai/img/image 16.png'
  };

  // Form data
  formData: UserInfo = {
    fullName: '',
    email: '',
    phone: '',
    address: '',
    avatar: ''
  };

  constructor() {}

  ngOnInit(): void {
    this.initializeFormData();
  }

  initializeFormData(): void {
    this.formData = { ...this.currentUserInfo };
  }

  toggleProfileEdit(): void {
    this.isEditingProfile = !this.isEditingProfile;
  }

  saveProfileChanges(userInfo: UserInfo): void {
    this.currentUserInfo = { ...userInfo };
    this.isEditingProfile = false;
    // Ở đây có thể gọi API để lưu thông tin
    console.log('Đã lưu thông tin:', userInfo);
  }

  cancelProfileEdit(): void {
    this.isEditingProfile = false;
    this.formData = { ...this.currentUserInfo };
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.formData.avatar = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }
}
