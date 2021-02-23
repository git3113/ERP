import { Request, Response } from 'express';

// 代码中会兼容本地 service mock 以及部署站点的静态数据
export default {
  // 支持值为 Object 和 Array
  'GET /api/getMenuData': (req: Request, res: Response) => {
    res.send([
      {
        key: '1',
        name: 'home',
        icon: 'smile',
        path: '/welcome',
      },
      {
        key: '2',
        name: 'system',
        path: '/sys',
        icon: 'smile',
        children:[
          {
            key: '21',
            name: 'user-list',
            icon: 'smile',
            path: '/sys/user'
          },
          {
            key: '22',
            name: 'module-list',
            icon: 'smile',
            path: '/sys/module'
          },
          {
            key: '23',
            name: 'role-list',
            icon: 'smile',
            path: '/list'
          }
        ]
      },
      {
        key: '3',
        path: '/admin',
        name: 'admin',
        icon: 'smile',
        access: 'canAdmin',
        children: [
          {
            key: '31',
            path: '/admin/sub-page',
            name: 'sub-page',
            icon: 'smile',
          },
        ],
      },
      {
        key: '4',
        name: 'list.table-list',
        icon: 'smile',
        path: '/list'
      },
    ]);
  },
};
