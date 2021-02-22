export default [
  {
    path: '/user',
    layout: false,
    routes: [
      {
        path: '/user',
        routes: [
          {
            name: 'login',
            path: '/user/login',
            component: './User/login',
          },
        ],
      },
    ],
  },
  {
    path: '/welcome',
    name: 'welcome',
    icon: 'smile',
    component: './Welcome',
  },
  {
    path: '/admin',
    name: 'admin',
    icon: 'crown',
    access: 'canAdmin',
    component: './Admin',
    routes: [
      {
        path: '/admin/sub-page',
        name: 'sub-page',
        icon: 'smile',
        component: './Welcome',
      },
    ],
  },
  {
    name: 'list.table-list',
    icon: 'table',
    path: '/list',
    component: './TableList',
  },
  {
    name: 'system',
    icon: 'table',
    path: '/sys',
    access: 'canAdmin',
    routes:[
      {
        path: '/sys/user',
        name: 'user-list',
        icon: 'smile',
        component: './system/user',
      },
      {
        path: '/sys/module',
        name: 'module-list',
        icon: 'smile',
        component: './system/module',
      },
      {
        path: '/sys/role',
        name: 'role-list',
        icon: 'smile',
        component: './system/role',
      },
    ]
  },
  {
    path: '/',
    redirect: '/welcome',
  },
  {
    component: './404',
  },
];
