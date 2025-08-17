import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminComponent } from './admin.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { DpDatePickerModule } from 'ng2-date-picker';
import { NgxPaginationModule } from 'ngx-pagination';
import { DashboardComponent } from './page/dashboard/dashboard.component';
import { ToTrinhComponent } from './page/to-trinh/to-trinh.component';
import { XuLyComponent } from './page/to-trinh/xu-ly/xu-ly.component';
import { AdminHeaderComponent } from 'src/app/components/admin-header/admin-header.component';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import { ApprovalSummaryComponent } from './page/to-trinh/approval-summary/approval-summary.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChangePasswordComponent } from './page/change-password/change-password.component';
import { UserComponent } from './page/user/user.component';
import { QuanLyDanhMucComponent } from './page/dashboard/quan-ly-danh-muc/quan-ly-danh-muc.component';
import { UpdateComponent } from './page/dashboard/quan-ly-danh-muc/update/update.component';
import { DocumentDetailComponent } from './page/to-trinh/document-detail/document-detail.component';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { DialogService } from 'src/app/services/dialog.service';
import { UserFormComponent } from './page/user/user-form/user-form.component';
import { FieldComponent } from './page/field/field.component';
import { FieldFormComponent } from './page/field/field-form/field-form.component';
import { DocumentFormComponent } from './page/to-trinh/document-form/document-form.component';
import { NgChartsModule } from 'ng2-charts';
import { LineChartComponent } from 'src/app/components/line-chart/line-chart.component';
import { PieChartComponent } from 'src/app/components/pie-chart/pie-chart.component';
import { DocumentHistoryComponent } from './page/document-history/document-history.component';
import { LeftMenuComponent } from 'src/app/components/left-menu/left-menu.component';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { NgbDatepickerModule, NgbTimepickerModule } from '@ng-bootstrap/ng-bootstrap';
import {
  NgxMatDatetimePickerModule,
  NgxMatNativeDateModule,
  NgxMatTimepickerModule,
} from '@angular-material-components/datetime-picker';

import { MatDatepickerModule } from '@angular/material/datepicker';
import { NgxMomentDateModule } from '@angular-material-components/moment-adapter';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MAT_DATE_LOCALE, MatNativeDateModule } from '@angular/material/core';
import { GetTitleByStatusPipe } from './pipes/get-title-by-status.pipe';
import { DocumentDetailXuLyComponent } from './page/to-trinh/document-detail-xu-ly/document-detail-xu-ly.component';
import { DocumentDetailKetQuaYKienComponent } from './page/to-trinh/document-detail-ket-qua-y-kien/document-detail-ket-qua-y-kien.component';
import { DocumentDetailHistoryListComponent } from './page/to-trinh/document-detail-history-list/document-detail-history-list.component';
import { MatRadioModule } from '@angular/material/radio';
import { FilterUserPipe, PermissionPipe } from 'src/app/pipes';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { DocumentDetailInformationComponent } from './page/to-trinh/document-detail-information/document-detail-information.component';
import { GetBadgeByStatusPipe } from './pipes/get-badge-by-status.pipe';
import { MatTableModule } from '@angular/material/table';
import { PlanRemindComponent } from './page/to-trinh/plan-remind/plan-remind.component';
import { TrackMessageResultComponent } from './page/to-trinh/track-message-result/track-message-result.component';
import { FilterSelectedUserPipe } from './pipes/filter-selected-user.pipe';
import { SelectUserDialogComponent } from './page/to-trinh/select-user-dialog/select-user-dialog.component';
import { GroupComponent } from './page/group/group.component';
import { GroupDetailComponent } from './page/group/group-detail/group-detail.component';
import { ReportYKienComponent } from './page/report/report-y-kien/report-y-kien.component';
import { ReportToTrinhComponent } from './page/report/report-to-trinh/report-to-trinh.component';
import { ReportDetailComponent } from './page/report/report-detail/report-detail.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { DocumentTypeComponent } from './page/document-type/document-type.component';
import { DocumentTypeFormComponent } from './page/document-type/document-type-form/document-type-form.component';
import { DepartmentComponent } from './page/department/department.component';
import { DepartmentFormComponent } from './page/department/department-form/department-form.component';
import { RoleComponent } from './page/role/role.component';
import { RoleFormComponent } from './page/role/role-form/role-form.component';
import { WorkFlowComponent } from './page/work-flow/work-flow.component';
import { WorkFlowDetailComponent } from './page/work-flow/work-flow-detail/work-flow-detail.component';
import { UnitComponent } from './page/unit/unit.component';
import { UnitFormComponent } from './page/unit/unit-form/unit-form.component';

const MaterialModule = [
  MatProgressBarModule,
  MatTooltipModule,
  MatProgressSpinnerModule,
  MatTabsModule,
  MatDialogModule,
  MatButtonModule,
  MatDatepickerModule,
  MatFormFieldModule,
  MatInputModule,
  MatButtonToggleModule,
  MatNativeDateModule,
  MatRadioModule,
  MatCheckboxModule,
  MatTableModule,
];
@NgModule({
  declarations: [
    AdminComponent,
    DashboardComponent,
    ToTrinhComponent,
    XuLyComponent,
    AdminHeaderComponent,
    ApprovalSummaryComponent,
    ChangePasswordComponent,
    UserComponent,
    QuanLyDanhMucComponent,
    UpdateComponent,
    DocumentDetailComponent,
    UserFormComponent,
    FieldComponent,
    FieldFormComponent,
    DocumentFormComponent,
    LineChartComponent,
    PieChartComponent,
    DocumentHistoryComponent,
    LeftMenuComponent,
    GetTitleByStatusPipe,
    DocumentDetailXuLyComponent,
    DocumentDetailKetQuaYKienComponent,
    DocumentDetailHistoryListComponent,
    PermissionPipe,
    FilterUserPipe,
    DocumentDetailInformationComponent,
    GetBadgeByStatusPipe,
    PlanRemindComponent,
    TrackMessageResultComponent,
    FilterSelectedUserPipe,
    SelectUserDialogComponent,
    GroupComponent,
    GroupDetailComponent,
    ReportYKienComponent,
    ReportToTrinhComponent,
    ReportDetailComponent,
    DocumentTypeComponent,
    DocumentTypeFormComponent,
    DepartmentComponent,
    DepartmentFormComponent,
    RoleComponent,
    RoleFormComponent,
    WorkFlowComponent,
    WorkFlowDetailComponent,
    UnitComponent,
    UnitFormComponent,
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    AdminRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    DpDatePickerModule,
    NgxPaginationModule,
    NgxDocViewerModule,
    NgChartsModule,
    NgxExtendedPdfViewerModule,
    NgbTimepickerModule,
    NgxMatDatetimePickerModule,
    NgxMatTimepickerModule,
    NgxMomentDateModule,
    NgxMatNativeDateModule,
    NgbDatepickerModule,
    DragDropModule,
    ...MaterialModule,
  ],
  providers: [DialogService, { provide: MAT_DATE_LOCALE, useValue: 'vi' }],
})
export class AdminModule {}
