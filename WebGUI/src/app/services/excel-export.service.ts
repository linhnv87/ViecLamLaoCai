import { Injectable } from '@angular/core';
import * as XLSX from 'xlsx-js-style';

@Injectable({
  providedIn: 'root'
})
export class ExcelExportService {

  constructor() { }
  exportToExcel(data: any[], fileName: string, sheetName: string): void {
    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(data);    
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    const columnWidths = this.calculateColumnWidths(data);
    ws['!cols'] = columnWidths;
    XLSX.utils.book_append_sheet(wb, ws, sheetName);
    XLSX.writeFile(wb, `${fileName}.xlsx`);
  }
  private calculateColumnWidths(data: any[]): any[] {
    const columnWidths: any[] = [];

    if (data.length > 0) {
      Object.keys(data[0]).forEach((key) => {
        const maxLength = Math.max(...data.map(row => String(row[key]).length));
        const colWidth = { wch: maxLength + 10 }; // Add extra space for better readability
        columnWidths.push(colWidth);
      });
    }

    return columnWidths;
  }
  exportToExcelCustom(data: any[], fileName: string, sheetName: string): void {
    const header = Object.keys(data[0]);
  
    const borderStyle = {
      top: { style: 'thin' },
      left: { style: 'thin' },
      bottom: { style: 'thin' },
      right: { style: 'thin' }
    };
  
    const headerRow = header.map((headerName) => ({
      v: headerName,
      s: { 
        font: { bold: true },
        border: borderStyle
      }
    }));
  
    const dataRows = data.map(row => header.map((key) => ({
      v: row[key],
      s: { 
        border: borderStyle
      }
    })));
  
    const dataWithHeader = [headerRow, ...dataRows];
    const ws: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(dataWithHeader);
    const columnWidths = this.calculateColumnWidths(data);
    ws['!cols'] = columnWidths;
  
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
  
    XLSX.utils.book_append_sheet(wb, ws, sheetName);
  
    XLSX.writeFile(wb, `${fileName}.xlsx`);
  }
  
}
