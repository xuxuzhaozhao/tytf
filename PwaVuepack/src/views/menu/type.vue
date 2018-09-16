<template>
  <div id="menutype">
    <v-toolbar flat color="white">
      <v-toolbar-title>菜品种类管理</v-toolbar-title>
      <v-divider
        class="mx-2"
        inset
        vertical
      ></v-divider>
      <v-spacer></v-spacer>
      <v-dialog v-model="dialog" max-width="500px">
        <v-btn slot="activator" color="primary" dark class="mb-2">添加菜品种类</v-btn>
        <v-card>
          <v-card-title>
            <span class="headline">{{ formTitle }}</span>
          </v-card-title>

          <v-card-text>
            <v-container grid-list-md>
              <v-layout wrap>
                <v-flex xs12 sm6 md6>
                  <v-text-field v-model="editedItem.Name" label="菜品种类名称"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md6>
                  <v-text-field v-model="editedItem.Sort" label="菜品种类序号"></v-text-field>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" flat @click.native="close">取消</v-btn>
            <v-btn color="blue darken-1" flat @click.native="save">保存</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-toolbar>
    <v-data-table
      :headers="headers"
      :items="desserts"
      hide-actions
      class="elevation-1"
    >
      <template slot="items" slot-scope="props">
        <td>{{ props.item.Name }}</td>
        <td>{{ props.item.Sort }}</td>
        <td>
          <v-icon
            @click="editItem(props.item)"
          >
            edit
          </v-icon>
        </td>
        <td>
            <v-icon
                @click="deleteItem(props.item)"
                style="color:red;"
            >
                delete
            </v-icon>
        </td>
      </template>
      <template slot="no-data">
        <v-btn color="primary" @click="initialize">Reset</v-btn>
      </template>
    </v-data-table>
    <xsnackbar :color="color" 
               :snackbar.sync="snackbarB" 
               :text="message"/>
  </div>
</template>

<script>
import Xsnackbar from "@/components/snackbar";
export default {
  components: {
    Xsnackbar
  },
  data: () => ({
    color: "success",
    message: "操作成功！",
    snackbarB: false,
    dialog: false,
    headers: [
      { text: "菜品种类名称", value: "Name", Sortable: false },
      { text: "排序", value: "Sort" },
      { text: "编辑", sortable: false },
      { text: "删除", sortable: false }
    ],
    desserts: [],
    editedIndex: -1,
    editedItem: {
      Name: "",
      Sort: 0
    },
    defaultItem: {
      Name: "",
      Sort: 0
    }
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "添加新菜品种类" : "修改此菜品种类";
    }
  },

  watch: {
    dialog(val) {
      val || this.close();
    }
  },

  created() {
    this.initialize();
  },

  methods: {
    initialize() {
      this.$http
        .get(`${this.$domain}/api/BaseTable/MenuType/getlist`)
        .then(res => {
          if (res.data.code === 20000) {
            this.desserts = res.data.data;
          } else {
           this.color = "error";
            this.message = res.data.message;
            this.snackbarB = true;
          }
        });
    },

    // snackbarop(){
    //   this.snackbar=
    // },

    editItem(item) {
      this.editedIndex = this.desserts.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialog = true;
    },

    deleteItem(item) {
      const index = this.desserts.indexOf(item);
      this.$confirm("确定删除此菜品种类吗?", {
        buttonTrueText: "确定",
        buttonFalseText: "返回"
      }).then(res => {
        if (res) {
          this.desserts.splice(index, 1);
          this.$http
            .delete(`${this.$domain}/api/BaseTable/MenuType/del/${item.Id}`)
            .then(res => {
              if (res.data.code === 20000) {
                this.color = "success";
              } else {
                this.color = "error";
              }
              this.message = res.data.message;
              this.snackbarB = true;
            });
        }
      });
    },

    close() {
      this.dialog = false;
      setTimeout(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      }, 300);
    },

    save() {
      this.$http
        .post(`${this.$domain}/api/BaseTable/MenuType/save`, this.editedItem)
        .then(res => {
          if (res.data.code === 20000) {
            this.color = "success";

            if (this.editedIndex > -1) {
              Object.assign(this.desserts[this.editedIndex], this.editedItem);
            } else {
              this.desserts.push(this.editedItem);
            }
            this.close();
          } else {
            this.color = "error";
          }
          this.message = res.data.message;
          this.snackbarB = true;
        });
    }
  }
};
</script>

<style>
#menutype {
  margin: 1%;
}
</style>
