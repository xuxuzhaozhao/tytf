// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import store from './utils/store'
import Vuetify from 'vuetify'

import VCharts from 'v-charts'
import axios from 'axios'
import 'vuetify/dist/vuetify.min.css'

import VueAMap from 'vue-amap'
import VuetifyConfirm from 'vuetify-confirm'

Vue.use(Vuetify)
Vue.use(VCharts)
Vue.use(VuetifyConfirm)

Vue.use(VueAMap)
VueAMap.initAMapApiLoader({
  key: '787278a673718516fc65d8c28d2c4f40',
  plugin: ['AMap.Autocomplete','AMap.Geolocation','AMap.PlaceSearch', 'AMap.Scale', 'AMap.OverView', 'AMap.ToolBar', 'AMap.MapType', 'AMap.PolyEditor', 'AMap.CircleEditor'],
  // 默认高德 sdk 版本为 1.4.4
  v: '1.4.4'
});


Vue.config.productionTip = false
Vue.prototype.$http = axios
Vue.prototype.$domain = '';

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  store,
  components: {
    App
  },
  template: '<App/>'
})
