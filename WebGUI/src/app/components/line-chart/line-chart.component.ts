import { Component, OnInit } from '@angular/core';
// import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { DocumentService } from 'src/app/services/admin/document.service';

@Component({
  selector: 'app-line-chart',
  templateUrl: './line-chart.component.html',
  styleUrls: ['./line-chart.component.css']
})
export class LineChartComponent implements OnInit {

  public lineChartData: any[] = [
    { data: [65, 59, 80, 81, 56, 55, 40], label: 'Series A' },
    // Add more datasets if needed
  ];

  public lineChartLabels: string[] = ['January', 'February', 'March', 'April', 'May', 'June', 'July'];

  public lineChartOptions: any = {
    responsive: true,
  };

  public lineChartType: any = 'line';
  public lineChartLegend = true;
  constructor(private documentService: DocumentService) {    
    
  }

  ngOnInit(): void {
   this.getChartData();   
  }

  getChartData(){
    // this.documentService.getPieChartData().subscribe(res => {
    //   if(res.isSuccess){
    //     const data = res.result;
    //     this.doughnutChartLabels = data.labels;
    //     this.doughnutChartData = data.datasets[0].data;
    //     this.doughnutChartColors = [
    //       {
    //         backgroundColor: data.datasets[0].backgroundColor
    //       }
    //     ]
    //   }
    // });
    this.documentService.getMonthChart().subscribe(res => {
      if(res.isSuccess){
        const data = res.result;
        this.lineChartData = [data.datasets as any];
        this.lineChartLabels = data.labels; 
      }
    })
  } 
}
