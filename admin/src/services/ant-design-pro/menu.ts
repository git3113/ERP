
import { request } from 'umi';

/** 获取当前的用户的菜单 GET /api/currentUser */
export async function getmenu(options?: { [key: string]: any }) {
  return request<API.MenuItem>('/api/System/GetModuleData', {
    method: 'GET',
    ...(options || {}),
  });
}

