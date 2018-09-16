<template>
  <div id="order">
     <v-toolbar flat color="white">
      <v-toolbar-title>订单管理</v-toolbar-title>
      <v-divider
        class="mx-2"
        inset
        vertical
      ></v-divider>
      <v-spacer></v-spacer>
      <v-btn @click="initialize">刷新页面</v-btn>

      <v-btn @click="openquerydialog">选择查询</v-btn>
      <v-dialog v-model="dialog" max-width="500px">
        <v-card>
          <v-card-title>
            <span class="headline">修改订单</span>
          </v-card-title>

          <v-card-text>
            <v-container grid-list-md>
              <v-layout wrap>
                <v-flex xs12 sm12 md12>
                  <v-select :items="selectItems"
                            item-text="name"
                            item-value="id"
                            label="餐桌位置"
                            v-model="editedItem.PositionId"
                          >
                  </v-select>
                </v-flex>
                <v-flex xs12 sm12 md12>
                  <v-text-field disabled v-model="editedItem.ShouldPrice" label="应收款"></v-text-field>
                </v-flex>
                <!-- <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.DiscountRate" label="折扣率"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.ActuallyPrice" label="实收款"></v-text-field>
                </v-flex> -->
                <v-flex xs12 sm12 md12>
                  <v-select :items="items"
                            item-text="text"
                            item-value="value"
                            label="是否买单"
                            v-model="editedItem.IsBuyed"
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
      
      <v-dialog v-model="querydialog" max-width="500px">
        <v-card>
          <v-card-title>
            <span class="headline">选择查询条件</span>
          </v-card-title>

          <v-card-text>
            <v-container grid-list-md>
              <v-layout wrap>
                <v-flex xs12 sm6 md6>
                  <v-select :items="selectItems"
                            item-text="name"
                            item-value="id"
                            label="餐桌位置"
                            v-model="pagination.PositionId"
                          >
                  </v-select>
                </v-flex>
                 <v-flex xs12 sm6 md6>
                  <v-select :items="items"
                            item-text="text"
                            item-value="value"
                            label="是否买单"
                            v-model="pagination.IsBuyed"
                          >
                  </v-select>
                </v-flex>

               <v-flex  xs12 sm6 md6>
                  <v-menu
                    ref="menu2"
                    :close-on-content-click="false"
                    v-model="menu2"
                    :nudge-right="40"
                    :return-value.sync="pagination.date"
                    lazy
                    transition="scale-transition"
                    offset-y
                    full-width
                    min-width="290px"
                  >
                    <v-text-field
                      slot="activator"
                      v-model="pagination.date"
                      label="创建时间上限"
                      prepend-icon="event"
                      readonly
                    ></v-text-field>
                    <v-date-picker v-model="pagination.date" @input="$refs.menu2.save(pagination.date)"></v-date-picker>

                  </v-menu>
               </v-flex>

               <v-flex  xs12 sm6 md6>
                 <v-menu
                    ref="menu3"
                    :close-on-content-click="false"
                    v-model="menu3"
                    :nudge-right="40"
                    :return-value.sync="pagination.date1"
                    lazy
                    transition="scale-transition"
                    offset-y
                    full-width
                    min-width="290px"
                  >
                    <v-text-field
                      slot="activator"
                      v-model="pagination.date1"
                      label="创建时间下限"
                      prepend-icon="event"
                      readonly
                    ></v-text-field>
                    <v-date-picker v-model="pagination.date1" @input="$refs.menu3.save(pagination.date1)"></v-date-picker>

                  </v-menu>
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

     </v-toolbar>
    <v-data-table
      :headers="headers"
      :items="desserts"
      :search="search"
      :pagination.sync="pagination"
      :total-items="totalItems"
      :loading="loading"
      hide-actions
      class="elevation-1"
    >
      <template slot="headerCell" slot-scope="props">
        <v-tooltip bottom>
          <span slot="activator">
            {{ props.header.text }}
          </span>
          <span>
            {{ props.header.text }}
          </span>
        </v-tooltip>
      </template>
      <template 
        slot="items" 
        slot-scope="props">
          <tr :style="props.item.IsBuyed?'background-color: #E0F2F1;':'background-color: #FFEBEE;'">
            <td>
                <v-icon
                @click="editItem(props.item)"
                style="color:green;"
            >
                check_circle  
            </v-icon>
            </td>
            <td >
            <v-icon
                @click="openItem(props.item)"
                style="color:#1976D2;"
            >
                edit
            </v-icon>
            </td>
            <td>{{ props.item.CreateTime }}</td>
            <td>{{ props.item.PositionName }}</td>
            <td>{{ props.item.ShouldPrice }}</td>
            <!-- <td>{{ props.item.DiscountRate }}</td>
            <td>{{ props.item.ActuallyPrice }}</td> -->
            <td>{{ props.item.IsBuyed?'已买单':'未买单' }}</td>
            <td>{{ props.item.WaiterName }}</td>
            <td>
            <v-icon
                @click="deleteItem(props.item)"
                style="color:red;"
            >
                delete
            </v-icon>
            </td>
          </tr>
      </template>
    </v-data-table>
    <div class="text-xs-center pt-2">
      <v-pagination v-model="pagination.page" :length="pages"></v-pagination>
    </div>
    <detail-order :detailDialog.sync="detailDialog"
                  :orderId="orderId"/>

    <xsnackbar :color="color" 
               :snackbar.sync="snackbarB" 
               :text="message"/>
  </div>
