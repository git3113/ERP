import { Request, Response } from 'express';

// 代码中会兼容本地 service mock 以及部署站点的静态数据
export default {
  // 支持值为 Object 和 Array
  'GET /api/getmenu': (req: Request, res: Response) => {
    res.send([
      {
        Id: '1',
        Name: '首页',
        Path: '/'
      },
      {
        Id: '2',
        Name: '系统设置',
        Path: '/system',
        Children:[
          {
            Id: '21',
            Name: '用户管理',
            Path: '/system/user'
          },
          {
            Id: '22',
            Name: '角色管理',
            Path: 'system/role'
          }
        ]
      }
    ]);
  },
};
