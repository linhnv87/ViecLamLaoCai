import { Injectable } from "@angular/core";


@Injectable({
    providedIn: 'root'
})
export class StringHelperService {
    convertToSlug(input: string): string {
        // Hàm loại bỏ dấu tiếng Việt
        const removeVietnameseTones = (str: string): string => {
            return str
                .normalize("NFD") // Tách dấu khỏi chữ
                .replace(/[\u0300-\u036f]/g, "") // Xóa dấu
                .replace(/đ/g, "d") // Thay 'đ' thành 'd'
                .replace(/Đ/g, "D"); // Thay 'Đ' thành 'D'
        };
    
        // Loại bỏ dấu
        let result = removeVietnameseTones(input);
    
        // Loại bỏ ký tự đặc biệt, giữ lại khoảng trắng
        result = result.replace(/[^a-zA-Z0-9\s]/g, '');
    
        // Thay khoảng trắng bằng dấu gạch ngang
        result = result.trim().replace(/\s+/g, "-");
    
        // Chuyển thành chữ in thường
        return result.toLowerCase();
    }
}