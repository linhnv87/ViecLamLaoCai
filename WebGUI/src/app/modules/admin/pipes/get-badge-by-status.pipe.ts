import { Pipe, PipeTransform } from '@angular/core';
import { DOCUMENT_STATUS } from 'src/app/utils/constants';

@Pipe({
  name: 'getBadgeByStatus',
})
export class GetBadgeByStatusPipe implements PipeTransform {
  transform(status?: number | string | null): string {
    let value = 'badge ';
    switch (status) {
      case DOCUMENT_STATUS.DU_THAO:
        value += 'badge-secondary';
        break;
      case DOCUMENT_STATUS.XIN_Y_KIEN:
        value += 'badge-info';
        break;
      case DOCUMENT_STATUS.XIN_Y_KIEN_LAI:
        value += 'badge-info';
        break;
      case DOCUMENT_STATUS.PHE_DUYET:
        value += 'badge-success';
        break;
      case DOCUMENT_STATUS.KY_SO:
        value += 'badge-warning';
        break;
      case DOCUMENT_STATUS.CHO_BAN_HANH:
        value += 'badge-info';
        break;
      case DOCUMENT_STATUS.BAN_HANH:
        value += 'badge-success';
        break;
      case DOCUMENT_STATUS.KHONG_BAN_HANH:
        value += 'badge-danger';
        break;
      case DOCUMENT_STATUS.TRA_LAI:
        value += 'badge-secondary';
        break;
      default:
        value += '';
        break;
    }

    return value;
  }
}
// <span *ngIf="document?.statusCode == 2" class="badge badge-secondary">Lưu tạm</span>
// <span *ngIf="document?.statusCode == 3" class="badge badge-info">Chờ duyệt</span>
// <span *ngIf="document?.statusCode == 4" class="badge badge-success">Chờ thông qua</span>
// <span *ngIf="document?.statusCode == 5" class="badge badge-warning">Ý kiến chỉnh sửa</span>
// <span *ngIf="document?.statusCode == 6" class="badge badge-danger">Không duyệt</span>
// <span *ngIf="document?.statusCode == 7" class="badge badge-secondary">Quá hạn</span>
// <span *ngIf="document?.statusCode == 8" class="badge badge-info"
//   >Đã thông qua - chờ ra nghị quyết</span
// >
// <span *ngIf="document?.statusCode == 9" class="badge badge-success">Đã phát hành</span>
// <span *ngIf="document?.statusCode == 10" class="badge badge-danger">Trả lại</span>
// <span *ngIf="document?.statusCode == 11" class="badge badge-danger">Đã ra nghị quyết</span>
// <span *ngIf="document?.statusCode == 12" class="badge badge-danger">Đã ký số</span>
