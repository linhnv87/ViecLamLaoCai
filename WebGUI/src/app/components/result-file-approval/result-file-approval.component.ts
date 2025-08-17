import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { GetFullFilePath } from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-result-file-approval',
  templateUrl: './result-file-approval.component.html',
  styleUrls: ['./result-file-approval.component.scss']
})
export class ResultFileApprovalComponent {
  @Input() modalFiles: DocumentFileModel[] = [];
  constructor(private activeModal: NgbActiveModal, private router: Router) {}

  closeModal() {
    this.activeModal.dismiss();
    this.router.navigate([], {
      queryParams: { fileName: null, userId: null },
      queryParamsHandling: 'merge',
    });
  }
  getFileIcon(filePath: string): string {
    const fileExtension = filePath.split('.').pop()?.toLowerCase();
    if (fileExtension === 'pdf') {
      return 'fas fa-eye ';
    } else if (fileExtension === 'doc' || fileExtension === 'docx') {
      return 'fa-solid fa-download';
    } else {
      return 'fa-solid fa-download';
    }
  }
  getFileTooltip(filePath: string): string {
    if (!filePath) return 'Không có file';
    const fileExtension = filePath.split('.').pop()?.toLowerCase();
    if (fileExtension === 'pdf') {
      return 'Xem File';
    } else if (fileExtension === 'doc' || fileExtension === 'docx') {
      return 'Tải xuống';
    } else {
      return 'Tải xuống';
    }
  }  
  downloadFile(filePath: string) {
    const fullPath = GetFullFilePath(filePath);
    const fileExtension = fullPath.split('.').pop()?.toLowerCase();
    if (fileExtension === 'pdf') {
      window.open(fullPath, '_blank', 'width=800,height=600,top=100,left=100');
    } 
    else {
      const anchor = document.createElement('a');
      anchor.href = fullPath;
      anchor.target = '_blank';
      document.body.appendChild(anchor);
      anchor.click();
      document.body.removeChild(anchor);
    }
  }
  
  
}
