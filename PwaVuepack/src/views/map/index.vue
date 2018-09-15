<template>
  <div id="near">
    <div id="amap-main">
    </div>
  </div>
</template>
<script>
import { lazyAMapApiLoaderInstance } from "vue-amap";
var map;
export default {
  name: "near",
  data() {
    return {
      localData: {}
    };
  },
  mounted() {
    let self = this;
    lazyAMapApiLoaderInstance.load().then(() => {
      //初始化地图
      map = new AMap.Map("amap-main", {
        resizeEnable: true,
        zoom: 20,
        center: new AMap.LngLat(113.309213, 23.146569)
      });
      //这个是添加AMapUI的点标注
      //   AMapUI.loadUI(["overlay/SimpleMarker"], SimpleMarker => {
      //     const marker = new SimpleMarker({
      //       iconLabel: "A",
      //       iconStyle: "red",
      //       color: "#fff",
      //       map: map,
      //       position: [113.309213, 23.146569]
      //     });
      //   });
      //使用定位
      map.plugin("AMap.Geolocation", function() {
        let geolocation = new AMap.Geolocation({
          enableHighAccuracy: true, //是否使用高精度定位，默认:true
          timeout: 10000, //超过10秒后停止定位，默认：无穷大
          buttonOffset: new AMap.Pixel(10, 20), //定位按钮与设置的停靠位置的偏移量，默认：Pixel(10, 20)
          zoomToAccuracy: true, //定位成功后调整地图视野范围使定位位置及精度范围视野内可见，默认：false
          buttonPosition: "RB"
        });
        map.addControl(geolocation);
        geolocation.getCurrentPosition();
        AMap.event.addListener(geolocation, "complete", self.onComplete); //返回定位信息
        AMap.event.addListener(geolocation, "error", self.onError); //返回定位出错信息
      });
    });
  },
  methods: {
    onComplete(data) {
      //里面为定位的信息
      this.localData = data;
      console.log(data);
    },
    onError(data) {
      console.log("error", data);
    }
  }
};
</script>
<style>
#near {
  width: 100%;
  height: 100%;
}
#near #amap-main {
  width: 100%;
  height: 100%;
}
</style>
