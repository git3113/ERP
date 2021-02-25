// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

import { MenuDataItem } from '@umijs/route-utils';
/** 获取当前的用户 GET /api/currentUser */
export async function currentUser<T>(options?: { [key: string]: any }) {
  return request<API.CommonResult<T>>('/api/User/GetCurrentUser', {
    method: 'GET',
    ...(options || {}),
  });
}

/** 获取当前的用户 GET /api/currentUser */
export async function getMenuData<T>(options?: { [key: string]: any }) {
  return request<API.CommonResult<T>>('/api/System/GetMenuData', {
    method: 'GET',
    ...(options || {}),
  });
}

/** 此处后端没有提供注释 GET /api/notices */
export async function getNotices(options?: { [key: string]: any }) {
  return request<API.NoticeIconList>('/api/notices', {
    method: 'GET',
    ...(options || {}),
  });
}
