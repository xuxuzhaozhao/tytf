<template>
    <v-layout row justify-center>
    <v-dialog v-model="loading" persistent content content-class="centered-dialog">
      <v-container fill-height>
        <v-layout column justify-center align-center>
          <v-progress-circular indeterminate :size="70" :width="7" :color="progressColor"></v-progress-circular>
          <h3 v-if="message != null">{{message}}</h3>
        </v-layout>
      </v-container>
    </v-dialog>
  </v-layout>
</template>

<script>
export default {
  props: {
    loading: { type: Boolean, default: false },
    message: { type: String, default: "订单数据正在加载中..." },
    progressColor: { type: String, default: "purple" }
  }
};
</script>

<style>
.dialog.centered-dialog,
.v-dialog.centered-dialog {
  background: #282c2dad;
  box-shadow: none;
  border-radius: 6px;
  width: auto;
  color: whitesmoke;
}
</style>
