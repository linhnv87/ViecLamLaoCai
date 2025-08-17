import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { ChangePasswordModel } from 'src/app/models/changePassword.model';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import { UserService } from 'src/app/services/user.service';
import { GetCurrentUserId } from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {
  /**
   *
   */

  changePassword: ChangePasswordModel = {
    userId: '00000000-0000-0000-0000-000000000000',
    oldPassword: '',
    newPassword: ''
  }

  repeatNewPassword: string = '';

  constructor(private userService: UserService,
    private modalService: NgbModal,
    route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService) {


  }
  ngOnInit(): void {
    // this.changePassword.userId = GetCurrentUserId();
    // console.log(this.changePassword);
  }



  onChangePassword() {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.result.then(result => {
      if(result){
        this.changePassword.userId = GetCurrentUserId();
        if (this.changePassword.userId != null) {
          // So sánh new password với repeat new password 
          if (this.changePassword.newPassword == this.repeatNewPassword) {
            // Gọi api Lấy dữ liệu password đưa xuống be va update
            var dataPromise = this.userService.ChangePassword(this.changePassword).subscribe(res => {
              if (res.isSuccess) {
                this.toastr.info('Đổi mật khẩu thành công!');
                this.router.navigate(['/auth']);
                console.log(this.changePassword);
              }
              else{
                this.toastr.warning('Mật khẩu cũ không đúng')
              }
            });
          }
          else {
            this.toastr.warning('Mật khẩu mới không trùng khớp');
            console.log(this.changePassword);
          }
        }
      }
    })
  }
}
