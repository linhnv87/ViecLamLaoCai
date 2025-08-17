import { Pipe, PipeTransform } from '@angular/core';
import { DOCUMENT_STATUS } from 'src/app/utils/constants';

@Pipe({
  name: 'getTitleByStatus',
})
export class GetTitleByStatusPipe implements PipeTransform {
  transform(value?: number | string | null, prefix?: string): string {
    let title = '';
    switch (value) {
      case DOCUMENT_STATUS.DU_THAO:
        title = 'Dự thảo';
        break;
      case DOCUMENT_STATUS.XIN_Y_KIEN:
        title = 'Xin ý kiến';
        break;
      case DOCUMENT_STATUS.XIN_Y_KIEN_LAI:
        title = 'Xin ý kiến lại';
        break;
      case DOCUMENT_STATUS.PHE_DUYET:
        title = 'Phê duyệt';
        break;
      case DOCUMENT_STATUS.KY_SO:
        title = 'Ký số';
        break;
      case DOCUMENT_STATUS.CHO_BAN_HANH:
        title = 'Chờ ban hành';
        break;
      case DOCUMENT_STATUS.BAN_HANH:
        title = 'Ban hành';
        break;
      case DOCUMENT_STATUS.KHONG_BAN_HANH:
        title = 'Không ban hành';
        break;
      case DOCUMENT_STATUS.TRA_LAI:
        title = 'Trả lại';
        break;
      default:
        title = '';
        break;
    }
    if (title) {
      return prefix ? `${prefix} ${title.toLocaleLowerCase()}` : title;
    }
    return prefix ? 'Tất cả tờ trình' : 'Tất cả';
  }
}
