<template>
  <v-content style="margin-top:48%;">
      <xsnackbar :color="color" 
               :snackbar.sync="snackbarB" 
               :text="message"/>
      <v-container fluid fill-height>
        <v-layout align-center justify-center>
          <v-flex xs12 sm8 md4>
            <v-card class="elevation-12">
              <v-toolbar dark color="primary">
                <v-toolbar-title>田园天府 - 订单管理后台登录</v-toolbar-title>
              </v-toolbar>
              <v-card-text>
                <v-form>
                  <v-text-field prepend-icon="person" name="login" label="Login" type="text" v-model="user.username"></v-text-field>
                  <v-text-field id="password" prepend-icon="lock" name="password" label="Password" type="password" v-model="user.password"></v-text-field>
                </v-form>
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn dark color="pink" @click="login">登录</v-btn>
              </v-card-actions>
            </v-card>
          </v-flex>
        </v-layout>
      </v-container>
    </v-content>
</template>

<script>
import Xsnackbar from "@/components/snackbar";
export default {
  components: {
    Xsnackbar
  },
  data() {
    return {
      color: "error",
      message: "操作成功！",
      snackbarB: false,
      user: {
        username: null,
        password: null
      }
    };
  },
  methods: {
    login() {
      this.$http.post(`${this.$domain}/api/User/login`, this.user).then(res => {
        if (res.data.code === 20000) {
          this.$store.commit("login", res.data.data);
          let redirect = decodeURIComponent(this.$route.query.redirect || "/");
          this.$router.push({
            path: redirect
          });
        } else {
          this.message = "用户名或密码错误！";
          this.snackbarB = true;
        }
      });
    }
  }
};
</script>

<style>
</style>