</template>

<script>
import Xsnackbar from "@/components/snackbar";
import DetailOrder from "@/views/order/detailorder/index";
export default {
  components: {
    DetailOrder,
    Xsnackbar
  },

  data() {
    return {
      color: "success",
      message: "操作成功！",
      snackbarB: false,
      detailDialog: false,
      querydialog: false,
      date: null,
      menu2: false,
      date1: null,
      menu3: false,
      orderId: "",
      items: [
        { text: "已买单", value: true },
        { text: "未买单", value: false }
      ],
      search: "",
      pagination: {
        rowsPerPage: 15,
        descending: true
      },
      loading: false,
      totalItems: 0,
      selected: [],
      selectItems: [],
      headers: [
        {
          text: "买单",
          sortable: false,
          align: "left"
        },
        {
          text: "修改",
          sortable: false
        },
        { text: "创建时间", value: "CreateTime", align: "left" },
        {
          text: "餐桌位置",
          value: "PositionName",
          sortable: false
        },
        {
          text: "应收款",
          value: "ShouldPrice",
          sortable: false
        },
        // {
        //   text: "折扣率",
        //   value: "DiscountRate",
        //   sortable: false
        // },
        // {
        //   text: "实收款",
        //   value: "ActuallyPrice",
        //   sortable: false
        // },
        {
          text: "是否买单",
          value: "IsBuyed",
          sortable: false
        },
        {
          text: "服务人员",
          value: "WaiterName",
          sortable: false
        },
        {
          text: "删除",
          sortable: false
        }
      ],
      dialog: false,
      editedItem: {},
      desserts: []
    };
  },

  computed: {
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

  created() {
    this.initialize();
    this.getSelectList();
  },

  watch: {
    pagination: {
      handler() {
        this.initialize();
      },
      deep: true
    },

    detailDialog(val) {
      if (!val) {
        this.initialize();
      }
    }
  },

  methods: {
    initialize() {
      this.loading = true;
      this.$http
        .post(`${this.$domain}/api/Order/getlist`, this.pagination)
        .then(res => {
          this.loading = false;
          if (res.data.code === 20000) {
            this.desserts = res.data.data.content;
            this.totalItems = res.data.data.count;
            this.pagination.totalItems = this.totalItems;
            console.log(this.pagination);
          } else {
            this.color = "error";
            this.message = res.data.message;
            this.snackbarB = true;
          }
        })
        .catch(() => {
          this.loading = false;
        });
    },

    getSelectList() {
      this.$http
        .get(`${this.$domain}/api/BaseTable/TablePosition/select`)
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

    deleteItem(item) {
      const index = this.desserts.indexOf(item);
      this.$confirm("徐老板，订单删除是很危险的操作，删除了就恢复不了哦？确定？?", {
        buttonTrueText: "确定",
        buttonFalseText: "返回"
      }).then(res => {
        if (res) {
          this.desserts.splice(index, 1);
        this.$http
          .delete(`${this.$domain}/api/Order/DeleteOrder/${item.OrderId}`)
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
        .post(`${this.$domain}/api/Order/save`, this.editedItem)
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

    query() {
      this.initialize();
      this.querydialog = false;
    },

    openquerydialog() {
      this.pagination.date = null;
      this.pagination.date1 = null;
      this.pagination.IsBuyed = null;
      this.pagination.PositionId = null;
      this.querydialog = true;
    },

    openItem(item) {
      this.orderId = item.OrderId;
      this.detailDialog = true;
    }
  }
};
</script>

<style scoped>
#order {
  margin: 1%;
}
</style>


</style>
