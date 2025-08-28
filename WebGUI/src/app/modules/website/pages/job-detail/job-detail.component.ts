import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { JobService } from '../../../../services/website/job.service';
import { JobDetailModel, HotJob } from '../../../../models/job.model';

@Component({
  selector: 'app-job-detail',
  templateUrl: './job-detail.component.html',
  styleUrls: ['./job-detail.component.scss']
})
export class JobDetailComponent implements OnInit {
  job: JobDetailModel | null = null;
  isLoading = true;
  activeTab: 'thongtin' | 'congty' | 'phuhop' = 'thongtin';
  hotJobs: HotJob[] = [];

  constructor(
    private route: ActivatedRoute,
    private jobService: JobService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : 0;
    this.loadJob(id);
    this.loadHotJobs();
  }

  loadJob(id: number): void {
    this.isLoading = true;
    this.jobService.getByIdDummy(id).subscribe((res: { isSuccess: boolean; result: JobDetailModel }) => {
      this.job = res.result;
      this.isLoading = false;
    });
  }

  trackByIndex(index: number): number { return index; }

  setTab(tab: 'thongtin' | 'congty' | 'phuhop'): void {
    this.activeTab = tab;
  }

  private loadHotJobs(): void {
    this.jobService.getHotJobsDummy(6).subscribe((res: { isSuccess: boolean; result: HotJob[] }) => {
      this.hotJobs = res.result || [];
    });
  }
}


