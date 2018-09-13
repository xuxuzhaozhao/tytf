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

Vue.use(Vuetify)
Vue.use(VCharts)

Vue.config.productionTip = false
Vue.prototype.$http = axios
Vue.prototype.$domain= '';

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  store,
  components: { App },
  template: '<App/>'
})
