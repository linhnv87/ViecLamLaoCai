import { Component, OnInit } from '@angular/core';
import { DocumentService } from 'src/app/services/admin/document.service';

@Component({
  selector: 'app-pie-chart',
  templateUrl: './pie-chart.component.html',
  styleUrls: ['./pie-chart.component.css']
})
export class PieChartComponent implements OnInit {
  public doughnutChartData: any[] = [
    { data: [25, 35, 40], backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56'], label: 'Series A' }
  ];

  public doughnutChartLabels: string[] = ['Red', 'Blue', 'Yellow'];

  public doughnutChartOptions: any = {
    responsive: true,
  };

  public doughnutChartType: any = 'doughnut';
  public doughnutChartLegend = true;
  public doughnutChartColors = [
    {
      backgroundColor: ['#16F1EA', '#B72BB1', '#C458B6', '#DF292D'],
    },
  ];

  constructor(private documentService: DocumentService) { }

  ngOnInit(): void {
    this.getChartData()
  }

  getChartData(){
    this.documentService.getPieChartData().subscribe(res => {
      if(res.isSuccess){
        const data = res.result;
        this.doughnutChartLabels = data.labels;        
        
        this.doughnutChartData = data.datasets;
        this.doughnutChartColors = [
          {
            backgroundColor: data.datasets[0].backgroundColor
          }
        ]
      }
    });    
  } 
}
