import { Component, OnInit } from '@angular/core';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { StatisticsService, StatisticsOverview, TrendData, SearchTrendData, IndustryStatistic } from '../../../../services/website/statistics.service';

@Component({
  selector: 'app-statistics-report',
  templateUrl: './statistics-report.component.html',
  styleUrls: ['./statistics-report.component.scss']
})
export class StatisticsReportComponent implements OnInit {

  statisticsOverview: StatisticsOverview = {
    totalBusinesses: 0,
    totalJobPostings: 0,
    totalApplications: 0,
    totalCandidates: 0,
    pendingApprovals: 0,
    activeJobs: 0,
    monthlyGrowth: 0,
    yearlyGrowth: 0
  };

  searchKeywords: SearchTrendData[] = [];

  topIndustries: IndustryStatistic[] = [];

  registrationTrends: TrendData[] = [];

  applicationTrends: TrendData[] = [];

  // Filter options
  selectedTimeRange = 'last12months';
  selectedIndustry = 'all';
  selectedMetric = 'all';

  constructor(
    private splashScreenService: SplashScreenService,
    private statisticsService: StatisticsService
  ) {}

  ngOnInit(): void {
    console.log('Statistics report component loaded');
    this.loadStatisticsData();
  }

  loadStatisticsData(): void {
    console.log('Loading statistics data...');
    
    // Load overview data
    this.statisticsService.getOverviewDummy().subscribe(res => {
      if (res.isSuccess) {
        this.statisticsOverview = res.result;
      }
    });

    // Load business trends
    this.statisticsService.getBusinessTrendsDummy().subscribe(res => {
      if (res.isSuccess) {
        this.registrationTrends = res.result;
      }
    });

    // Load search keywords
    this.statisticsService.getSearchKeywordsDummy().subscribe(res => {
      if (res.isSuccess) {
        this.searchKeywords = res.result;
      }
    });

    // Load application trends
    this.statisticsService.getBusinessTrendsDummy().subscribe(res => {
      if (res.isSuccess) {
        this.applicationTrends = res.result.map(trend => ({
          ...trend,
          value: trend.value * 6
        }));
      }
    });

    this.topIndustries = [
      { name: 'Công nghệ thông tin', businessCount: 234, jobCount: 1456, percentage: 28.5, color: '#007aff' },
      { name: 'Thương mại dịch vụ', businessCount: 189, jobCount: 987, percentage: 22.3, color: '#28a745' },
      { name: 'Sản xuất chế biến', businessCount: 156, jobCount: 678, percentage: 18.7, color: '#ffc107' },
      { name: 'Đầu tư tài chính', businessCount: 123, jobCount: 456, percentage: 15.2, color: '#dc3545' },
      { name: 'Du lịch dịch vụ', businessCount: 98, jobCount: 234, percentage: 9.8, color: '#6f42c1' },
      { name: 'Giáo dục đào tạo', businessCount: 67, jobCount: 189, percentage: 5.5, color: '#fd7e14' }
    ];
  }

  onTimeRangeChange(range: string): void {
    this.selectedTimeRange = range;
    this.updateChartData();
  }

  onIndustryChange(industry: string): void {
    this.selectedIndustry = industry;
    this.updateChartData();
  }

  onMetricChange(metric: string): void {
    this.selectedMetric = metric;
    this.updateChartData();
  }

  updateChartData(): void {
    console.log('Updating chart data for:', {
      timeRange: this.selectedTimeRange,
      industry: this.selectedIndustry,
      metric: this.selectedMetric
    });
  }

  getTrendIcon(trend: string): string {
    switch (trend) {
      case 'up': return '↗️';
      case 'down': return '↘️';
      case 'stable': return '➡️';
      default: return '➡️';
    }
  }

  getTrendClass(change: number): string {
    if (change > 0) return 'trend-up';
    if (change < 0) return 'trend-down';
    return 'trend-stable';
  }

  exportReport(format: string): void {
    console.log('Exporting report in format:', format);
    this.statisticsService.exportReport(format).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `statistics-report.${format}`;
      a.click();
      
      this.splashScreenService.showQuickFeedback(
        'success',
        'Xuất báo cáo thành công!',
        `Báo cáo đã được xuất định dạng ${format.toUpperCase()}`
      );
    });
  }

  refreshData(): void {
    console.log('Refreshing statistics data...');
    this.statisticsService.refreshData().subscribe(res => {
      if (res.isSuccess) {
        this.loadStatisticsData();
        this.splashScreenService.showBriefLoading(
          'Đang cập nhật...',
          'Vui lòng chờ trong giây lát',
          500,
          'Dữ liệu đã được cập nhật!',
          'Thống kê mới nhất đã được tải'
        );
      }
    });
  }

  getMaxValue(data: TrendData[]): number {
    return Math.max(...data.map(item => item.value));
  }

  getChartHeight(value: number, max: number): number {
    return (value / max) * 100;
  }

  getTotalJobs(): number {
    return this.topIndustries.reduce((total, industry) => total + industry.jobCount, 0);
  }

  getTotalBusinessesInIndustries(): number {
    return this.topIndustries.reduce((total, industry) => total + industry.businessCount, 0);
  }

  getApplicationTrendPoints(): string {
    const maxValue = this.getMaxValue(this.applicationTrends);
    return this.applicationTrends.map((trend, i) => {
      const x = i * 80 + 40;
      const y = 300 - (trend.value / maxValue) * 250;
      return `${x},${y}`;
    }).join(' ');
  }

  getApplicationTrendPoint(trend: any, index: number): { x: number, y: number } {
    const maxValue = this.getMaxValue(this.applicationTrends);
    return {
      x: index * 80 + 40,
      y: 300 - (trend.value / maxValue) * 250
    };
  }
}
