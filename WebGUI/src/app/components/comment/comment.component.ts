import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})
export class CommentComponent implements OnChanges {

  @Input() title = '';
  @Input() showUpload = true;
  comment: string = '';
  constructor(private activeModal: NgbActiveModal){}
  ngOnInit(): void {
      
  } 

  ngOnChanges(changes: SimpleChanges): void {
      console.log(this.title);
      
  }

  file: File | undefined;
  selectedFileName = '';

  onFileSelected(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files) {
      const newFiles = Array.from(inputElement.files);
      console.log(newFiles);      
      if (newFiles.length > 0) {
        this.file = newFiles[0];
        const newFileNames = newFiles.map((file) => file.name);
        this.selectedFileName = newFileNames[0];        
      }
    }
  }
  closeModal(){
    this.activeModal.dismiss();
  }
  confirmAction(){    
    if(this.comment == ''){
      alert(`${this.title} không được để trống`)
    }
    else{
      this.activeModal.close({comment: this.comment, file: this.file});
    }
  }  
}
