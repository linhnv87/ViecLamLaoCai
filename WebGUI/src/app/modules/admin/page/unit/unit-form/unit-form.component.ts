import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DepartmentModel } from 'src/app/models/department.model';
import { DocumentTypeModel } from 'src/app/models/document-type.model';
import { FieldModel } from 'src/app/models/field.model';
import { UnitModel } from 'src/app/models/unit.model';
import { DepartmentService } from 'src/app/services/admin/department.service';
import { DocumentTypeService } from 'src/app/services/admin/document-type.service';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import { UnitService } from 'src/app/services/admin/unit.service';
import * as common from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-unit-form',
  templateUrl: './unit-form.component.html',
  styleUrls: ['./unit-form.component.scss']
})

export class UnitFormComponent implements OnInit {

  userId = common.GetCurrentUserId();

  dataUnit: UnitModel = {
    id: 0,
    name: '',
    isActive: true,
    modified: new Date(),
    deleted: false,
    created: new Date(),
    createdBy: this.userId,
    modifiedBy: this.userId
  };

  id: number | undefined;

  constructor(private unitService: UnitService, private toastr: ToastrService, private router: Router, private route: ActivatedRoute) { }
  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      this.id = id;
      this.getDataById();
    });
  }

  getDataById() {
    if (this.id) {
      this.unitService.getById(this.id).subscribe(res => {
        console.log(res);

        if (res.isSuccess) {
          this.dataUnit = res.result as UnitModel;
        }
      })
    }
  }

  submit(sendToApprove: boolean) {
    var checkForm = this.dataUnit.name?.trim() == '';

    if (!checkForm) {
      if (this.id) {
        this.unitService.update(this.dataUnit).subscribe(res => {
          if (res.isSuccess) {
            this.toastr.success('Chỉnh sửa thông tin đơn vị thành công');
            this.router.navigate(['/admin/unit'])
          } else {
            this.toastr.error(res.message)
          }
        })
      }
      else {
        const addDataPromise = this.unitService.create(this.dataUnit).subscribe((res) => {
          if (res.isSuccess) {

            this.toastr.success('Thêm dơn vị thành công');
            this.router.navigate(['/admin/unit'])
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
