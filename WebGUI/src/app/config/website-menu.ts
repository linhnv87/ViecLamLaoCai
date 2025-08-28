import { ROLES } from '../utils/constants';

export interface WebsiteMenuModel {
  path?: string;
  text?: string;
  icon?: string;
  children?: WebsiteMenuModel[];
  permissions?: string[];
  isActive?: boolean;
  requiresVerification?: boolean;
}

export const websiteMenuItems: WebsiteMenuModel[] = [
  {
    path: '/website/dashboard',
    text: 'Quản lý chung',
    icon: 'assets/vieclamlaocai/img/icon/icon-setting.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.UNG_VIEN, ROLES.DOANH_NGHIEP, ROLES.CO_QUAN_QUAN_LY],
  },
  {
    path: '/website/find-candidates',
    text: 'Tìm ứng viên phù hợp',
    icon: 'assets/vieclamlaocai/img/icon/icon-search.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.DOANH_NGHIEP],
    requiresVerification: true,
  },
  {
    path: '/website/applications',
    text: 'Ứng viên',
    icon: 'assets/vieclamlaocai/img/icon/icon-group-user.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.DOANH_NGHIEP],
    requiresVerification: true,
  },
  {
    path: '/website/business-approval',
    text: 'Phê duyệt doanh nghiệp',
    icon: 'assets/vieclamlaocai/img/icon/icon-check-circle.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.CO_QUAN_QUAN_LY],
  },
  {
    path: '/website/statistics-report',
    text: 'Báo cáo thống kê',
    icon: 'assets/vieclamlaocai/img/icon/icon-docs.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.CO_QUAN_QUAN_LY],
  },
  {
    path: '/website/labor-market-report',
    text: 'Thị trường lao động',
    icon: 'assets/vieclamlaocai/img/icon/icon-color-bag.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.CO_QUAN_QUAN_LY],
  },
  {
    path: '/website/cv-management',
    text: 'Quản lý CV',
    icon: 'assets/vieclamlaocai/img/icon/icon-docs.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.UNG_VIEN],
  },
  {
    path: '/website/change-info',
    text: 'Thay đổi thông tin',
    icon: 'assets/vieclamlaocai/img/icon/icon-user-circle.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.UNG_VIEN, ROLES.DOANH_NGHIEP, ROLES.CO_QUAN_QUAN_LY], 
  },
  {
    path: '/website/labor-market-report1',
    text: 'Đăng tin tuyển dụng',
    icon: 'assets/vieclamlaocai/img/icon/icon-square-plus.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.DOANH_NGHIEP],
    requiresVerification: true,
  },
  {
    path: '/website/labor-market-report3',
    text: 'Tất cả tuyển dụng',
    icon: 'assets/vieclamlaocai/img/icon/icon-search-user.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.DOANH_NGHIEP],
    requiresVerification: true,
  },
  {
    path: '/website/labor-market-report2',
    text: 'Vị trí phỏng vấn',
    icon: 'assets/vieclamlaocai/img/icon/icon-meeting-user.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.DOANH_NGHIEP], 
  },
  {
    path: '/website/labor-market-repor',
    text: 'Lịch phỏng vấn',
    icon: 'assets/vieclamlaocai/img/icon/icon-calculator.svg',
    children: [],
    isActive: true,
    permissions: [ROLES.ADMIN, ROLES.DOANH_NGHIEP], 
  },
];

export function getActiveMenuItems(): WebsiteMenuModel[] {
  return websiteMenuItems.filter(item => item.isActive);
}

export function getMenuItemByPath(path: string): WebsiteMenuModel | undefined {
  return websiteMenuItems.find(item => item.path === path);
}

// lấy menu items dựa trên role của user
export function getMenuItemsByRole(userRoles: string[]): WebsiteMenuModel[] {
  if (!userRoles || userRoles.length === 0) {
    return [];
  }

  return websiteMenuItems.filter(item => {
    if (!item.permissions || item.permissions.length === 0) {
      return item.isActive;
    }
    const hasPermission = item.permissions.some(permission => 
      userRoles.includes(permission)
    );

    return hasPermission && item.isActive;
  });
}







