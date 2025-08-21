import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
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
    SearchResultsComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    SharedModule,
    WebsiteRoutingModule,
  ]
})
export class WebsiteModule { }
