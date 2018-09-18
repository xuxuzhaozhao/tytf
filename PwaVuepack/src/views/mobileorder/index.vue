<template>
    <div>
        <v-toolbar flat fixed color="white" style="margin-top:56px;">
            <v-toolbar-title>订单管理</v-toolbar-title>
            <v-divider
                class="mx-2"
                inset
                vertical
            ></v-divider>
            <v-spacer></v-spacer>
            <!-- <v-btn @click="reload">刷新</v-btn> -->

            <!-- <v-btn dark color="#E53935" @click="openquerydialog">未买单：{{5}}</v-btn> -->
            <v-btn :color="setting['移动端查询按钮颜色']" @click="openquerydialog">查询</v-btn>
            
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
        <div style="height:33px;"></div>
        <v-card>
            <v-container
                fluid
                grid-list-lg
            >
                <v-layout row wrap>
                <v-flex xs12 v-for="item in orderList" :key="item.OrderId">
                    <v-card :color="item.IsBuyed?setting['已买单卡片颜色']:setting['未买单卡片颜色']" class="white--text">
                    <v-card-title primary-title>
                        <div>
                            <div class="headline">{{item.PositionName}}&nbsp;&nbsp;{{getdatetime(item.CreateTime)}}</div>
                            <div> </div>
                            <div>此订单：{{item.IsBuyed?'已买单':'未买单'}}</div>
                            <div>应收款：<span style="font-weight:700;font-size:18px;">￥{{item.ShouldPrice}}</span> 元整</div>
                            <div>服务人员：{{item.WaiterName}}</div>
                            <div v-if="item.Note!=null && item.Note!=''">备注：{{item.Note}}</div>
                        </div>
                    </v-card-title>
                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn :color="setting['取消买单按钮颜色']" v-if="item.IsBuyed" @click="noBuy(item)">取消买单</v-btn>
                        <v-btn :color="setting['确认买单按钮颜色']" v-if="!item.IsBuyed" @click="submitBuy(item)">确认买单</v-btn>
                        <v-btn :color="setting['菜单详情按钮颜色']"   @click="openItem(item)">菜单详情</v-btn>
                    </v-card-actions>
                    </v-card>
                </v-flex>
                </v-layout>
                <v-btn v-if="frontButton" :color="setting['移动端上一页按钮颜色']" @click="frontPage"><v-icon>chevron_left</v-icon>上一页</v-btn>
                <v-btn v-if="nextButton" :color="setting['移动端下一页按钮颜色']" @click="nextPage">下一页<v-icon>chevron_right</v-icon></v-btn>
            </v-container>
            <v-btn
              fixed
              dark
              fab
              bottom
              right
              :color="setting['移动端回到顶部按钮颜色']"
              @click="returnTop"
            >
              <v-icon>keyboard_arrow_up</v-icon>
            </v-btn>
        </v-card>

        <detail-order :detailDialog.sync="detailDialog"
            :orderId="orderId"
            :isBuyed="isBuyed"/>

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
        rowsPerPage: 12,
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
      nextButton: false,
      isBuyed: false,
      setting: {
        未买单卡片颜色: "primary",
        已买单卡片颜色: "primary",
        确认买单按钮颜色: "#FAFAFA",
        取消买单按钮颜色: "#FAFAFA",
        菜单详情按钮颜色: "#FAFAFA",
        移动端查询按钮颜色: "primary",
        移动端上一页按钮颜色: "primary",
        移动端下一页按钮颜色: "primary",
        移动端回到顶部按钮颜色: "primary"
      }
    };
  },
  created() {
    this.initializeSetting();
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

    initializeSetting() {
      this.$http
        .get(`${this.$domain}/api/BaseTable/TytfSetting/getlist`)
        .then(res => {
          if (res.data.code === 20000) {
            res.data.data.forEach(item => {
              switch (item.Name) {
                case "未买单卡片颜色":
                  this.setting["未买单卡片颜色"] = item.Value;
                  break;
                case "已买单卡片颜色":
                  this.setting["已买单卡片颜色"] = item.Value;
                  break;
                case "确认买单按钮颜色":
                  this.setting["确认买单按钮颜色"] = item.Value;
                  break;
                case "取消买单按钮颜色":
                  this.setting["取消买单按钮颜色"] = item.Value;
                  break;
                case "菜单详情按钮颜色":
                  this.setting["菜单详情按钮颜色"] = item.Value;
                  break;
                case "移动端查询按钮颜色":
                  this.setting["移动端查询按钮颜色"] = item.Value;
                  break;
                case "移动端上一页按钮颜色":
                  this.setting["移动端上一页按钮颜色"] = item.Value;
                  break;
                case "移动端下一页按钮颜色":
                  this.setting["移动端下一页按钮颜色"] = item.Value;
                  break;
                case "移动端回到顶部按钮颜色":
                  this.setting["移动端回到顶部按钮颜色"] = item.Value;
                  break;
              }
            });
          } else {
            this.color = "error";
            this.message = "设置项初始化失败！";
            this.snackbarB = true;
          }
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
        rowsPerPage: 12,
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

    openItem(item) {
      this.orderId = item.OrderId;
      this.isBuyed = item.IsBuyed;
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
    },

    noBuy(item) {
      this.$confirm("确定取消买单吗?", {
        buttonTrueText: "确定",
        buttonFalseText: "返回"
      }).then(res => {
        if (res) {
          this.$http
            .get(`${this.$domain}/api/order/NoBuy/${item.OrderId}`)
            .then(res => {
              if (res.data.code === 20000) {
                this.color = "success";
                item.IsBuyed = false;
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
