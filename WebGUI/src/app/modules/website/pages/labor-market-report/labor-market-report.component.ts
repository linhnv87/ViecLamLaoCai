import { Component, OnInit } from '@angular/core';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { LaborMarketService } from '../../../../services/website/labor-market.service';
import { MarketOverview, JobTrend, SkillDemand, SalaryRange, MarketInsight } from '../../../../models/labor-market.model';

// Note: All interfaces are now imported from LaborMarketService
// This eliminates duplicate interface declarations

@Component({
  selector: 'app-labor-market-report',
  templateUrl: './labor-market-report.component.html',
  styleUrls: ['./labor-market-report.component.scss']
})
export class LaborMarketReportComponent implements OnInit {

  marketOverview: MarketOverview = {
    totalActiveJobs: 0,
    totalCompaniesHiring: 0,
    avgTimeToHire: 0,
    jobGrowthRate: 0,
    candidateCompetition: 0,
    salaryGrowth: 0
  };

  // Job trends by industry
  jobTrends: JobTrend[] = [
    {
      industry: 'Công nghệ thông tin',
      demandLevel: 'high',
      growth: 25.8,
      avgSalary: '15-35 triệu',
      topPositions: ['Frontend Developer', 'Backend Developer', 'Full Stack Developer', 'DevOps Engineer'],
      color: '#007aff'
    },
    {
      industry: 'Thương mại điện tử',
      demandLevel: 'high',
      growth: 22.3,
      avgSalary: '12-28 triệu',
      topPositions: ['Digital Marketing', 'E-commerce Manager', 'Content Creator', 'Data Analyst'],
      color: '#28a745'
    },
    {
      industry: 'Tài chính ngân hàng',
      demandLevel: 'medium',
      growth: 15.7,
      avgSalary: '18-45 triệu',
      topPositions: ['Financial Analyst', 'Risk Manager', 'Investment Advisor', 'Credit Officer'],
      color: '#ffc107'
    },
    {
      industry: 'Sản xuất chế biến',
      demandLevel: 'medium',
      growth: 12.4,
      avgSalary: '10-25 triệu',
      topPositions: ['Production Manager', 'Quality Control', 'Supply Chain', 'Process Engineer'],
      color: '#fd7e14'
    },
    {
      industry: 'Du lịch khách sạn',
      demandLevel: 'low',
      growth: 8.9,
      avgSalary: '8-20 triệu',
      topPositions: ['Tour Guide', 'Hotel Manager', 'Event Coordinator', 'Customer Service'],
      color: '#6f42c1'
    },
    {
      industry: 'Giáo dục đào tạo',
      demandLevel: 'low',
      growth: 6.2,
      avgSalary: '12-30 triệu',
      topPositions: ['Online Teacher', 'Course Designer', 'Academic Coordinator', 'Training Specialist'],
      color: '#dc3545'
    }
  ];

  // Skill demands
  skillDemands: SkillDemand[] = [
    { skill: 'JavaScript/TypeScript', demand: 1456, growth: 28.5, category: 'Programming', level: 'hot' },
    { skill: 'React/Vue.js', demand: 1234, growth: 25.2, category: 'Frontend', level: 'hot' },
    { skill: 'Node.js/Python', demand: 1123, growth: 22.8, category: 'Backend', level: 'hot' },
    { skill: 'Digital Marketing', demand: 987, growth: 20.3, category: 'Marketing', level: 'hot' },
    { skill: 'Data Analysis', demand: 856, growth: 18.7, category: 'Analytics', level: 'hot' },
    { skill: 'Project Management', demand: 743, growth: 15.2, category: 'Management', level: 'stable' },
    { skill: 'UI/UX Design', demand: 689, growth: 12.8, category: 'Design', level: 'stable' },
    { skill: 'Cloud Computing', demand: 567, growth: 35.6, category: 'Infrastructure', level: 'hot' },
    { skill: 'Mobile Development', demand: 456, growth: 28.9, category: 'Mobile', level: 'hot' },
    { skill: 'DevOps/CI-CD', demand: 398, growth: 32.4, category: 'Operations', level: 'hot' }
  ];

  // Salary ranges
  salaryRanges: SalaryRange[] = [
    { position: 'Senior Full Stack Developer', minSalary: 25, maxSalary: 50, avgSalary: 35, experience: '3-5 năm', count: 156 },
    { position: 'Product Manager', minSalary: 30, maxSalary: 60, avgSalary: 42, experience: '5-7 năm', count: 89 },
    { position: 'DevOps Engineer', minSalary: 28, maxSalary: 55, avgSalary: 38, experience: '3-5 năm', count: 134 },
    { position: 'Data Scientist', minSalary: 22, maxSalary: 45, avgSalary: 32, experience: '2-4 năm', count: 112 },
    { position: 'UI/UX Designer', minSalary: 18, maxSalary: 35, avgSalary: 25, experience: '2-4 năm', count: 167 },
    { position: 'Digital Marketing Manager', minSalary: 15, maxSalary: 30, avgSalary: 22, experience: '3-5 năm', count: 145 },
    { position: 'Business Analyst', minSalary: 20, maxSalary: 40, avgSalary: 28, experience: '2-4 năm', count: 123 },
    { position: 'Quality Assurance Engineer', minSalary: 12, maxSalary: 25, avgSalary: 18, experience: '1-3 năm', count: 198 }
  ];

