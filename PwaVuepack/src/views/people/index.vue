<template>
  <div id="peopleindex">
    <v-toolbar flat color="white">
      <v-toolbar-title>服务人员管理</v-toolbar-title>
      <v-divider
        class="mx-2"
        inset
        vertical
      ></v-divider>
      <v-spacer></v-spacer>
      <v-dialog v-model="dialog" max-width="500px">
        <v-btn slot="activator" color="primary" dark class="mb-2">新增服务员</v-btn>
        <v-card>
          <v-card-title>
            <span class="headline">{{ formTitle }}</span>
          </v-card-title>

          <v-card-text>
            <v-container grid-list-md>
              <v-layout wrap>
                <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.Name" label="姓名"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.WeiXinId" label="微信Id"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-select :items="items"
                            item-text="text"
                            item-value="value"
                            label="是否启用"
                            solo
                            v-model="editedItem.IsUsed"
                          >
                  </v-select>
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
        <td>{{ props.item.WeiXinId }}</td>
        <td>{{ props.item.IsUsed?"启用":"停用" }}</td>
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
    items: [{ text: "启用", value: true }, { text: "停用", value: false }],
    color: "success",
    message: "操作成功！",
    snackbarB: false,
    dialog: false,
    headers: [
      {
        text: "姓名",
        align: "left",
        value: "Name"
      },
      { text: "微信Id（必要）", value: "WeiXinId", sortable: false },
      { text: "是否启用", value: "IsUsed", sortable: false },
      { text: "编辑", sortable: false },
      { text: "删除", sortable: false }
    ],
    desserts: [],
    editedIndex: -1,
    editedItem: {
      Name: "",
      WeiXinId: "",
      IsUsed: true,
      Sort: 1
    },
    defaultItem: {
      Name: "",
      WeiXinId: "",
      IsUsed: true,
      Sort: 1
    }
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "新增服务员" : "编辑服务员";
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
        .get(`${this.$domain}/api/BaseTable/Waiter/getlist`)
        .then(res => {
          if (res.data.code === 20000) {
            this.desserts = res.data.data;
          } else {
            alert(res.data.message);
          }
        });
    },

    editItem(item) {
      console.log(item);
      this.editedIndex = this.desserts.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialog = true;
    },

    deleteItem(item) {
      const index = this.desserts.indexOf(item);
      if (confirm("确定删除此服务人员吗?")) {
        this.desserts.splice(index, 1);
        this.$http
          .delete(`${this.$domain}/api/BaseTable/Waiter/del/${item.Id}`)
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
        .post(`${this.$domain}/api/People/save`, this.editedItem)
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
#peopleindex {
  margin: 1%;
}
</style>
