import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DocumentTypeModel } from 'src/app/models/document-type.model';
import { FieldModel } from 'src/app/models/field.model';
import { DocumentTypeService } from 'src/app/services/admin/document-type.service';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import * as common from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-document-type-form',
  templateUrl: './document-type-form.component.html',
  styleUrls: ['./document-type-form.component.scss']
})
export class DocumentTypeFormComponent  implements OnInit {

  userId = common.GetCurrentUserId();

  dataType: DocumentTypeModel = {
    id: 0,
    name: '',
    description: '',
    // modified: new Date(),
    // deleted: false,
    // created: new Date(),
    // createdBy: this.userId,
    // modifiedBy: this.userId
  };

  id: number | undefined;  

  constructor(private typeService: DocumentTypeService, private toastr: ToastrService, private router: Router, private route: ActivatedRoute){}
  ngOnInit(): void {
      this.route.params.subscribe(params => {      
      const id = params['id'];      
      this.id = id;
      this.getDataById();
    });  
  }

  getDataById(){
    if(this.id){
      this.typeService.getById(this.id).subscribe(res => {
        console.log(res);
        
        if(res.isSuccess){
          this.dataType = res.result as DocumentTypeModel;
        }
      })
    }
  }

  submit(sendToApprove: boolean) {
    var checkForm = this.dataType.name?.trim() == '';

    if (!checkForm) {         
      if(this.id){
        this.typeService.update(this.dataType).subscribe(res => {
          if (res.isSuccess) {            
            this.toastr.success('Chỉnh sửa thông tin danh mục thành công');
            this.router.navigate(['/admin/document-type'])
          } else {
            this.toastr.error(res.message)
          }
        })
      } 
      else{
        const addDataPromise = this.typeService.create(this.dataType).subscribe((res) => {
          if (res.isSuccess) {
            
            this.toastr.success('Thêm danh mục thành công');
            this.router.navigate(['/admin/document-type'])
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
