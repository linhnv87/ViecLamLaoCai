import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { DashboardComponent } from './page/dashboard/dashboard.component';
import { ToTrinhComponent } from './page/to-trinh/to-trinh.component';
import { XuLyComponent } from './page/to-trinh/xu-ly/xu-ly.component';
import { ApprovalSummaryComponent } from './page/to-trinh/approval-summary/approval-summary.component';
import { ChangePasswordComponent } from './page/change-password/change-password.component';
import { UserComponent } from './page/user/user.component';
import { QuanLyDanhMucComponent } from './page/dashboard/quan-ly-danh-muc/quan-ly-danh-muc.component';
import { UpdateComponent } from './page/dashboard/quan-ly-danh-muc/update/update.component';
import { DocumentDetailComponent } from './page/to-trinh/document-detail/document-detail.component';
import { UserFormComponent } from './page/user/user-form/user-form.component';
import { FieldComponent } from './page/field/field.component';
import { FieldFormComponent } from './page/field/field-form/field-form.component';
import { DocumentFormComponent } from './page/to-trinh/document-form/document-form.component';
import { DocumentHistoryComponent } from './page/document-history/document-history.component';
import { AuthGuard } from 'src/app/guards/auth.guard';
import { TrackMessageResultComponent } from './page/to-trinh/track-message-result/track-message-result.component';
import { GroupComponent } from './page/group/group.component';
import { ReportYKienComponent } from './page/report/report-y-kien/report-y-kien.component';
import { ReportToTrinhComponent } from './page/report/report-to-trinh/report-to-trinh.component';
import { DocumentTypeComponent } from './page/document-type/document-type.component';
import { DocumentTypeFormComponent } from './page/document-type/document-type-form/document-type-form.component';
import { DepartmentComponent } from './page/department/department.component';
import { DepartmentFormComponent } from './page/department/department-form/department-form.component';
import { RoleComponent } from './page/role/role.component';
import { RoleFormComponent } from './page/role/role-form/role-form.component';
import { WorkFlowComponent } from './page/work-flow/work-flow.component';
import { UnitComponent } from './page/unit/unit.component';
import { UnitFormComponent } from './page/unit/unit-form/unit-form.component';

const routes: Routes = [
  {
    path: '',
    component: AdminComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: DashboardComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'Dashboard',
        component: DashboardComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'to-trinh/add',
        component: DocumentFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'to-trinh/:id/edit',
        component: DocumentFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'to-trinh/:statusCode',
        component: ToTrinhComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'to-trinh/:statusCode/add',
        component: DocumentFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'to-trinh/:statusCode/xu-ly/:id',
        component: XuLyComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'to-trinh/:statusCode/approval-summary/:id',
        component: ApprovalSummaryComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'to-trinh/:statusCode/document-detail/:id',
        component: DocumentDetailComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'changePassword',
        component: ChangePasswordComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'User',
        component: UserComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'User/add',
        component: UserFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'User/edit/:id',
        component: UserFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'field',
        component: FieldComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'field/add',
        component: FieldFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'field/edit/:id',
        component: FieldFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'history',
        component: DocumentHistoryComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'to-trinh/:statusCode/track-message-result-creator/:id',
        component: TrackMessageResultComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'track-message-result',
        component: TrackMessageResultComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'group',
        component: GroupComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'report/y-kien',
        component: ReportYKienComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'report/to-trinh',
        component: ReportToTrinhComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'document-type',
        component: DocumentTypeComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'document-type/add',
        component: DocumentTypeFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'document-type/edit/:id',
        component: DocumentTypeFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'department',
        component: DepartmentComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'department/add',
        component: DepartmentFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'department/edit/:id',
        component: DepartmentFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'role',
        component: RoleComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'role/add',
        component: RoleFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'role/edit/:id',
        component: RoleFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'work-flow',
        component: WorkFlowComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'unit',
        component: UnitComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'unit/add',
        component: UnitFormComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'unit/edit/:id',
        component: UnitFormComponent,
        canActivate: [AuthGuard],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
