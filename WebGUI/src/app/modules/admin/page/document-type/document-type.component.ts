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
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { DocumentTypeModel } from 'src/app/models/document-type.model';
import { DocumentTypeService } from 'src/app/services/admin/document-type.service';

@Component({
  selector: 'app-document-type',
  templateUrl: './document-type.component.html',
  styleUrls: ['./document-type.component.scss']
})
export class DocumentTypeComponent {
  serRoles = common.GetRoleInfo();
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
  typeData: DocumentTypeModel[] = [];
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
  constructor(private route: ActivatedRoute, private router: Router, private documentTypeService: DocumentTypeService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private modalService: NgbModal
  ) {
    this.loadData();
  }
  loadData() {
    const getAllPromise = this.documentTypeService.getAll().subscribe((res) => {
      if (res.isSuccess) {
        this.typeData = res.result as DocumentTypeModel[];
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
          const deletePromise = this.documentTypeService.delete(item).subscribe(res => {
            if (res.isSuccess) {
              this.toastr.success('Xóa loại VB thành công')
              this.loadData();
            } else {
              this.toastr.error(res.message);
            }
          })
        }
      })

    }
  }  

  moveUp(id: number) {
    const index = this.typeData.findIndex(item => item.id === id);
    if (index > 0) {
      this.swapOrder(index, index - 1,id);
    }
  }

  moveDown(id: number) {
    const index = this.typeData.findIndex(item => item.id === id);
    if (index < this.typeData.length - 1) {
      this.swapOrder(index, index + 1,id);
    }
  }

  swapOrder(fromIndex: number, toIndex: number,id:number) {
    [this.typeData[fromIndex], this.typeData[toIndex]] = [this.typeData[toIndex], this.typeData[fromIndex]];
    this.typeData.forEach((item, index) => item.order = index + 1);
    this.updateOrder(id);
  }
  
  updateOrder(id:number) {
    const orderList = this.typeData.map(item => ({
      id: item.id,
      order: item.order ?? 0 
    }));
  
    this.documentTypeService.updateOrder(orderList).subscribe(res => {
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
  itemsPerPage: number = 10;
  currentPage: number = 1;
  get filteredTypeData() {
    return this.typeData.filter(doc =>
      doc.name.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }
}