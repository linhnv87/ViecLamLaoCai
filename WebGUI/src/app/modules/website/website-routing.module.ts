import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WebsiteComponent } from './website.component';
import { AuthGuard } from '../../guards/auth.guard';

// Import components
import { HomeComponent } from './pages/home/home.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { FindCandidatesComponent } from './pages/find-candidates/find-candidates.component';
import { ApplicationsComponent } from './pages/applications/applications.component';
import { BusinessApprovalComponent } from './pages/business-approval/business-approval.component';
import { StatisticsReportComponent } from './pages/statistics-report/statistics-report.component';
import { LaborMarketReportComponent } from './pages/labor-market-report/labor-market-report.component';
import { SearchResultsComponent } from './pages/search-results/search-results.component';
import { RegisterBusinessComponent } from './pages/register-business/register-business.component';
import { RegisterCandidateComponent } from './pages/register-candidate/register-candidate.component';
import { CvManagementComponent } from './pages/cv-management/cv-management.component';
import { CvBuilderComponent } from './pages/cv-builder/cv-builder.component';
import { JobDetailComponent } from './pages/job-detail/job-detail.component';
import { ChangeInfoPageComponent } from './pages/change-info/change-info-page.component';
import { ChangePasswordPageComponent } from './pages/change-password/change-password-page.component';

const routes: Routes = [
  {
    path: '',
    component: WebsiteComponent,
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', component: HomeComponent },
      { path: 'register-business', component: RegisterBusinessComponent },
      { path: 'register-candidate', component: RegisterCandidateComponent },
      { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
      { path: 'find-candidates', component: FindCandidatesComponent, canActivate: [AuthGuard] },
      { path: 'applications', component: ApplicationsComponent, canActivate: [AuthGuard] },
      { path: 'business-approval', component: BusinessApprovalComponent, canActivate: [AuthGuard] },
      { path: 'statistics-report', component: StatisticsReportComponent, canActivate: [AuthGuard] },
      { path: 'labor-market-report', component: LaborMarketReportComponent, canActivate: [AuthGuard] },
  
      { path: 'cv-management', component: CvManagementComponent },
      { path: 'cv-builder', component: CvBuilderComponent },
      { path: 'cv-builder/:id', component: CvBuilderComponent },
      
      { path: 'change-info', component: ChangeInfoPageComponent, canActivate: [AuthGuard] },
      { path: 'change-password', component: ChangePasswordPageComponent, canActivate: [AuthGuard] },
      
      { path: 'search', component: SearchResultsComponent },
      { path: 'job/:id', component: JobDetailComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WebsiteRoutingModule { }
