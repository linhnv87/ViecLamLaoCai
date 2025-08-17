import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { StatusCode } from 'src/app/models/enums/statusCode.enum';
import { FieldModel } from 'src/app/models/field.model';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import * as common from 'src/app/utils/commonFunctions';
@Component({
  selector: 'app-field',
  templateUrl: './field.component.html',
  styleUrls: ['./field.component.css']
})
export class FieldComponent {

  userRoles = common.GetRoleInfo();
  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();
  itemId: string | undefined | null;
  isModalOpen: boolean = false;  
  waitForResponse = false;

  files: File[] = [];
  statusCode?: number;
  openModal() {
    this.isModalOpen = true;
  }
  closeModal() {
    this.isModalOpen = false;
  }
  searchText: string = '';
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
  updatedItemId: number | null = null;
  constructor(private route: ActivatedRoute, private router: Router, private fieldService: FieldServiceService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private modalService: NgbModal
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
    if (item) {
      const modalRef = this.modalService.open(ConfirmationComponent);
      modalRef.result.then(result => {
        if(result){
          const deletePromise = this.fieldService.delete(item).subscribe(res => {
            if (res.isSuccess) {
              this.toastr.success('Xóa tờ trình thành công')
              this.loadtenfield();
            } else {
              this.toastr.error(res.message);
            }
          })
        }
      })

    }
  }  

  moveUp(id: number) {
    const index = this.fieldData.findIndex(item => item.id === id);
    if (index > 0) {
      this.swapOrder(index, index - 1,id);
    }
  }

  moveDown(id: number) {
    const index = this.fieldData.findIndex(item => item.id === id);
    if (index < this.fieldData.length - 1) {
      this.swapOrder(index, index + 1,id);
    }
  }

  swapOrder(fromIndex: number, toIndex: number,id:number) {
    [this.fieldData[fromIndex], this.fieldData[toIndex]] = [this.fieldData[toIndex], this.fieldData[fromIndex]];
    this.fieldData.forEach((item, index) => item.order = index + 1);
    this.updateOrder(id);
  }
  
  updateOrder(id:number) {
    const orderList = this.fieldData.map(item => ({
      id: item.id,
      order: item.order ?? 0 
    }));
  
    this.fieldService.updateOrder(orderList).subscribe(res => {
      if (res.isSuccess) {
        this.toastr.success('Cập nhật thứ tự thành công');
        this.actionOnItem = 0;
        this.updatedItemId = id;
      } else {
        this.toastr.error(res.message);
      }
    });
  }
  clearHighlight() {
    this.updatedItemId = null;
  }
  actionOnItem = 0;
  toggleActions(id: number) {
    if (this.actionOnItem != id) {
      this.actionOnItem = id;
    } else {
      this.actionOnItem = 0;
    }
  }
  get filteredFieldData() {
    return this.fieldData.filter(doc =>
      doc.title?.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }
  itemsPerPage: number = 10;
  currentPage: number = 1;
}
