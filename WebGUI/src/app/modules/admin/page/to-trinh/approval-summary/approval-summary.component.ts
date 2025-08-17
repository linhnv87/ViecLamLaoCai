import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DocumentApprovalQueryModel, DocumentApprovalSummaryModel, DocumentApprovalSummaryModel_V2 } from 'src/app/models/documentApproval.model';
import { FieldModel } from 'src/app/models/field.model';
import { DocumentApprovalService } from 'src/app/services/admin/document-approval.service';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import * as moment from 'moment';
import { UserService } from 'src/app/services/user.service';
import { GetAllInfoUserModel } from 'src/app/models/user.model';
import { ExcelExportService } from 'src/app/services/excel-export.service';

@Component({
  selector: 'app-approval-summary',
  templateUrl: './approval-summary.component.html',
  styleUrls: ['./approval-summary.component.css']
})
export class ApprovalSummaryComponent implements OnInit {

  filterByDate = false;
  docId: number = 0;
  filter: DocumentApprovalQueryModel = {
    title: '',
    authorId: '',
    fieldId: 0,
    status: 0,
    // isPassed: 0,
    submitFrom: '',//new Date().toISOString().split('T')[0],//moment(new Date()).format('dd/MM/yyyy'),
    submitTo: '',//new Date().toISOString().split('T')[0]//moment(new Date()).format('dd/MM/yyyy')
  }
  fieldSelections = [
    {
      key: 0,
      label: 'Tất cả danh mục'
    }
  ]  
  userSelections = [
    {
      key: '',
      label: 'Tất cả chuyên viên'
    }
  ]
  statusSelections = [
    {
      key: 0,
      label: 'Tất cả trạng thái'
    },
    {
      key: 3,
      label: 'Chờ xử lý'
    },
    {
      key: 5,
      label: 'Bổ sung ý kiến'
    },
    {
      key: 4,
      label: 'Chờ thông qua'
    },
    {
      key: 8,
      label: 'Chờ ra nghị quyết'
    },
    {
      key: 9,
      label: 'Hoàn thành'
    },
    {
      key: 6,
      label: 'Không duyệt'
    },
    {
      key: 10,
      label: 'Trả lại'
    },
    {
      key: 7,
      label: 'Quá hạn'
    },
  ]
  summaryData: DocumentApprovalSummaryModel_V2[] = [];
  constructor(
    private excelExportService: ExcelExportService,
    private approvalService: DocumentApprovalService, private route: ActivatedRoute, private fieldService: FieldServiceService, private userService: UserService){}

  ngOnInit(): void {
      // this.getParamValue('id');
      this.loadField();
      // this.loadData(false);
      this.loadUsers();
      // this.loadData_V2();
  }

  inputTypeA = 'text';
  inputTypeB = 'text';
  dateValue: string = '';
  // isInputInFocus = false;

  onInputFocus(type: number) {
    if(type == 1){
      this.inputTypeA = 'date';
    }
    if(type == 2){
      this.inputTypeB = 'date';
    }
    // this.isInputInFocus = true;
  }

  onInputBlur(type: number) {
    if(type == 1){
      this.inputTypeA = 'text';
    }
    if(type == 2){
      this.inputTypeB = 'text';
    }
  }

  loadField(){
    this.fieldService.getAll().subscribe(res => {
      if(res.isSuccess){
        const fieldsData = res.result as FieldModel[];
        fieldsData.forEach(e => {
          this.fieldSelections.push({key: e.id!, label: e.title!})
        })
      }
    })
  }

  loadUsers(){
    this.userService.GetAllSpecialistInfo().subscribe(res => {
      if(res.isSuccess){
        const fieldsData = res.result as GetAllInfoUserModel[];
        fieldsData.forEach(e => {
          this.userSelections.push({key: e.userId!, label: e.userFullName!})
        })
      }
    })
  }

  exportExcel(){    
    // Transform the data with new headers
    const transformedData: any[] = []
    this.summaryData.forEach((e, i) => {
      var customObj: any = {};
      customObj['STT'] = i + 1
      customObj['Tiêu đề tờ trình'] = e.title
      customObj['Chuyên viên trình'] = e.author
      customObj['Lĩnh vực'] = e.field
      customObj['Ngày trình'] = e.submittedAt
      customObj['Hạn phê duyệt'] = e.deadlineAt
      customObj['Thời gian kết thúc'] = e.endAt
      customObj['Kết quả'] = e.isPassed ? 'Thông qua' : 'Không thông qua'
      customObj['Số lần trình'] = e.submitCount
      transformedData.push(customObj)
    })
    console.log(transformedData);
    
    this.excelExportService.exportToExcel(transformedData, 'Biểu tổng hợp tờ trình', 'Tổng hợp tờ trình')
  }



  getParamValue(key: string){
    if(this.route.snapshot.paramMap.get(key) != null) {
      this.docId = parseInt(this.route.snapshot.paramMap.get('id')!);      
    }
  }

  // loadData(hasFilter: boolean){
  //   this.approvalService.GetApprovalSummary(this.docId).subscribe(res => {            
  //     if(res.isSuccess){
  //       this.summaryData = res.result as DocumentApprovalSummaryModel[];        
  //       this.summaryData.forEach(e => {
  //         var t = Math.max(1, e.approvals.length, e.declines.length, e.noResponses.length);
  //         e.maxRows = t;          
  //       })
  //       if(hasFilter){
  //         const curentDate = new Date().toISOString().split('T')[0];
  //         this.summaryData = this.summaryData.filter(e => 
  //           (this.filter.title.trim() == '' || e.title.toLowerCase().includes(this.filter.title.toLowerCase().trim()))
  //           && (this.filter.author.trim() == '' || e.submitter.toLowerCase().includes(this.filter.author.toLowerCase().trim()))
  //           && (this.filter.field.trim() == '' || e.field.toLowerCase().trim() == this.filter.field.toLowerCase().trim())
  //           && (this.filter.status.trim() == '' || e.status.toLowerCase().trim() == this.filter.status.toLowerCase().trim())
  //           && (!this.filterByDate || !moment(e.submittedAt).isBefore(moment(this.filter.fromDate)))
  //           && (!this.filterByDate || !moment(e.submittedAt).isAfter(moment(this.filter.toDate).add(1, 'day')))
  //         )
  //       }
  //     }
  //   })
  // }

  loadData_V2(){
    this.approvalService.GetApprovalSummary_V2(this.filter).subscribe(res => {            
      if(res.isSuccess){
        this.summaryData = res.result as DocumentApprovalSummaryModel_V2[];        
        this.summaryData.forEach(e => {
          var t = Math.max(1, e.approvals.length, e.declines.length, e.noResponses.length);
          e.maxRows = t;          
        })        
      }
    })
  }

  onFilter(){
    console.log(this.filter);
    // this.loadData(true);    
    this.loadData_V2()
  }
}
