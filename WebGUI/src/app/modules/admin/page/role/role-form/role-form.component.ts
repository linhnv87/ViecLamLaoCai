import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { GetAllRoleModel } from 'src/app/models/role.model';
import { RoleService } from 'src/app/services/role.service';
import * as common from 'src/app/utils/commonFunctions';
import { StringHelperService } from 'src/app/utils/string-helper.service';

@Component({
  selector: 'app-role-form',
  templateUrl: './role-form.component.html',
  styleUrls: ['./role-form.component.scss']
})
export class RoleFormComponent  implements OnInit  {
  
  userId = common.GetCurrentUserId();
  dataRole: GetAllRoleModel = {
    roleId: "",
    roleName: '',
    active: true,
    deleted: false,
    description: '',
  };

  id: string | undefined;

  constructor(private roleService: RoleService,private stringHelperService:StringHelperService, private toastr: ToastrService, private router: Router, private route: ActivatedRoute) { }
  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      this.id = id;
      this.getDataById();
    });
  }

  getDataById() {
    if (this.id) {
      this.roleService.getById(this.id).subscribe(res => {
        console.log(res);

        if (res.isSuccess) {
          this.dataRole = res.result as GetAllRoleModel;
        }
      })
    }
  }
  onClassNameChange(e: any) {
    this.dataRole.roleName = this.stringHelperService.convertToSlug(this.dataRole.description);
 }
  submit(sendToApprove: boolean) {
    var checkForm = this.dataRole.description?.trim() == '';
   
    if (!checkForm) {
      if (this.id) {
        this.roleService.update(this.dataRole).subscribe(res => {
          if (res.isSuccess) {
            this.toastr.success('Chỉnh sửa thông tin danh mục thành công');
            this.router.navigate(['/admin/role'])
          } else {
            this.toastr.error(res.message)
          }
        })
      }
      else {
        this.dataRole.roleId = crypto.randomUUID();
        const addDataPromise = this.roleService.create(this.dataRole).subscribe((res) => {
          if (res.isSuccess) {

            this.toastr.success('Thêm danh mục thành công');
            this.router.navigate(['/admin/role'])
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
