import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TokenResponseModel, UserLogInModel, UserSignUpModel } from 'src/app/models/user.model';
import { NotificationService } from 'src/app/services/nofitication.service';
import { UserService } from 'src/app/services/user.service';
import { SplashScreenService } from 'src/app/services/splash-screen.service';
import { LOCAL_STORAGE_KEYS } from 'src/app/utils/constants';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss'],
})
export class AuthComponent implements OnInit {
  logInData: UserLogInModel = {
    userName: '',
    password: '',
  };
  
  isLoggingIn = false;

  constructor(
    private userService: UserService,
    private router: Router,
    private toastr: ToastrService,
    private notificationService: NotificationService,
    private splashScreenService: SplashScreenService,
  ) {}

  ngOnInit() {
    localStorage.setItem('pageReloaded', 'false');
  }

  login() {
    if (!this.logInData.userName || !this.logInData.password) {
      this.toastr.error('Vui lòng nhập đầy đủ email và mật khẩu', 'Lỗi đăng nhập');
      return;
    }

    // Start loading state
    this.isLoggingIn = true;
    
    // Show splash screen only for login process
    this.splashScreenService.show({
      type: 'loading',
      title: 'Đang đăng nhập...',
      message: 'Vui lòng chờ trong giây lát'
    });

    this.userService.LogIn(this.logInData).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.toastr.success('Đăng nhập thành công!', 'Thành công');
          this.setUpToken(res.result);
          
          // Navigate immediately after successful login
          this.isLoggingIn = false;
          this.splashScreenService.hide();
          this.router.navigate(['/website/dashboard']);
        } else {
          this.splashScreenService.hide();
          this.isLoggingIn = false;
          this.toastr.error(res.message || 'Đăng nhập thất bại', 'Lỗi');
        }
      },
      error: (error) => {
        console.error('Login error:', error);
        this.splashScreenService.hide();
        this.isLoggingIn = false;
        this.toastr.error('Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại!', 'Lỗi hệ thống');
      }
    });
  }



  setUpToken(responseToken: TokenResponseModel) {
    localStorage.setItem(LOCAL_STORAGE_KEYS.USER_INFO, JSON.stringify(responseToken));
  }

  ssoLogin(): void {
    // Show splash screen for SSO process
    this.splashScreenService.show({
      type: 'loading',
      title: 'Đang kết nối SSO...',
      message: 'Vui lòng chờ trong giây lát'
    });
    
    this.userService.getSSOUrl().subscribe({
      next: (res) => {
        if (res.isSuccess && res.result.authority != undefined) {
          // Redirect immediately
          this.splashScreenService.hide();
          window.open(res.result.auth_url, '_self');
        } else {
          this.splashScreenService.hide();
          this.toastr.error('Không thể kết nối đến hệ thống SSO', 'Lỗi');
        }
      },
      error: (error) => {
        console.error('SSO error:', error);
        this.splashScreenService.hide();
        this.toastr.error('Có lỗi xảy ra khi kết nối SSO', 'Lỗi hệ thống');
      }
    });
  }


}
