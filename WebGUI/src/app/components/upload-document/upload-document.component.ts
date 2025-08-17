import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DocumentService } from 'src/app/services/admin/document.service';
import { ConfirmationComponent } from '../confirmation/confirmation.component';

@Component({
  selector: 'app-upload-document',
  templateUrl: './upload-document.component.html',
  styleUrls: ['./upload-document.component.css']
})
export class UploadDocumentComponent implements OnInit {

  @Input() docId = 0;
  files: File[] = [];
  selectedFileNames: string = 'Chưa có file nào được chọn.';
  selectedFileNamesArray: string[] = [];
  selectImg: boolean = false;
  constructor(private activeModal: NgbActiveModal, private documentService: DocumentService, private modalService: NgbModal){}
  ngOnInit(): void {
      
  }

  closeModal(){
    this.activeModal.dismiss();
  }
  confirmAction(){    
    if(this.files.length == 0 || this.docId == 0){
      alert(`File đính kèm không được để trống`)
    }
    else{
      const modalRef = this.modalService.open(ConfirmationComponent);
      modalRef.result.then(result => {
        if(result){
          this.documentService.publish(this.docId, this.files).subscribe(res => {
            if(res.isSuccess){
              this.activeModal.close(true);
            }
            else{
              console.log(res);
              
            }
          })
        }
      })
    }
  }  

  onFileSelected(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files) {
      const newFiles = Array.from(inputElement.files);
      if (newFiles.length > 0) {
        this.files = this.files.concat(newFiles);
        const newFileNames = newFiles.map((file) => file.name);
        this.selectedFileNamesArray = this.selectedFileNamesArray.concat(newFileNames);
        this.selectImg = true;
      }
    }
  }
  deleteFile(index: number): void {
    this.files.splice(index, 1);
    this.updateSelectedFileNamesArray();
  }
  
  updateSelectedFileNamesArray(): void {
    this.selectedFileNamesArray = this.files.map((file) => file.name);
  }
}
