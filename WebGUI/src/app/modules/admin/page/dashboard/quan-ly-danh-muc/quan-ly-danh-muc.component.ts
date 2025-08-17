import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FieldModel } from 'src/app/models/field.model';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import * as common from 'src/app/utils/commonFunctions';
import { StatusCode } from 'src/app/models/enums/statusCode.enum';
import { DocumentService } from 'src/app/services/admin/document.service';

@Component({
  selector: 'app-quan-ly-danh-muc',
  templateUrl: './quan-ly-danh-muc.component.html',
  styleUrls: ['./quan-ly-danh-muc.component.css']
})
export class QuanLyDanhMucComponent {
  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();
  itemId: string | undefined | null;
  isModalOpen: boolean = false;
  specialistAction = true;
  approverAction = false;
  generalAction = false;
  waitForResponse = false;

  files: File[] = [];
  statusCode?: number;
  openModal() {
    this.isModalOpen = true;
  }
  closeModal() {
    this.isModalOpen = false;
  }

  fieldData: FieldModel[] = [];
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
  constructor(private route: ActivatedRoute, private router: Router, private fieldService: FieldServiceService,
    private toastr: ToastrService,
    private fb: FormBuilder,
  ) {
    this.loadtenfield();
  }
  loadtenfield() {
    const getAllPromise = this.fieldService.getAll().subscribe((res) => {
      if (res.isSuccess) {
        this.fieldData = res.result as FieldModel[];
      } else {
        this.toastr.error(res.message);
      }
    });
  }
  deletefield(item: number) {
    if ({ item }) {
      const deletePromise = this.fieldService.delete(item).subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Xóa tờ trình thành công')
          this.loadtenfield();
        } else {
          this.toastr.error(res.message);
        }
      })

    }
  }

  submit(sendToApprove: boolean) {
    var checkForm = this.dataField.title?.trim() == '';

    if (!checkForm) {
      if (sendToApprove) {
        this.statusCode = StatusCode.Pending;
      }
      this.waitForResponse = true;
      const addDataPromise = this.fieldService.create(this.dataField).subscribe((res) => {
        if (res.isSuccess) {
          this.waitForResponse = false;
          this.toastr.success('Thêm danh mục thành công');
          this.loadtenfield();
          this.isModalOpen = false;
        } else {
          this.waitForResponse = false;
        }
      });
    } else {
      this.toastr.warning('Cần nhập đầy đủ thông tin')
    }
  }


  submit1() {
    
    var checkForm = this.dataField.title?.trim() == '';
    if (!checkForm) {
      if (this.itemId) {
        // this.data.active = JSON.parse(this.data.active);
        const addDataPromise = this.fieldService.update(this.dataField).subscribe(response => {
          if(response.isSuccess) {
            this.toastr.success('Sửa danh mục thành công');
            this.router.navigate(['/admin/quan-ly-danh-muc'])
          }        
        })
      }else{
        // this.data.active = JSON.parse(this.data.active);
        const addDataPromise = this.fieldService.create(this.dataField).subscribe(response => {
          if(response.isSuccess){
            this.toastr.success('Thêm danh mục thành công');
            this.router.navigate(['/admin/quan-ly-danh-muc'])
          }
          else {
            console.log("Not response !!");
          }
        });
      }
    }
    else {
      this.toastr.warning('Cần nhập đầy đủ thông tin')
    }
    
    
  }


  itemsPerPage: number = 10;
  currentPage: number = 1;

}
