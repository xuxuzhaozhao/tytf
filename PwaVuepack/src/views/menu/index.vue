<template>
  <div id="menuindex">
    <v-toolbar flat color="white">
      <v-toolbar-title>菜品管理</v-toolbar-title>
      <v-divider
        class="mx-2"
        inset
        vertical
      ></v-divider>
      <v-spacer></v-spacer>
      <v-btn @click="openquerydialog">选择查询</v-btn>
      <v-dialog v-model="querydialog" max-width="500px">
        <v-card>
          <v-card-title>
            <span class="headline">选择查询条件</span>
          </v-card-title>

          <v-card-text>
            <v-container grid-list-md>
              <v-layout wrap>
                <v-flex xs12 sm12 md12>
                  <v-select :items="selectItems"
                            item-text="name"
                            item-value="id"
                            label="选择菜品种类"
                            solo
                            clear-icon
                            v-model="pagination.MenuTypeId"
                          >
                  </v-select>
                </v-flex>
                 <v-flex xs12 sm12 md12>
                  <v-text-field label="输入菜品名称"
                                solo
                                v-model="pagination.MenuName" />
                  </v-select>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" flat @click.native="querydialog = false">取消</v-btn>
            <v-btn color="blue darken-1" flat @click.native="query">查询</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <v-dialog v-model="dialog" max-width="500px">
        <v-btn slot="activator" color="primary" dark class="mb-2">新增菜品</v-btn>
        <v-card>
          <v-card-title>
            <span class="headline">{{ formTitle }}</span>
          </v-card-title>

          <v-card-text>
            <v-container grid-list-md>
              <v-layout wrap>
                <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.Name" label="菜品名称"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-select :items="selectItems"
                            item-text="name"
                            item-value="id"
                            label="选择菜品种类"
                            solo
                            v-model="editedItem.MenuTypeId"
                          >
                  </v-select>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.Price" label="单价 (或每斤单价)"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.Description" label="简单描述"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.Sort" label="排序"></v-text-field>
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
      :pagination.sync="pagination"
      :total-items="totalItems"
      :loading="loading"
      hide-actions
      class="elevation-1"
    >
      <template slot="items" slot-scope="props">
        <td>
          <v-icon
            @click="uploadItem(props.item)"
          >
            present_to_all
          </v-icon>
        </td>
        <td>{{ props.item.MenuTypeName }}</td>
        <td>{{ props.item.Name }}</td>
        <td>{{ props.item.Price }}</td>
        <td>{{ props.item.Description }}</td>
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
     <div class="text-xs-center pt-2">
      <v-pagination v-model="pagination.page" :length="pages"></v-pagination>
    </div>

    <v-dialog
      v-model="uploadDialog"
      width="220"
    >
     <v-card>
        <v-card-title class="headline">
          {{editedItem.Name}}
        </v-card-title>
        <xuploader :src="`${this.$domain}/api/upload/menu/${editedItem.Id}`" 
                   ref="xuploader" 
                   @finished='uploadDialog = false'
                   :originFiles='originFiles' />
      </v-card>
    </v-dialog>


    <xsnackbar :color="color" 
               :snackbar.sync="snackbarB" 
               :text="message"/>
  </div>
</template>

<script>
import Xsnackbar from "@/components/snackbar";
import Xuploader from "@/components/uploader";
export default {
  components: {
    Xsnackbar,
    Xuploader
  },
  data: () => ({
    pagination: {
      rowsPerPage: 15
    },
    originFiles: [],
    loading: false,
    totalItems: 0,
    color: "success",
    message: "操作成功！",
    snackbarB: false,
    items: [{ text: "启用", value: true }, { text: "停用", value: false }],
    dialog: false,
    querydialog: false,
    headers: [
      { text: "上传美照", sortable: false },
      { text: "种类名称", value: "MenuTypeName", sortable: false },
      {
        text: "菜品名称",
        align: "left",
        sortable: false,
        value: "Name"
      },
      { text: "单价 (或每斤单价)", value: "Price", sortable: false },
      { text: "简单描述", value: "Description", sortable: false },
      { text: "排序", value: "Sort" },
      { text: "编辑", sortable: false },
      { text: "删除", sortable: false }
    ],
    selectItems: [],
    desserts: [],
    editedIndex: -1,
    editedItem: {
      Name: "",
      MenuTypeName: 0,
      Price: 0,
      Description: "",
      icon: 0,
      IsUsed: true
    },
    defaultItem: {
      Name: "",
      MenuTypeName: 0,
      Price: 0,
      Description: "",
      icon: 0,
      IsUsed: true
    },
    uploadDialog: false
  }),

  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "新增菜品" : "编辑菜品";
    },
    pages() {
      if (
        this.pagination.rowsPerPage == null ||
        this.pagination.totalItems == null
      )
        return 0;

      return Math.ceil(
        this.pagination.totalItems / this.pagination.rowsPerPage
      );
    }
  },

  watch: {
    dialog(val) {
      val || this.close();
    },
    pagination: {
      handler() {
        this.initialize();
      },
      deep: true
    },
    uploadDialog(val) {
      if (!val) {
        this.$refs.xuploader.finished();
      }
    }
  },

  created() {
    this.initialize();
    this.getSelectList();
  },

  methods: {
    initialize() {
      this.$http
        .post(`${this.$domain}/api/Menu/getlist`, this.pagination)
        .then(res => {
          if (res.data.code === 20000) {
            this.desserts = res.data.data.content;
            this.totalItems = res.data.data.count;
            this.pagination.totalItems = this.totalItems;
          } else {
            alert(res.data.message);
          }
        });
    },

    getSelectList() {
      this.$http
        .get(`${this.$domain}/api/BaseTable/MenuType/select`)
        .then(res => {
          if (res.data.code == 20000) {
            this.selectItems = res.data.data;
          }
        });
    },

    editItem(item) {
      this.editedIndex = this.desserts.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialog = true;
    },

    uploadItem(item) {
      this.editedIndex = this.desserts.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.uploadDialog = true;
      this.originFiles = [];
      this.originFiles.push({
        src: item.RootUrl + item.Icon,
        name: item.Name
      });
    },

    deleteItem(item) {
      const index = this.desserts.indexOf(item);
      if (confirm("确定删除此菜品吗?")) {
        this.desserts.splice(index, 1);
        this.$http
          .delete(`${this.$domain}/api/BaseTable/Menu/del/${item.Id}`)
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
        .post(`${this.$domain}/api/Menu/save`, this.editedItem)
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
          this.initialize();
        });
    },

    openquerydialog() {
      this.pagination.MenuTypeId = null;
      this.pagination.MenuName = null;
      this.querydialog = true;
    },

    query() {
      this.initialize();
      this.querydialog = false;
    }
  }
};
</script>

<style>
#menuindex {
  margin: 1%;
}
</style>
