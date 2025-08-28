import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { WebsiteRoutingModule } from './website-routing.module';
import { WebsiteComponent } from './website.component';
import { SharedModule } from '../../shared/shared.module';

// Import all page components
import { HomeComponent } from './pages/home/home.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { FindCandidatesComponent } from './pages/find-candidates/find-candidates.component';
import { ApplicationsComponent } from './pages/applications/applications.component';
import { BusinessApprovalComponent } from './pages/business-approval/business-approval.component';
import { StatisticsReportComponent } from './pages/statistics-report/statistics-report.component';
import { LaborMarketReportComponent } from './pages/labor-market-report/labor-market-report.component';
import { RegisterBusinessComponent } from './pages/register-business/register-business.component';
import { RegisterCandidateComponent } from './pages/register-candidate/register-candidate.component';
import { SearchResultsComponent } from './pages/search-results/search-results.component';
import { CvManagementComponent } from './pages/cv-management/cv-management.component';
import { CvBuilderComponent } from './pages/cv-builder/cv-builder.component';
import { JobDetailComponent } from './pages/job-detail/job-detail.component';
import { ChangeInfoPageComponent } from './pages/change-info/change-info-page.component';
import { ChangePasswordPageComponent } from './pages/change-password/change-password-page.component';
import { EmailVerificationComponent } from './pages/email-verification/email-verification.component';

@NgModule({
  declarations: [
    WebsiteComponent,
    HomeComponent,
    DashboardComponent,
    FindCandidatesComponent,
    ApplicationsComponent,
    BusinessApprovalComponent,
    StatisticsReportComponent,
    LaborMarketReportComponent,
    RegisterBusinessComponent,
    RegisterCandidateComponent,
    SearchResultsComponent,
    CvManagementComponent,
    CvBuilderComponent,
    JobDetailComponent,
    ChangeInfoPageComponent,
    ChangePasswordPageComponent,
    EmailVerificationComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    WebsiteRoutingModule,
  ]
})
export class WebsiteModule { }

