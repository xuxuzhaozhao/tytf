<template>
  <div id="admin">
    <v-app id="inspire">
    <v-navigation-drawer
      :clipped="$vuetify.breakpoint.lgAndUp"
      v-model="drawer"
      fixed
      app
    >
      <v-list dense>
        <template v-for="item in items">
          <v-layout
            v-if="item.heading"
            :key="item.heading"
            row
            align-center
          >
            <v-flex xs6>
              <v-subheader v-if="item.heading">
                {{ item.heading }}
              </v-subheader>
            </v-flex>
            <v-flex xs6 class="text-xs-center">
              <a href="#!" class="body-2 black--text">EDIT</a>
            </v-flex>
          </v-layout>
          <v-list-group
            v-else-if="item.children"
            v-model="item.model"
            :key="item.text"
            :prepend-icon="item.model ? item.icon : item['icon-alt']"
            append-icon=""
          >
            <v-list-tile slot="activator">
              <v-list-tile-content>
                <v-list-tile-title>
                  <router-link :to="item.link">
                    {{ item.text }}
                  </router-link>
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile
              v-for="(child, i) in item.children"
              :key="i"
              @click=""
            >
              <v-list-tile-action v-if="child.icon">
                <v-icon>{{ child.icon }}</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>
                  {{ child.text }}
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
          </v-list-group>
          <v-list-tile v-else :key="item.text" @click="">
            <v-list-tile-action>
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>
                <router-link :to="item.link">
                 {{ item.text }}
                </router-link>
              </v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
        </template>
      </v-list>
    </v-navigation-drawer>
    <v-toolbar
      :clipped-left="$vuetify.breakpoint.lgAndUp"
      color="blue darken-3"
      dark
      app
      fixed
    >
      <v-toolbar-title style="width: 300px" class="ml-0 pl-3">
        <v-toolbar-side-icon @click.stop="drawer = !drawer"></v-toolbar-side-icon>
        <span class="hidden-sm-and-down">田园天府后台管理</span>
      </v-toolbar-title>
      
      <v-spacer></v-spacer>
      <v-btn icon @click="reload">
        <v-icon>notifications</v-icon>
      </v-btn>
      <v-btn icon large>
        <v-avatar size="32px" tile>
          <img
            src="/static/img/header.jpg"
            alt="徐老板"
          >
        </v-avatar>
      </v-btn>
      <v-btn @click="logout">退出登录</v-btn>
    </v-toolbar>
    <v-content>
      <keep-alive>
        <router-view v-if="isRouterAlive"/>
      </keep-alive>
    </v-content>
  </v-app>
  </div>
</template>

<script>
export default {
  name: "Admin",
  data: () => ({
    isRouterAlive:true,
    dialog: false,
    drawer: null,
    items: [
      { icon: "keyboard", text: "电脑端订单管理", link: "/admin/order" },
      { icon: "keyboard", text: "移动端订单管理", link: "/admin/mobileorder" },
      { icon: "add", text: "餐桌位置管理", link: "/admin/position" },
      { icon: "content_copy", text: "菜品种类管理", link: "/admin/menutype" },
      { icon: "history", text: "菜品管理", link: "/admin/menu" },
      { icon: "contacts", text: "服务人员管理", link: "/admin/people" }
    ]
  }),
  methods: {
    logout() {
      this.$store.commit("logout");
      this.$router.push({
        path: "/login"
      });
    },
    reload() {
      this.$router.go(0)
    }

  }
};
</script>

<style>
a:link {
  color: rgb(253, 7, 7);
  text-decoration: none;
}

a:active {
  color: red;
}
</style>
