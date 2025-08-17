import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DocumentFileService } from 'src/app/services/admin/document-file.service';
import * as common from 'src/app/utils/commonFunctions';
import { ToastrService } from 'ngx-toastr';
import { DocumentFileModel } from 'src/app/models/documentFile.model';

@Component({
  selector: 'app-add-file',
  templateUrl: './add-file.component.html',
  styleUrls: ['./add-file.component.scss']
})
export class AddFileComponent implements OnInit {
  @Input() docId!: number; 
  userId = common.GetCurrentUserId();
  sideFiles: File[] = [];

  constructor(
    public activeModal: NgbActiveModal,
    private documentFileService: DocumentFileService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  save(): void {
    if (this.sideFiles.length > 0) {
      this.documentFileService.addRelatedDocument(this.docId, this.userId, this.sideFiles).subscribe(
        res => {
          if (res.isSuccess) {
            this.toastr.success('Thêm văn bản thành công!', 'Thành công');
            this.activeModal.close(res.result);
          } else {
            this.toastr.error('Không thể thêm văn bản, vui lòng thử lại.', 'Lỗi');
          }
        }
      );
    }
  }
  dismiss(): void {
    this.activeModal.dismiss();
  }
  onSideFileSelected(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files) {
      const newFiles = Array.from(inputElement.files);
      if (newFiles.length > 0) {
        this.sideFiles = this.sideFiles.concat(newFiles);
      }
    }
    inputElement.value = '';
  }
  moveSideFileUp(index: number): void {
    if (index > 0) {
      const temp = this.sideFiles[index];
      this.sideFiles[index] = this.sideFiles[index - 1];
      this.sideFiles[index - 1] = temp;
      // this.updateSelectedFileNamesArray();
    }
  }

  moveSideFileDown(index: number): void {
    if (index < this.sideFiles.length - 1) {
      const temp = this.sideFiles[index];
      this.sideFiles[index] = this.sideFiles[index + 1];
      this.sideFiles[index + 1] = temp;
      // this.updateSelectedFileNamesArray();
    }
  }
  deleteSideFile(index: number): void {
    this.sideFiles.splice(index, 1);
    // this.updateSelectedFileNamesArray();
  }

}