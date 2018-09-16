<template>
  <ve-line :data="chartData" :settings="chartSettings">
    <xsnackbar :color="color" 
               :snackbar.sync="snackbarB" 
               :text="message"/>
  </ve-line>
</template>

<script>
import Xsnackbar from "@/components/snackbar";
export default {
  components: {
    Xsnackbar
  },
  data() {
    this.chartSettings = {
      stack: { 用户: ["订单数量", "当日总销售价"] },
      area: true
    };
    return {
      color: "success",
      message: "操作成功！",
      snackbarB: false,
      chartData: {
        columns: ["日期", "订单数量", "当日总销售价"],
        rows: []
      }
    };
  },

  created() {
    this.initialize();
  },

  methods: {
    initialize() {
      this.$http.post(`${this.$domain}/api/OrderSummary/GetList`).then(res => {
        if (res.data.code === 20000) {
          this.chartData.rows = res.data.data;
        } else {
          this.color = "error";
          this.message = "上传成功！";
          this.snackbarB = true;
        }
        console.log(res.data);
      });
    }
  }
};
</script>