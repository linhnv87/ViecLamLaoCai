import { environment } from 'src/environments/environments';
import { LOCAL_STORAGE_KEYS, ROLES } from './constants';
import { SimplerRoleTempModel } from '../models/user.model';

export const GetCurrentUserId = () => {
  const userInfo = localStorage.getItem(LOCAL_STORAGE_KEYS.USER_INFO);
  if (!!userInfo) return JSON.parse(userInfo).userId;
  return 0;
};

export const GetCurrentUserInfo = () => {
  const userInfo = localStorage.getItem(LOCAL_STORAGE_KEYS.USER_INFO);
  if (!!userInfo) return JSON.parse(userInfo);
  return 0;
};

export const GetCurrentUserRoles = (): string[] => {
  const userInfo = localStorage.getItem(LOCAL_STORAGE_KEYS.USER_INFO);
  if (!!userInfo) return JSON.parse(userInfo)?.roles;
  return [];
};

export interface RoleInfo {
  admin: boolean;
  chuyenVien: boolean;
  banChapHanh: boolean;
  banThuongVu: boolean;
  truongPhong: boolean;
  photruongPhong: boolean;
  phoChanhVanPhong: boolean;
  chanhVanPhong: boolean;
  phoBiThu: boolean;
  biThu: boolean;
  vanThu: boolean;
}

export const GetRoleInfo = () => {
  const currentRoles = GetCurrentUserRoles();
  if (currentRoles?.length) {
    const roleInfo: RoleInfo = {
      admin: currentRoles.includes(ROLES.ADMIN),
      chuyenVien: currentRoles.includes(ROLES.CHUYEN_VIEN),
      banChapHanh: currentRoles.includes(ROLES.BAN_CHAP_HANH),
      banThuongVu: currentRoles.includes(ROLES.BAN_THUONG_VU),
      truongPhong: currentRoles.includes(ROLES.TRUONG_PHONG),
      photruongPhong: currentRoles.includes(ROLES.PHO_TRUONG_PHONG),
      phoChanhVanPhong: currentRoles.includes(ROLES.PHO_CHANH_VAN_PHONG),
      chanhVanPhong: currentRoles.includes(ROLES.CHANH_VAN_PHONG),
      phoBiThu: currentRoles.includes(ROLES.PHO_BI_THU),
      biThu: currentRoles.includes(ROLES.BI_THU),
      vanThu: currentRoles.includes(ROLES.VAN_THU),
    };
    return roleInfo;
  }
  return {
    admin: false,
    chuyenVien: false,
    banChapHanh: false,
    banThuongVu: false,
    truongPhong: false,
    phoChanhVanPhong: false,
    chanhVanPhong: false,
    phoBiThu: false,
    biThu: false,
    vanThu: false,
  };
};

export const GetFullFilePath = (relativePath: string) => {
  return environment.hostUrl + relativePath;
};

export const GetFullFilePathSign = (relativePath: string) => {
  return environment.vgca_Url + relativePath;
};

export const fillMissingVersions = (docVersions: any[]) => {
  const filledVersions: any[] = [];

  if (docVersions.length === 0) {
    return filledVersions;
  }

  // Sort versions in ascending order
  const sortedVersions = docVersions
    .slice()
    .sort((a, b) => parseInt(a.version) - parseInt(b.version));

  // Iterate through sorted versions to fill the gaps
  for (let i = 0; i < sortedVersions.length - 1; i++) {
    filledVersions.push(sortedVersions[i]);

    const currentVersion = parseInt(sortedVersions[i].version);
    const nextVersion = parseInt(sortedVersions[i + 1].version);

    // Fill the gap between versions with empty data array
    for (let version = currentVersion + 1; version < nextVersion; version++) {
      filledVersions.push({
        version: version.toString(),
        data: [],
        show: false,
      });
    }
  }

  // Add the last version to the result
  filledVersions.push(sortedVersions[sortedVersions.length - 1]);

  return filledVersions;
};

export const fillMissingAttempts = (docVersions: any[]) => {
  const filledVersions: any[] = [];

  if (docVersions.length === 0) {
    return filledVersions;
  }

  // Sort versions in ascending order
  const sortedVersions = docVersions
    .slice()
    .sort((a, b) => parseInt(a.submitCount) - parseInt(b.submitCount));

  // Iterate through sorted versions to fill the gaps
  for (let i = 0; i < sortedVersions.length - 1; i++) {
    filledVersions.push(sortedVersions[i]);

    const currentVersion = parseInt(sortedVersions[i].submitCount);
    const nextVersion = parseInt(sortedVersions[i + 1].submitCount);

    // Fill the gap between versions with empty data array
    for (let submitCount = currentVersion + 1; submitCount < nextVersion; submitCount++) {
      filledVersions.push({
        submitCount: submitCount.toString(),
        data: [],
        show: false,
      });
    }
  }

  // Add the last version to the result
  filledVersions.push(sortedVersions[sortedVersions.length - 1]);

  return filledVersions;
};

export function displayUserRoles(
  roles?: SimplerRoleTempModel[],
  isHideRoundBrackets?: boolean,
): string {
  const roleVales = roles?.map(role => role.description).join(', ') || '';
  if (roleVales) {
    return isHideRoundBrackets ? roleVales : `(${roleVales})`;
  }
  return '';
}
