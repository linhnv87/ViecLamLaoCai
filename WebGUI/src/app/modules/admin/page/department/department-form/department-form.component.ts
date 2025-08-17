import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DepartmentModel } from 'src/app/models/department.model';
import { DocumentTypeModel } from 'src/app/models/document-type.model';
import { FieldModel } from 'src/app/models/field.model';
import { DepartmentService } from 'src/app/services/admin/department.service';
import { DocumentTypeService } from 'src/app/services/admin/document-type.service';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import * as common from 'src/app/utils/commonFunctions';


@Component({
  selector: 'app-department-form',
  templateUrl: './department-form.component.html',
  styleUrls: ['./department-form.component.scss']
})
export class DepartmentFormComponent implements OnInit {

  userId = common.GetCurrentUserId();

  dataDepartment: DepartmentModel = {
    id: 0,
    departmentName: '',
    isActive: true,
    modified: new Date(),
    deleted: false,
    created: new Date(),
    createdBy: this.userId,
    modifiedBy: this.userId
  };

  id: number | undefined;

  constructor(private departmentSerivce: DepartmentService, private toastr: ToastrService, private router: Router, private route: ActivatedRoute) { }
  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      this.id = id;
      this.getDataById();
    });
  }

  getDataById() {
    if (this.id) {
      this.departmentSerivce.getById(this.id).subscribe(res => {
        console.log(res);

        if (res.isSuccess) {
          this.dataDepartment = res.result as DepartmentModel;
        }
      })
    }
  }

  submit(sendToApprove: boolean) {
    var checkForm = this.dataDepartment.departmentName?.trim() == '';

    if (!checkForm) {
      if (this.id) {
        this.departmentSerivce.update(this.dataDepartment).subscribe(res => {
          if (res.isSuccess) {
            this.toastr.success('Chỉnh sửa thông tin danh mục thành công');
            this.router.navigate(['/admin/department'])
          } else {
            this.toastr.error(res.message)
          }
        })
      }
      else {
        const addDataPromise = this.departmentSerivce.create(this.dataDepartment).subscribe((res) => {
          if (res.isSuccess) {

            this.toastr.success('Thêm danh mục thành công');
            this.router.navigate(['/admin/department'])
          } else {
            this.toastr.error(res.message)
          }
        });
      }
    } else {
      this.toastr.warning('Cần nhập đầy đủ thông tin')
    }
  }
}
