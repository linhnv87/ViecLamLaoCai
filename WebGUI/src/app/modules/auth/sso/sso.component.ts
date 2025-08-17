import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TokenResponseModel } from 'src/app/models/user.model';
import { UserService } from 'src/app/services/user.service';
import { LOCAL_STORAGE_KEYS } from 'src/app/utils/constants';

@Component({
  selector: 'app-sso',
  templateUrl: './sso.component.html',
  styleUrls: ['./sso.component.css'],
})
export class SsoComponent implements OnInit {
  returnUrl = '/admin/Dashboard';

  constructor(
    private route: ActivatedRoute,
    private authService: UserService,
    private router: Router,
    private toastr: ToastrService,
  ) {}
  ngOnInit(): void {
    if (location.href.includes('gogosso')) {
      this.ssoLogin();
    } else {
      this.startProcess();
    }
  }

  ssoLogin(): void {
    const z = this.authService.getSSOUrl().subscribe(res => {
      if (res.isSuccess && res.result.authority != undefined) {
        window.open(res.result.auth_url, '_self');
      }
    });
  }

  startProcess(): void {
    this.route.queryParams.subscribe(params => {
      console.log(params);

      var code = params['code'];
      var sessionState = params['session_state'];
      try {
        const ssoSignIn = this.authService.ssoCallbackSignin(code).subscribe(response => {
          console.log(response);

          if (response.isSuccess) {
            this.setUpToken(response.result);
            this.router.navigateByUrl(this.returnUrl); // Main page
          } else {
            this.toastr.error('Đăng nhập SSO không thành công: ' + response.message);
            this.router.navigateByUrl('/auth');
          }
        });
      } catch {
        this.toastr.error('Đăng nhập SSO không thành công');
        this.router.navigateByUrl('/auth');
      }
    });
  }

  setUpToken(responseToken: TokenResponseModel) {
    localStorage.setItem(LOCAL_STORAGE_KEYS.USER_INFO, JSON.stringify(responseToken));
  }
}
