import Vue from 'vue'
import Router from 'vue-router'
import store from '@/utils/store/index'
import * as types from '@/utils/store/types'

import order from '@/views/order/index'
import menu from '@/views/menu/index'
import menuType from '@/views/menu/type'
import people from '@/views/people/index'
import position from '@/views/position/index'
import Login from '@/views/login/index'
import Admin from '@/views/admin/index'
import Chart from "@/views/charts/index";
import MobileOrder from '@/views/mobileorder/index'
import Amap from '@/views/map/index'

Vue.use(Router)

const router = new Router({
  routes: [{
      path: '/',
      name: 'Admin',
      component: Admin,
      redirect: '/admin/mobileorder',
      meta: {
        requireAuth: true
      },
      children: [{
          path: '/amap',
          name: 'Amap',
          component: Amap
        },
        {
          path: '/admin/mobileorder',
          name: 'MobileOrder',
          component: MobileOrder
        },
        {
          path: '/admin/order',
          name: 'Order',
          component: order
        },
        {
          path: '/admin/menu',
          name: 'menu',
          component: menu
        },
        {
          path: '/admin/menutype',
          name: 'menuType',
          component: menuType
        },
        {
          path: '/admin/people',
          name: 'people',
          component: people
        },
        {
          path: '/admin/position',
          name: 'position',
          component: position
        }
      ]
    },
    {
      path: '/login',
      name: 'login',
      component: Login
    }, {
      path: '/chart',
      name: 'chart',
      component: Chart
    }
  ]
})

// 页面刷新时，重新赋值token
if (window.localStorage.getItem('token')) {
  store.commit(types.LOGIN, window.localStorage.getItem('token'))
}

router.beforeEach((to, from, next) => {
  if (to.matched.some(r => r.meta.requireAuth)) {
    if (store.state.token) {
      //   if (IsPC()) {
      //     next({
      //       path: '/admin/order'
      //     })
      //   } else {
      //     next({
      //       path: '/admin/mobileorder'
      //     })
      next();
    } else {
      next({
        path: '/login',
        query: {
          redirect: to.fullPath
        }
      })
    }
  } else {
    next();
  }
})


function IsPC() {
  var userAgentInfo = navigator.userAgent;
  var Agents = new Array("Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod");
  var flag = true;
  for (var v = 0; v < Agents.length; v++) {
    if (userAgentInfo.indexOf(Agents[v]) > 0) {
      flag = false;
      break;
    }
  }
  return flag;
}

export default router
