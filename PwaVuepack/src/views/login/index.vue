
  <template>
  <v-layout row>
    <v-flex xs12 sm6 offset-sm3>
      <v-card>
        <v-card-media
          src="/static/img/tytf.jpg"
          height="200px"
        >
        </v-card-media>
          <v-container>
            <v-layout column>
              <v-flex xl6 xs3 sm3>
                <v-text-field solo label="Name" v-model="user.username"></v-text-field>
              </v-flex>
              <v-flex xl6 xs3 sm3>
                <v-text-field type="password" solo-inverted label="Password" v-model="user.password"></v-text-field>
              </v-flex>
              <v-flex xl6 xs3 sm3>
                <v-spacer></v-spacer>
                <v-btn dark color="pink" @click="login">Login</v-btn>
              </v-flex>
            </v-layout>
          </v-container>
      </v-card>
    </v-flex>
    <xsnackbar :color="color" 
               :snackbar.sync="snackbarB" 
               :text="message"/>
  </v-layout>
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
      show: false,
      user: {
        username: "",
        password: ""
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
