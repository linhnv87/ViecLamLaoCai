import { ROLES } from '../utils/constants';

export interface MenuModel {
  path?: string;
  text?: string;
  icon?: string;
  children?: MenuModel[];
  permissions?: string[];
}
export const menuItems = [
  {
    path: '/admin/to-trinh/add',
    text: 'Thêm mới tờ trình',
    icon: 'fas fa-plus',
    children: [],
    permissions: [ROLES.CHUYEN_VIEN],
  },
  {
    path: '#hr',
    text: '',
    icon: '',
    children: [],
    permissions: [ROLES.CHUYEN_VIEN],
  },
  {
    path: '/admin/to-trinh/tong-to-trinh',
    text: 'Tổng tờ trình',
    icon: 'fas fa-share-alt-square',
    children: [],
  },
  {
    path: '/admin/to-trinh/du-thao',
    text: 'Dự thảo', // Chờ xử lý 3 - Quá hạn 7 - Chờ ra nghị quyết (4, 8)
    icon: 'fas fa-share-alt-square',
    children: [],
    permissions: [ROLES.CHUYEN_VIEN],
  },
  {
    path: '/admin/to-trinh/xin-y-kien',
    text: 'Xin ý kiến',
    icon: 'fas fa-retweet',
    children: [],
  },
  {
    path: '/admin/to-trinh/xin-y-kien-lai',
    text: 'Xin ý kiến lại',
    icon: 'fas fa-retweet',
    children: [],
    permissions: [ROLES.CHUYEN_VIEN],
  },
  {
    path: '/admin/to-trinh/phe-duyet',
    text: 'Phê duyệt',
    icon: 'far fa-check-circle',
    children: [],
  },
  {
    path: '/admin/to-trinh/ky-so',
    text: 'Chờ ký số',
    icon: 'fas fa-clipboard-check',
    children: [],
  },
  {
    path: '/admin/to-trinh/cho-ban-hanh',
    text: 'Chờ ban hành',
    icon: 'fas fa-clipboard-check',
    children: [],
  },
  {
    path: '/admin/to-trinh/ban-hanh',
    text: 'Ban hành',
    icon: 'far fa-calendar-check',
    children: [],
  },
  {
    path: '/admin/to-trinh/khong-ban-hanh',
    text: 'Không ban hành',
    icon: 'fas fa-undo-alt',
    children: [],
  },
  // {
  //   path: '/admin/to-trinh/tra-lai',
  //   text: 'Trả lại',
  //   icon: 'fas fa-angle-double-left',
  //   children: [],
  // },
  {
    path: '#hr',
    text: '',
    icon: '',
    children: [],
  },
  // ,
  // {
  //   path: '/admin/to-trinh/0/approval-summary/0',
  //   text: 'Báo cáo thống kê',
  //   icon: 'fas fa-history',
  //   children: []
  // },
  // {
  //   path: '/admin/history',
  //   text: 'Lịch sử thu hồi',
  //   icon: 'fas fa-history',
  //   children: []
  // }
];

export const AdminMenu = [
  {
    path: '/admin/field',
    text: 'Quản lí lĩnh vực',
    icon: 'fas fa-layer-group',
    children: [],
  },
  {
    path: '/admin/document-type',
    text: 'Quản lí loại văn bản',
    icon: 'fas fa-file-alt',
    children: [],
  },
  {
    path: '/admin/department',
    text: 'Quản lí phòng ban',
    icon: 'fas fa-building',
    children: [],
  },
  {
    path: '/admin/unit',
    text: 'Quản lí đơn vị',
    icon: 'fas fa-archive',
    children: [],
  },
  {
    path: '/admin/role',
    text: 'Quản lí vai trò',
    icon: 'fas fa-tag',
    children: [],
  },
  {
    path: '/admin/User',
    text: 'Quản lí người dùng',
    icon: 'fa-solid fa-user',
    children: [],
  },
  {
    path: '/admin/group',
    text: 'Quản lí nhóm',
    icon: 'fas fa-layer-group',
    children: [],
  },
  {
    path: '/admin/work-flow',
    text: 'Quản lí luồng xử lý',
    icon: 'fa-solid fa-stream',
    children: [],
  },
  {
    path: '/admin/track-message-result',
    text: 'Kết quả gửi tin nhắn',
    icon: 'fa-solid fa-square-poll-horizontal',
    children: [],
  },
];

export const SystemMenu = [
  {
    path: '/system/quan-ly-co-quan',
    text: 'quản lý cơ quan phòng ban đơn vị',
    icon: 'fa-solid fa-building',
    children: [],
  },
  {
    path: '/system/quan-ly-chuc-danh',
    text: 'quản lý chức danh',
    icon: 'fa-solid fa-user-tie',
    children: [],
  },
  {
    path: '/system/nguoi-dung-don-vi',
    text: 'người dùng đơn vị',
    icon: 'fa-solid fa-users',
    children: [],
  },
  {
    path: '/system/cau-hinh-quyen-han',
    text: 'quản trị cấu hình quyền hạn',
    icon: 'fa-solid fa-cogs',
    children: [],
  },
  {
    path: '/system/phan-quyen-nguoi-dung',
    text: 'phân quyền người dùng - nhóm',
    icon: 'fa-solid fa-user-shield',
    children: [],
  },
  {
    path: '/system/dinh-nghia-quyen',
    text: 'định nghĩa quyền và chức năng',
    icon: 'fa-solid fa-list-check',
    children: [],
  },
  {
    path: '/system/cau-hinh-luong',
    text: 'cấu hình luồng xử lý động',
    icon: 'fa-solid fa-gears',
    children: [],
  },
  {
    path: '/system/cau-hinh-tin-nhan',
    text: 'cấu hình tin nhắn nhắc',
    icon: 'fa-solid fa-bell',
    children: [],
  },
];
