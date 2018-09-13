<template>
  <ve-line :data="chartData" :settings="chartSettings"></ve-line>
</template>

<script>
export default {
  data() {
    this.chartSettings = {
      stack: { 用户: ["订单数量", "当日总销售价"] },
      area: true
    };
    return {
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
          alert(res.data.message);
        }
        console.log(res.data)
      });
    }
  }
};
</script>