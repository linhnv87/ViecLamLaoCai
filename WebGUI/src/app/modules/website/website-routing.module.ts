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

const routes: Routes = [
  {
    path: '',
    component: WebsiteComponent,
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },

      // Public website pages
      { path: 'home', component: HomeComponent },
      { path: 'register-business', component: RegisterBusinessComponent },
      { path: 'register-candidate', component: RegisterCandidateComponent },

      // Dashboard routes cho doanh nghiệp (yêu cầu đăng nhập)
      { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
      { path: 'find-candidates', component: FindCandidatesComponent, canActivate: [AuthGuard] },
      { path: 'applications', component: ApplicationsComponent, canActivate: [AuthGuard] },
                  { path: 'business-approval', component: BusinessApprovalComponent, canActivate: [AuthGuard] },
            { path: 'statistics-report', component: StatisticsReportComponent, canActivate: [AuthGuard] },
            { path: 'labor-market-report', component: LaborMarketReportComponent, canActivate: [AuthGuard] },
            { path: 'search', component: SearchResultsComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WebsiteRoutingModule { }
