import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TokenResponseModel, UserLogInModel, UserSignUpModel } from 'src/app/models/user.model';
import { NotificationService } from 'src/app/services/nofitication.service';
import { UserService } from 'src/app/services/user.service';
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

  constructor(
    private userService: UserService,
    private router: Router,
    private toastr: ToastrService,
    private notificationService: NotificationService,
  ) {}

  ngOnInit() {
    localStorage.setItem('pageReloaded', 'false');
  }

  login() {
    this.userService.LogIn(this.logInData).subscribe(res => {
      if (res.isSuccess) {
        this.toastr.success('Đăng nhập thành công');
        this.setUpToken(res.result);
        this.router.navigate(['/admin']);
      } else {
        this.toastr.error(res.message);
      }
    });
  }

  setUpToken(responseToken: TokenResponseModel) {
    localStorage.setItem(LOCAL_STORAGE_KEYS.USER_INFO, JSON.stringify(responseToken));
  }

  ssoLogin(): void {
    this.userService.getSSOUrl().subscribe(res => {
      if (res.isSuccess && res.result.authority != undefined) {
        window.open(res.result.auth_url, '_self');
      }
    });
    // var url = `https://login.yenbai.gov.vn/oauth2/authorize?&response_type=code&scope=openid&client_id=haevpAMd2LJE4jpvLmZ2Vqz5CUEa&redirect_uri=http%3A%2F%2Flocalhost%3A4200%2Fsignin-oidc`
    // window.open(url);
  }
}
