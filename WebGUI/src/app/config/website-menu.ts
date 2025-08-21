import { ROLES } from '../utils/constants';

export interface WebsiteMenuModel {
  path?: string;
  text?: string;
  icon?: string;
  children?: WebsiteMenuModel[];
  permissions?: string[];
  isActive?: boolean;
}

export const websiteMenuItems: WebsiteMenuModel[] = [
  {
    path: '/website/dashboard',
    text: 'Quản lý chung',
    icon: 'assets/vieclamlaocai/img/icon/icon-setting.svg',
    children: [],
    isActive: true,
  },
  {
    path: '/website/find-candidates',
    text: 'Tìm ứng viên phù hợp',
    icon: 'assets/vieclamlaocai/img/icon/icon-search.svg',
    children: [],
    isActive: true,
  },
  {
    path: '/website/applications',
    text: 'Ứng viên',
    icon: 'assets/vieclamlaocai/img/icon/icon-group-user.svg',
    children: [],
    isActive: true,
  },
  {
    path: '/website/business-approval',
    text: 'Phê duyệt doanh nghiệp',
    icon: 'assets/vieclamlaocai/img/icon/icon-check-circle.svg',
    children: [],
    isActive: true,
  },
  {
    path: '/website/statistics-report',
    text: 'Báo cáo thống kê',
    icon: 'assets/vieclamlaocai/img/icon/icon-docs.svg',
    children: [],
    isActive: true,
  },
  {
    path: '/website/labor-market-report',
    text: 'Thị trường lao động',
    icon: 'assets/vieclamlaocai/img/icon/icon-color-bag.svg',
    children: [],
    isActive: true,
  },
  {
    path: '/website/labor-market-report6',
    text: 'Thay đổi thông tin',
    icon: 'assets/vieclamlaocai/img/icon/icon-user-circle.svg',
    children: [],
    isActive: true,
  },
  {
    path: '/website/labor-market-report1',
    text: 'Đăng tin tuyển dụng',
    icon: 'assets/vieclamlaocai/img/icon/icon-square-plus.svg',
    children: [],
    isActive: true,
  },
  {
    path: '/website/labor-market-report3',
    text: 'Tất cả tuyển dụng',
    icon: 'assets/vieclamlaocai/img/icon/icon-search-user.svg',
    children: [],
    isActive: true,
  },
  {
    path: '/website/labor-market-report2',
    text: 'Vị trí phỏng vấn',
    icon: 'assets/vieclamlaocai/img/icon/icon-meeting-user.svg',
    children: [],
    isActive: true,
  },
  {
    path: '/website/labor-market-repor',
    text: 'Lịch phỏng vấn',
    icon: 'assets/vieclamlaocai/img/icon/icon-calculator.svg',
    children: [],
    isActive: true,
  },
];

export function getActiveMenuItems(): WebsiteMenuModel[] {
  return websiteMenuItems.filter(item => item.isActive);
}

export function getMenuItemByPath(path: string): WebsiteMenuModel | undefined {
  return websiteMenuItems.find(item => item.path === path);
}