  // Market insights
  marketInsights: MarketInsight[] = [
    {
      title: 'Nhu cầu tuyển dụng IT tăng mạnh',
      description: 'Các công ty công nghệ tại Lào Cai đang tăng cường tuyển dụng với mức lương cạnh tranh để thu hút nhân tài.',
      impact: 'positive',
      category: 'Industry Trend',
      date: '2024-01-15'
    },
    {
      title: 'Remote work trở thành xu hướng chính',
      description: 'Hơn 70% các vị trí IT và Marketing hiện tại cho phép làm việc từ xa hoặc hybrid.',
      impact: 'positive',
      category: 'Work Model',
      date: '2024-01-12'
    },
    {
      title: 'Thiếu hụt nhân lực có kinh nghiệm',
      description: 'Các vị trí senior trong lĩnh vực công nghệ và tài chính đang có tỷ lệ cạnh tranh thấp.',
      impact: 'negative',
      category: 'Talent Gap',
      date: '2024-01-10'
    },
    {
      title: 'Tăng trường đầu tư vào đào tạo nhân lực',
      description: 'Các doanh nghiệp đang đầu tư mạnh vào chương trình đào tạo và phát triển kỹ năng nhân viên.',
      impact: 'positive',
      category: 'Investment',
      date: '2024-01-08'
    },
    {
      title: 'Xu hướng tự động hóa ảnh hưởng đến một số ngành',
      description: 'Các công việc lặp đi lặp lại trong sản xuất và dịch vụ đang được thay thế bằng công nghệ.',
      impact: 'negative',
      category: 'Technology Impact',
      date: '2024-01-05'
    },
    {
      title: 'Phát triển các kỹ năng mềm được chú trọng',
      description: 'Khả năng giao tiếp, làm việc nhóm và quản lý thời gian đang trở thành yêu cầu quan trọng.',
      impact: 'neutral',
      category: 'Skill Development',
      date: '2024-01-03'
    }
  ];

  // Filter options
  selectedTimeRange = 'quarterly';
  selectedIndustry = 'all';
  selectedRegion = 'laocai';

  constructor(
    private splashScreenService: SplashScreenService,
    private laborMarketService: LaborMarketService
  ) {}

  ngOnInit(): void {
    console.log('Labor market report component loaded');
    this.loadMarketData();
  }

  loadMarketData(): void {
    console.log('Loading labor market data...');
    
    // Load market overview
    this.laborMarketService.getMarketOverviewDummy().subscribe(res => {
      if (res.isSuccess) {
        this.marketOverview = res.result;
      }
    });

    // Load job trends
    this.laborMarketService.getJobTrendsDummy().subscribe(res => {
      if (res.isSuccess) {
        this.jobTrends = res.result;
      }
    });

    // Load skill demands
    this.laborMarketService.getSkillDemandsDummy().subscribe(res => {
      if (res.isSuccess) {
        this.skillDemands = res.result;
      }
    });
  }

  onTimeRangeChange(range: string): void {
    this.selectedTimeRange = range;
    this.updateMarketData();
  }

  onIndustryChange(industry: string): void {
    this.selectedIndustry = industry;
    this.updateMarketData();
  }

  onRegionChange(region: string): void {
    this.selectedRegion = region;
    this.updateMarketData();
  }

  updateMarketData(): void {
    console.log('Updating market data for:', {
      timeRange: this.selectedTimeRange,
      industry: this.selectedIndustry,
      region: this.selectedRegion
    });
  }

  getDemandLevelClass(level: string): string {
    switch (level) {
      case 'high': return 'demand-high';
      case 'medium': return 'demand-medium';
      case 'low': return 'demand-low';
      default: return 'demand-medium';
    }
  }

  getDemandLevelText(level: string): string {
    switch (level) {
      case 'high': return 'Nhu cầu cao';
      case 'medium': return 'Nhu cầu trung bình';
      case 'low': return 'Nhu cầu thấp';
      default: return level;
    }
  }

  getSkillLevelClass(level: string): string {
    switch (level) {
      case 'hot': return 'skill-hot';
      case 'stable': return 'skill-stable';
      case 'declining': return 'skill-declining';
      default: return 'skill-stable';
    }
  }

  getInsightIcon(impact: string): string {
    switch (impact) {
      case 'positive': return '📈';
      case 'negative': return '📉';
      case 'neutral': return '📊';
      default: return '📊';
    }
  }

  getInsightClass(impact: string): string {
    switch (impact) {
      case 'positive': return 'insight-positive';
      case 'negative': return 'insight-negative';
      case 'neutral': return 'insight-neutral';
      default: return 'insight-neutral';
    }
  }

  exportMarketReport(format: string): void {
    console.log('Exporting market report in format:', format);
    this.laborMarketService.exportReport(format).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `labor-market-report.${format}`;
      a.click();
      
      this.splashScreenService.showQuickFeedback(
        'success',
        'Xuất báo cáo thành công!',
        `Báo cáo thị trường lao động đã được xuất định dạng ${format.toUpperCase()}`
      );
    });
  }

  refreshMarketData(): void {
    console.log('Refreshing market data...');
    this.laborMarketService.refreshData().subscribe(res => {
      if (res.isSuccess) {
        this.loadMarketData();
        this.splashScreenService.showBriefLoading(
          'Đang cập nhật...',
          'Vui lòng chờ trong giây lát',
          600,
          'Dữ liệu đã được cập nhật!',
          'Báo cáo thị trường lao động mới nhất đã được tải'
        );
      }
    });
  }

  getMaxSalary(): number {
    return Math.max(...this.salaryRanges.map(range => range.maxSalary));
  }

  getSalaryBarWidth(salary: number): number {
    return (salary / this.getMaxSalary()) * 100;
  }

  getTopSkills(): SkillDemand[] {
    return this.skillDemands.slice(0, 5);
  }

  getHotSkills(): SkillDemand[] {
    return this.skillDemands.filter(skill => skill.level === 'hot');
  }
}








