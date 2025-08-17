import { DocumentService } from 'src/app/services/admin/document.service';
import { ConfirmationComponent } from '../confirmation/confirmation.component';
import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-priority-document',
    templateUrl: './priority-document.component.html',
    styleUrls: ['./priority-document.component.css']
})
export class PriorityDocumentComponent implements OnInit {
    @Input() docId = 0;
    priorityNumber = 0
    constructor(private activeModal: NgbActiveModal, private documentService: DocumentService, private modalService: NgbModal) { }
    ngOnInit(): void {
        this.documentService.getDocumentsById(this.docId).subscribe(res => {
            if (res.isSuccess) {
                this.priorityNumber = res.result.priorityNumber ?? 0;
            }
            else {
                console.log(res);
            }
        })
    }
    confirmAction() {
        if (this.priorityNumber == 0) {
            alert(`Độ ưu tiên phải lớn hơn 0`)
        }
        else {
            const modalRef = this.modalService.open(ConfirmationComponent);
            modalRef.result.then(result => {
                if (result) {
                    this.documentService.updatePriorityDocument(this.docId, this.priorityNumber).subscribe(res => {
                        if (res.isSuccess) {
                            this.activeModal.close(true);
                        }
                        else {
                            console.log(res);

                        }
                    })
                }
            })
        }
    }
    closeModal() {
        this.activeModal.dismiss();
    }
}