export const LOCAL_STORAGE_KEYS = {
  USER_INFO: 'UserInfo',
};

export const DATE_FORMAT = {
  FULL_DATE_TO_BE: 'YYYY-MM-DD HH:mm:ss',
  FULL_DATE_TO_SHOW: 'HH:mm dd/MM/yyyy',
};

export const ROLES = {
  ADMIN: 'Admin',
  CHUYEN_VIEN: 'chuyen-vien',
  BAN_CHAP_HANH: 'ban-chap-hanh',
  BAN_THUONG_VU: 'ban-thuong-vu',
  TRUONG_PHONG: 'truong-phong',
  PHO_TRUONG_PHONG: 'pho-truong-phong',
  PHO_CHANH_VAN_PHONG: 'pho-chanh-van-phong',
  CHANH_VAN_PHONG: 'chanh-van-phong',
  PHO_BI_THU: 'pho-bi-thu',
  BI_THU: 'bi-thu',
  VAN_THU: 'van-thu',
};
export const ROLE_OPTIONS = [
  {
    value: ROLES.CHUYEN_VIEN,
    text: 'Chuyên viên',
  },
  { value: ROLES.BAN_CHAP_HANH, text: 'Ban chấp hành' },
  { value: ROLES.BAN_THUONG_VU, text: 'Ban thường vụ' },
  { value: ROLES.TRUONG_PHONG, text: 'Trưởng phòng' },
  { value: ROLES.PHO_CHANH_VAN_PHONG, text: 'Phó chánh văn phòng' },
  { value: ROLES.CHANH_VAN_PHONG, text: 'Chánh văn phòng' },
  { value: ROLES.PHO_BI_THU, text: 'Phó bí thư' },
  { value: ROLES.BI_THU, text: 'Bí thư' },
  { value: ROLES.VAN_THU, text: 'Văn thư' },
];

export const DOCUMENT_STATUS = {
  DU_THAO: 'du-thao',
  XIN_Y_KIEN: 'xin-y-kien',
  XIN_Y_KIEN_LAI: 'xin-y-kien-lai',
  PHE_DUYET: 'phe-duyet',
  KY_SO: 'ky-so',
  CHO_BAN_HANH: 'cho-ban-hanh',
  BAN_HANH: 'ban-hanh',
  KHONG_BAN_HANH: 'khong-ban-hanh',
  TRA_LAI: 'tra-lai',
};

export const ACTION_STATUS = {
  TRA_VE: 'tra-ve',
  XIN_Y_KIEN_LAI: 'xin-y-kien-lai',
  CHUYEN_XU_LY: 'chuyen-xu-ly',
  CHUYEN_KY_SO: 'chuyen-ky-so',
};

export const DOCUMENT_STATUS_OPTIONS = [
  {
    value: '',
    text: 'Tất cả',
  },
  {
    value: DOCUMENT_STATUS.DU_THAO,
    text: 'Dự thảo',
  },
  {
    value: DOCUMENT_STATUS.XIN_Y_KIEN,
    text: 'Xin ý kiến',
  },
  {
    value: DOCUMENT_STATUS.XIN_Y_KIEN,
    text: 'Xin ý kiến lại',
  },
  {
    value: DOCUMENT_STATUS.PHE_DUYET,
    text: 'Phê duyệt',
  },
  {
    value: DOCUMENT_STATUS.KY_SO,
    text: 'Ký số',
  },
  {
    value: DOCUMENT_STATUS.CHO_BAN_HANH,
    text: 'Chờ ban hành',
  },
  {
    value: DOCUMENT_STATUS.BAN_HANH,
    text: 'Ban hành',
  },
  {
    value: DOCUMENT_STATUS.KHONG_BAN_HANH,
    text: 'Không ban hành',
  },
  {
    value: DOCUMENT_STATUS.TRA_LAI,
    text: 'Trả lại',
  },
];

const currentYear = new Date().getFullYear();
export const YEAR_OPTIONS = [
  {
    id: 0,
    text: 'Tất cả',
  },
  {
    id: currentYear,
    text: currentYear,
  },
  {
    id: currentYear - 1,
    text: currentYear - 1,
  },
  {
    id: currentYear - 2,
    text: currentYear - 2,
  },
  {
    id: currentYear - 3,
    text: currentYear - 3,
  },
  {
    id: currentYear - 4,
    text: currentYear - 4,
  },
];

export const MONTH_OPTIONS = [
  {
    id: 0,
    text: 'Tất cả',
  },
  {
    id: 1,
    text: 1,
  },
  {
    id: 2,
    text: 2,
  },
  {
    id: 3,
    text: 3,
  },
  {
    id: 4,
    text: 4,
  },
  {
    id: 5,
    text: 5,
  },
  {
    id: 6,
    text: 6,
  },
  {
    id: 7,
    text: 7,
  },
  {
    id: 8,
    text: 8,
  },
  {
    id: 9,
    text: 9,
  },
  {
    id: 10,
    text: 10,
  },
  {
    id: 11,
    text: 11,
  },
  {
    id: 12,
    text: 12,
  },
];
