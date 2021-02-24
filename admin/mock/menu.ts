import { Request, Response } from 'express';

// 代码中会兼容本地 service mock 以及部署站点的静态数据
export default {
  // 支持值为 Object 和 Array
  'GET /api/getMenuData': (req: Request, res: Response) => {
    res.send([
      {
        key: '1',
        name: 'home',
        icon: 'icon-shoucang1',
        path: '/welcome',
      },
      {
        key: '2',
        name: 'system',
        path: '/system',
        icon: 'icon-shoucang1',
        children:[
          {
            key: '21',
            name: 'user-list',
            icon: 'icon-shoucang1',
            path: '/system/user'
          },
          {
            key: '22',
            name: 'module-list',
            icon: 'icon-shoucang1',
            path: '/system/module'
          },
          {
            key: '23',
            name: 'role-list',
            icon: 'icon-shoucang1',
            path: '/system/role'
          }
        ]
      },
      {
        key: '3',
        path: '/admin',
        name: 'admin',
        icon: 'icon-shoucang1',
        access: 'canAdmin',
        children: [
          {
            key: '31',
            path: '/admin/sub-page',
            name: 'sub-page',
            icon: 'icon-shoucang1',
          },
        ],
      },
      {
        key: '4',
        name: 'list.table-list',
        icon: 'icon-shoucang1',
        path: '/list'
      },
    ]);
  },
};
