import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserService } from 'src/app/services/user.service';
import { ModalNotificationComponent } from '../modal-notification/modal-notification.component';
import { NotificationService } from 'src/app/services/nofitication.service';
import { interval } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin-header',
  templateUrl: './admin-header.component.html',
  styleUrls: ['./admin-header.component.css'],
})
export class AdminHeaderComponent implements OnInit {
  @Input() title: string = '';
  constructor(
    private userService: UserService,
    private router: Router,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private toastr: ToastrService,
  ) {}

  userInfo: any;

  haveNewAlert = false;

  ngOnInit(): void {
    this.loadUserInfo();
    this.checkNotificationAlert();
    const source = interval(15000); // Interval in milliseconds (15 seconds = 15000 milliseconds)

    source.pipe().subscribe(() => {
      this.checkNotificationAlert(); // Replace `yourFunction` with the function you want to execute
    });
  }

  loadUserInfo() {
    const userInfoRes = this.userService.GetLocalStorageUserInfo();
    if (userInfoRes != null) {
      this.userInfo = JSON.parse(userInfoRes);
    }
  }

  checkNotificationAlert() {
    var alertCount = localStorage.getItem('notificationCount');
    if (alertCount == null) {
      // this.haveNewAlert = true;
      this.notificationService.CheckNewAlert(this.userInfo.userId).subscribe(res => {
        if (res.isSuccess) {
          this.haveNewAlert = true;
          localStorage.setItem('notificationCount', res.result.toString());
          // this.toastr.info("Bạn có thông báo mới")
        }
      });
    } else {
      var alertCountNumber = parseInt(alertCount);
      this.notificationService.CheckNewAlert(this.userInfo.userId).subscribe(res => {
        if (res.isSuccess) {
          if (res.result > alertCountNumber) {
            this.haveNewAlert = true;

            localStorage.setItem('notificationCount', res.result.toString());
            this.toastr.info('Bạn có thông báo mới');
          } else {
            this.haveNewAlert = false;
          }
        }
      });
    }
  }

  logOut() {
    if (confirm('Xác nhận đăng xuất')) {
      this.userService.LogOut();
      this.router.navigate(['/auth']);
    }
  }

  openNotification() {
    this.haveNewAlert = false;
    this.modalService.open(ModalNotificationComponent, { size: 'xl' });
  }

  showMenu = false;
  showAction() {
    this.showMenu = !this.showMenu;
  }

  goToUrl() {
    window.open(
      'https://login.yenbai.gov.vn/oauth2/authorize?response_type=code&client_id=1HM2W5lILPLQ4qy45NckEkLr6Oca&redirect_uri=https://hop.yenbai.gov.vn/callback&scope=openid',
      '_blank',
    );
  }
}
``;
