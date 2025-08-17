import {Component, OnInit} from '@angular/core';
import * as common from "../../../../utils/commonFunctions";
import {DocumentApprovalModel} from "../../../../models/documentApproval.model";
import {ActivatedRoute, Router} from "@angular/router";
import {DocumentService} from "../../../../services/admin/document.service";
import {FieldServiceService} from "../../../../services/admin/field-service.service";
import {ToastrService} from "ngx-toastr";
import {FormBuilder} from "@angular/forms";
import {DocumentApprovalService} from "../../../../services/admin/document-approval.service";
import {NotificationService} from "../../../../services/nofitication.service";

import {CreateAccount, GetAllInfoUserModel, UserSignUpModel} from "../../../../models/user.model";
import {Observable} from "rxjs";
import {GetAllRoleModel} from "../../../../models/role.model";
import {RoleService} from "../../../../services/role.service";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit{

  UserInfoAll : GetAllInfoUserModel[] = [];
  searchText: string = ''; 
  filteredUsers: GetAllInfoUserModel[] = [];
  constructor(private UserService : UserService,private  RoleService : RoleService,private toastr: ToastrService, private modalService: NgbModal) {

  }
  userId = common.GetCurrentUserId();  

  ngOnInit(): void {
     this.getAllUser()   
    
  }
  filterUsers() {
    this.filteredUsers = this.UserInfoAll.filter(user =>
      user.userName.toLowerCase().includes(this.searchText.toLowerCase()) ||
      user.userFullName.toLowerCase().includes(this.searchText.toLowerCase()) ||
      user.email.toLowerCase().includes(this.searchText.toLowerCase()) ||
      user.phoneNumber.includes(this.searchText)
    );
  }
  getAllUser (){
    const dataPromise = this.UserService.GetAllUserInfo().subscribe(res =>{
      if(res.isSuccess){
        this.UserInfoAll = res.result as GetAllInfoUserModel[];
        this.filteredUsers = this.UserInfoAll; 
      }
    })
  }
  
  resetPassword(user: GetAllInfoUserModel){
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = 'Mật khẩu của tài khoản này sẽ được tự động đổi thành Tinhuy@123456';
    modalRef.result.then(result => {
      if(result){
        this.UserService.ResetPassword(user.userId).subscribe(res => {
          if(res.isSuccess){
            this.toastr.success(`Mật khẩu của tài khoản ${user.userName} - [${user.userFullName}] được thay đổi thành`)
          }
        })
      }
    })
  }

  lockUser(user: GetAllInfoUserModel){
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = user.isLockedout ? 'Tài khoản này sẽ được mở khóa' : 'Tài khoản này sẽ bị khóa và không thể đăng nhập';
    modalRef.result.then(result => {
      if(result){
        this.UserService.LockUser(user.userId).subscribe(res => {
          if(res.isSuccess){
            this.toastr.success(`Thao tác thành công`)
            this.getAllUser()
          }
        })
      }
    })
  }

  deleteUser(user: GetAllInfoUserModel){
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = 'Xác nhận xóa tài khoản này, toàn bộ dữ liệu liên quan sẽ bị mất. Thao tác này là không thể thu hồi';
    modalRef.result.then(result => {
      if(result){
        this.UserService.DeleteUser(user.userId).subscribe(res => {
          if(res.isSuccess){
            this.toastr.success(`Thao tác thành công`)
            this.getAllUser()
          }
        })
      }
    })
  }
  

  isModalOpen: boolean = false;
  isModallOpen: boolean = false;
  openModal() {
    this.isModalOpen = true;
  }
  closeModal() {
    this.isModalOpen = false;
  }

  actionOnItem = '';
  toggleActions(id: string) {
    if (this.actionOnItem != id) {
      this.actionOnItem = id;
    } else {
      this.actionOnItem = '';
    }
  }
    
  waitForResponse = false;
  itemsPerPage: number = 10;
  currentPage: number = 1;
}

