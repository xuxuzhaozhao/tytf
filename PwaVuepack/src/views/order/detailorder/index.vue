<template>
  <div class="detailorder">
     <v-dialog
        v-model="dialog"
        fullscreen
        hide-overlay
        transition="dialog-bottom-transition"
        scrollable
      >
        <v-card tile>
          <v-toolbar card dark color="primary">
            <v-btn icon dark @click.native="close">
              <v-icon>close</v-icon>
            </v-btn>
            <v-toolbar-title>订单详情</v-toolbar-title>
          </v-toolbar>
          <v-progress-linear v-if="indeterminate" :indeterminate="indeterminate"></v-progress-linear>
          <v-card-text>
            <div style="font-size:18px">
              &nbsp;<span style="color:black">总价：</span><span style="color:red">￥{{sumPrice}}</span>&nbsp;元整
            </div>
            <br /><hr />
            <v-container grid-list-md text-xs-right>
              <v-layout v-for="menu in menus" :key="menu.Id" align-center justify-space-between row fill-height>
                <v-flex xs6 sm6 md6 lg6>
                  <span class="modifiedMenuName" @click="modifiedItem(menu)">{{menu.MenuName}}</span>
                </v-flex>
                <v-flex xs6 sm6 md6 lg6>
                  ￥{{menu.SinglePrice}}<span v-if="menu.Weight > 1"> × {{menu.Weight}}</span>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card-text>
          <v-btn @click="addItemOpen"><v-icon>add</v-icon>添加菜品</v-btn>
          <v-btn @click="submitBuy" color="success">确认买单</v-btn>
          <v-btn @click="submitBuy" color="success">确认买单</v-btn>
          <div style="flex: 1 1 auto;"></div>
        </v-card>
      </v-dialog>

    <v-dialog v-model="dialogMenuName" max-width="500px">
        <v-card>
          <v-card-title v-if="addnotmodified">
            <v-btn color="error" @click="deleteItem">删除此菜品</v-btn>
          </v-card-title>

          <v-card-text>
            <v-container grid-list-md>
              <v-layout wrap>
                <v-flex xs12>
                  <!-- <v-select :items="selectItems"
                            item-text="name"
                            item-value="id"
                            label="菜品名称"
                            v-model="editedItem.MenuId"
                          >
                  </v-select> -->
                  <v-autocomplete
                            v-model="editedItem.MenuId"
                            :items="selectItems"
                            item-text="name"
                            item-value="id"
                            label="菜品名称"
                        >
                        </v-autocomplete>
                </v-flex>
                <v-flex xs12>
                  <v-text-field v-model="editedItem.SinglePrice" label="菜品单价"></v-text-field>
                </v-flex>
                <v-flex xs12>
                  <v-text-field v-model="editedItem.Weight" label="数量（重量）"></v-text-field>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" flat @click.native="closeItem">取消</v-btn>
            <v-btn color="blue darken-1" flat @click.native="saveItem">保存</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <xsnackbar 
        :color="color" 
        :snackbar.sync="snackbarB" 
        :text="message"/>
  </div>
</template>

<script>
import Xsnackbar from "@/components/snackbar";
export default {
  props: ["detailDialog", "orderId"],

  components: {
    Xsnackbar
  },

  data() {
    return {
      dialogMenuName: false,
      dialog: false,
      editedItem: {},
      addItem: {},
      addnotmodified: true,
      color: "success",
      message: "操作成功！",
      snackbarB: false,
      detailOrderId: "",
      sumPrice: 0,
      indeterminate: false,
      selectItems: [],
      menus: [
        // {
        //   MenuName: "绍子鱼豆花",
        //   SinglePrice: "SinglePrice",
        //   Weight: 3
        // },
        // {
        //   MenuName: "加多宝",
        //   SinglePrice: "12",
        //   Weight: 3
        // },
        // {
        //   MenuName: "家常翘壳（黑龙滩水库）",
        //   SinglePrice: "SinglePrice"
        // }
      ]
    };
  },

  create() {},

  watch: {
    detailDialog(val) {
      if (val) {
        this.dialog = val;
        this.initialize();
        this.getSelectList();
      }
    },

    "editedItem.MenuId"(val) {
      this.selectItems.forEach(item => {
        if (item.id === this.editedItem.MenuId) {
          this.editedItem.SinglePrice = item.price;
          this.editedItem.Weight = 1;
        }
      });
    }
  },

  methods: {
    initialize() {
      this.indeterminate = true;
      this.$http
        .get(`${this.$domain}/api/order/getdetail/${this.orderId}`)
        .then(res => {
          if (res.data.code === 20000) {
            this.menus = res.data.data;
            let sum = 0;
            this.menus.forEach(item => {
              if (item.Weight === 0) {
                item.Weight = 1;
              }
              sum += item.SinglePrice * item.Weight;
            });
            this.sumPrice = sum;
            this.indeterminate = false;
          } else {
            alert(res.data.message);
          }
        });
    },

    getSelectList() {
      this.$http.get(`${this.$domain}/api/BaseTable/Menu/select`).then(res => {
        if (res.data.code == 20000) {
          this.selectItems = res.data.data;
        }
      });
    },

    addItemOpen() {
      this.addItem = {};
      this.addnotmodified = false;
      this.dialogMenuName = true;
    },

    close() {
      this.dialog = false;
      this.$emit("update:detailDialog", false);
    },

    modifiedItem(menu) {
      this.addnotmodified = true;
      this.dialogMenuName = true;
      this.editedItem = menu;
    },

    closeItem() {
      this.dialogMenuName = false;
      this.editedItem = {};
      this.sumPrice = 0;
      this.menus = [];
      this.initialize();
    },

    saveItem() {
      this.editedItem.OrderId = this.orderId;
      this.$http
        .post(`${this.$domain}/api/Order/saveDetailOrder`, this.editedItem)
        .then(res => {
          if (res.data.code === 20000) {
            this.color = "success";

            this.closeItem();
          } else {
            this.color = "error";
          }
          this.message = res.data.message;
          this.snackbarB = true;
        });
    },

    deleteItem() {
      let item = this.editedItem;
      const index = this.menus.indexOf(item);
      if (confirm("确定删除此菜品吗?")) {
        this.menus.splice(index, 1);
        this.$http
          .delete(`${this.$domain}/api/BaseTable/DetailOrder/del/${item.Id}`)
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

    submitBuy() {
      if (confirm("买单后也能修改菜单，确定吗?")) {
        this.$http
          .get(`${this.$domain}/api/order/submitBuy/${this.orderId}`)
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
    }
  }
};
</script>

<style>
.modifiedMenuName {
  color: #00838f;
}
</style>
