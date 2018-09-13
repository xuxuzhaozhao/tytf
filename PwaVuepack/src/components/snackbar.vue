<template>
  <div id="xsnackbar">
    <v-snackbar
      v-model="snackbardialog"
      :color="color"
      :timeout="timeout"
      top
    >
      {{ text }}
      <v-btn
        dark
        flat
        @click="close"
      >
        关闭
      </v-btn>
    </v-snackbar>
  </div>
</template>

<script>
export default {
  props: ["color", "snackbar", "text"],

  data() {
    return {
      timeout: 6000,
      snackbardialog: false
    };
  },

  watch: {
    snackbar(val) {
      if (val) {
        this.snackbardialog = true;
      }
    },
    snackbardialog(val) {
      if (!val) {
        this.close();
      }
    }
  },

  methods: {
    close() {
      this.snackbardialog = false;
      this.$emit("update:snackbar", false);
    }
  }
};
</script>

<style>
</style>
