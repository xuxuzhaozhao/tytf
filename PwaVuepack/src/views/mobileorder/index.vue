<template>
    <div>
        <v-toolbar flat color="white">
            <v-toolbar-title>订单管理</v-toolbar-title>
            <v-divider
                class="mx-2"
                inset
                vertical
            ></v-divider>
            <v-spacer></v-spacer>
            <v-btn @click="reload">刷新</v-btn>

            <v-btn @click="openquerydialog">查询</v-btn>
            
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
        <v-card>
            <v-container
                fluid
                grid-list-lg
            >
                <v-layout row wrap>
                <v-flex xs12 v-for="item in orderList" :key="item.OrderId">
                    <v-card :color="item.IsBuyed?'#BDBDBD':'#E53935'" class="white--text">
                    <v-card-title primary-title>
                        <div>
                            <div class="headline">{{getdatetime(item.CreateTime)}} &nbsp;&nbsp; {{item.IsBuyed?'已买单':'未买单'}}</div>
                            <div> </div>
                            <div>位置：{{item.PositionName}}。</div>
                            <div>应收款：<span style="font-weight:700;font-size:18px;">￥{{item.ShouldPrice}}</span> 元整</div>
                            <div>服务人员：{{item.WaiterName}}</div>
                        </div>
                    </v-card-title>
                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn :color="'#FAFAFA'" v-if="item.IsBuyed" @click="submitBuy(item)">取消买单</v-btn>
                        <v-btn :color="'#FAFAFA'" v-if="!item.IsBuyed" @click="submitBuy(item)">确认买单</v-btn>
                        <v-btn :color="'#FAFAFA'"   @click="openItem(item.OrderId)">修改订单</v-btn>
                    </v-card-actions>
                    </v-card>
                </v-flex>
                </v-layout>
                <v-btn v-if="frontButton" color="primary" @click="frontPage"><v-icon>chevron_left</v-icon>上一页</v-btn>
                <v-btn v-if="nextButton" color="primary" @click="nextPage">下一页<v-icon>chevron_right</v-icon></v-btn>
            </v-container>
            <v-btn
              fixed
              dark
              fab
              bottom
              right
              color="primary"
              @click="returnTop"
            >
              <v-icon>keyboard_arrow_up</v-icon>
            </v-btn>
        </v-card>

        <detail-order :detailDialog.sync="detailDialog"
            :orderId="orderId"/>

        <xsnackbar :color="color" 
            :snackbar.sync="snackbarB" 
            :text="message"/>
        <x-loading :loading='loading' />
    </div>
</template>

<script>
import Xsnackbar from "@/components/snackbar";
import DetailOrder from "@/views/order/detailorder/index";
import XLoading from "@/components/loading";
export default {
  components: {
    DetailOrder,
    Xsnackbar,
    XLoading
  },
  data() {
    return {
      loading: false,
      pagination: {
        rowsPerPage: 8,
        descending: true,
        page: 1
      },
      orderList: [],
      querydialog: false,
      detailDialog: false,
      selectItems: [],
      menu2: false,
      menu3: false,
      orderId: "",
      message: "",
      snackbarB: "",
      color: "",
      items: [
        { text: "已买单", value: true },
        { text: "未买单", value: false }
      ],
      frontButton: false,
      nextButton: false
    };
  },
  created() {
    this.initialize();
    this.getSelectList();
  },
  watch: {
    detailDialog(val) {
      if (!val) {
        this.initialize();
      }
    },
    pagination: {
      handler() {
        this.initialize();
      },
      deep: true
    },
    "pagination.page": {
      handler() {
        if (this.pagination.page <= 1) {
          this.frontButton = false;
        } else {
          this.frontButton = true;
        }

        console.log(this.pagination.page, this.pages);
        if (this.pagination.page >= this.pages) {
          this.nextButton = false;
        } else {
          this.nextButton = true;
        }
      }
    }
  },
  // computed: {
  //   pages() {
  //     if (
  //       this.pagination.rowsPerPage == null ||
  //       this.pagination.totalItems == null
  //     )
  //       return 0;

  //     return Math.ceil(
  //       this.pagination.totalItems / this.pagination.rowsPerPage
  //     );
  //   }
  // },
  methods: {
    initialize() {
      this.loading = true;
      this.$http
        .post(`${this.$domain}/api/Order/getlist`, this.pagination)
        .then(res => {
          this.loading = false;
          if (res.data.code === 20000) {
            this.returnTop();
            this.orderList = res.data.data.content;
            this.pagination.totalItems = res.data.data.count;

            this.handlePage();
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

    handlePage() {
      if (
        this.pagination.rowsPerPage == null ||
        this.pagination.totalItems == null
      ) {
        this.pages = 0;
      } else {
        this.pages = Math.ceil(
          this.pagination.totalItems / this.pagination.rowsPerPage
        );
      }

      if (this.pagination.page <= 1) {
        this.frontButton = false;
      } else {
        this.frontButton = true;
      }

      if (this.pagination.page >= this.pages) {
        this.nextButton = false;
      } else {
        this.nextButton = true;
      }
    },

    nextPage() {
      this.pagination.page++;
    },

    frontPage() {
      this.pagination.page--;
    },

    returnTop() {
      this.scrollToTop(15); //300ms滑动到顶
    },

    scrollToTop(scrollDuration) {
      var scrollStep = -window.scrollY / (scrollDuration / 15),
        scrollInterval = setInterval(function() {
          if (window.scrollY != 0) {
            window.scrollBy(0, scrollStep);
          } else clearInterval(scrollInterval);
        }, 15);
    },

    reload() {
      this.pagination = {
        page: 1,
        rowsPerPage: 8,
        descending: true
      };
      this.initialize();
    },

    getdatetime(time) {
      return time.slice(5);
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

    query() {
      this.initialize();
      this.querydialog = false;
    },

    openquerydialog() {
      this.pagination.date = null;
      this.pagination.page = 1;
      this.pagination.date1 = null;
      this.pagination.IsBuyed = null;
      this.pagination.PositionId = null;
      this.querydialog = true;
    },

    openItem(orderId) {
      this.orderId = orderId;
      this.detailDialog = true;
    },

    submitBuy(item) {
      this.$confirm("确定买单?", {
        buttonTrueText: "确定",
        buttonFalseText: "返回" 
      }).then(res => {
        if (res) {
          this.$http
            .get(`${this.$domain}/api/order/submitBuy/${item.OrderId}`)
            .then(res => {
              if (res.data.code === 20000) {
                this.color = "success";
                item.IsBuyed = true;
              } else {
                this.color = "error";
              }
              this.message = res.data.message;
              this.snackbarB = true;
            });
        }
      });
    }
  }
};
</script>

<style>
</style>
