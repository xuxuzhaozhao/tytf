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
            <v-toolbar-title>订单详情 - {{isBuyed?'已买单':'未买单'}}</v-toolbar-title>
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
          <!-- <v-btn @click="submitBuy" color="success">确认买单</v-btn> -->
          <v-btn @click="printMenu(0)" color="primary">打印菜单(三联)</v-btn>
          <v-menu offset-y>
            <v-btn
                slot="activator"
                color="primary"
                dark
                style="width:96.3%"
            >
                更多打印选项
            </v-btn>
            <v-list>
                <v-list-tile
                v-for="item in printitems"
                :key="item.key"
                @click="printMenu(item.key)"
                >
                <v-list-tile-title>{{ item.value }}</v-list-tile-title>
                </v-list-tile>
            </v-list>
          </v-menu>
          
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
        <x-loading :loading='loading' message="正在传送菜单数据..." />
  </div>
</template>

<script>
import XLoading from "@/components/loading";
import Xsnackbar from "@/components/snackbar";
export default {
  props: ["detailDialog", "orderId", "isBuyed", "detailItem"],

  components: {
    Xsnackbar,
    XLoading
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
      menus: [],
      loading: false,
      detailItemS: {},
      //'打印总订单（总台 留）','打印大厨房菜单','打印凉菜房菜单'
      printitems: [
        {
          key: 1,
          value: "打印总订单（总台 留）"
        },
        {
          key: 2,
          value: "打印大厨房菜单"
        },
        {
          key: 3,
          value: "打印凉菜房菜单"
        }
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

    detailItem(val) {
      this.detailItemS = val;
    },

    dialog(val) {
      if (!val) {
        this.$emit("modifiedPrice", this.detailItemS);
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
              this.detailItemS.ShouldPrice = sum;
            });
            this.sumPrice = sum;
            this.indeterminate = false;
          } else {
            this.color = "error";
            this.message = res.data.message;
            this.snackbarB = true;
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
      this.$confirm("确定删除此菜品吗?", {
        buttonTrueText: "确定",
        buttonFalseText: "返回"
      }).then(res => {
        if (res) {
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
      });
    },

    // submitBuy() {
    //   this.$confirm("买单后也能修改菜单，确定吗?", {
    //     buttonTrueText: "确定",
    //     buttonFalseText: "返回"
    //   }).then(res => {
    //     if (res) {
    //       this.$http
    //         .get(`${this.$domain}/api/order/submitBuy/${this.orderId}`)
    //         .then(res => {
    //           if (res.data.code === 20000) {
    //             this.color = "success";
    //           } else {
    //             this.color = "error";
    //           }
    //           this.message = res.data.message;
    //           this.snackbarB = true;
    //         });
    //     }
    //   });
    // },

    printMenu(val) {
        let ti ;
        switch (val) {
            case 0:
                ti="将打印三联单";
                break;
            case 1:
                ti="将打印总订单（总台 留）";
                break;
            case 2:
                ti="将打印大厨房菜单";
                break;
            case 3:
                ti="将打印凉菜房菜单";
                break;
        }
      this.$confirm(`${this.isBuyed ? "订单已买单" : ti}，是否继续打印?`, {
        buttonTrueText: "确定",
        buttonFalseText: "返回"
      }).then(res => {
        if (res) {
          this.loading = true;
          this.$http
            .get(
              `${this.$domain}/api/Printer/PrintOrder/${this.orderId}/${val}`
            )
            .then(res => {
              if (res.data.code === 20000) {
                this.color = "success";
              } else {
                this.color = "error";
              }
              this.message = res.data.message;
              this.snackbarB = true;
              this.loading = false;
            });
        }
      });
    }
  }
};
</script>

<style>
.modifiedMenuName {
  color: #00838f;
}
</style>
