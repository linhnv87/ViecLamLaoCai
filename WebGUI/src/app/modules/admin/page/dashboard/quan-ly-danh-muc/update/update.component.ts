import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FieldModel } from 'src/app/models/field.model';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import * as common from 'src/app/utils/commonFunctions';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.css']
})
export class UpdateComponent implements OnInit, OnDestroy {
  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();
  itemId: string | undefined | null;
  isModalOpen = false;
  specialistAction = true;
  approverAction = false;
  generalAction = false;
  waitForResponse = false;
  isAuthor = false;
  document: FieldModel | undefined | null;
  dataField: FieldModel = {
    id: 0,
    title: '',
    active: true,
    modified: new Date(),
    deleted: false,
    created: new Date(),
    createdBy: this.userId,
    modifiedBy: this.userId
  };
  subscription: Subscription = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fieldService: FieldServiceService,
    private toastr: ToastrService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.subscription.add(
      this.route.params.subscribe((params) => {
        this.itemId = params['id'];
        console.log('ID của phần tử cần sửa:', this.itemId);
        // this.getDocumentData(this.itemId);
        this.checkUserPermission();
      })
    );
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  openModal() {
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  update() {
    const checkForm = this.dataField.title?.trim() === '';
    if (!checkForm) {
      this.waitForResponse = true;
      this.subscription.add(
        this.fieldService.update(this.dataField).subscribe(
          (res) => {
            this.waitForResponse = false;
            if (res.isSuccess) {
              this.toastr.success('Sửa danh mục thành công');
              this.isModalOpen = false;
            } else {
              this.toastr.error('Sửa danh mục thất bại');
            }
          },
          (error) => {
            this.waitForResponse = false;
            this.toastr.error('Đã xảy ra lỗi khi sửa danh mục');
          }
        )
      );
    } else {
      this.toastr.warning('Cần nhập đầy đủ thông tin');
    }
  }
  // getDocumentById(id: number) {
    
  //   const dataPromise = this.fieldService.getById(id).subscribe(res => {
  //     if (res.isSuccess) {        
  //       this.document = res.result as FieldModel;
          
  //     }
  //   });
  // }


  // getDocumentData(id: string | undefined | null) {
  //   this.subscription.add(
  //     this.fieldService.getDocumentById(id).subscribe(
  //       (res) => {
  //         this.document = res;
  //         // Thực hiện các thao tác khác liên quan đến dữ liệu tài liệu (nếu cần)
  //       },
  //       (error) => {
  //         console.log('Đã xảy ra lỗi khi lấy dữ liệu tài liệu');
  //       }
  //     )
  //   );
  // }

  checkUserPermission() {
    // Thực hiện kiểm tra quyền của người dùng và cập nhật các biến isAuthor, specialistAction, approverAction, generalAction (nếu cần)
  }
}