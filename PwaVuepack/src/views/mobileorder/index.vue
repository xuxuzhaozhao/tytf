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
            <v-btn @click="initialize">刷新页面</v-btn>

            <v-btn @click="initialize">选择查询</v-btn>
            <!-- <v-dialog v-model="dialog" max-width="500px">
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
            </v-dialog> -->
        </v-toolbar>
        <v-card>
            <v-container
                fluid
                grid-list-lg
            >
                <v-layout row wrap>
                <v-flex xs12 v-for="item in [1,1,1,1,1,1,1,1,11,,1,1,1,1,1]" :key="item">
                    <v-card color="pink darken-2" class="white--text">
                    <v-card-title primary-title>
                        <div>
                            <div class="headline">09月13日 21:03 &nbsp;&nbsp;&nbsp; 未买单</div>
                            <div> </div>
                            <div>位置：雅1。</div>
                            <div>应收款：<span style="font-weight:700">￥255.5</span> 元整</div>
                            <div>服务人员：徐程意</div>
                        </div>
                    </v-card-title>
                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn color="green" dark>确认买单</v-btn>
                        <v-btn color="blue" dark>修改订单</v-btn>
                    </v-card-actions>
                    </v-card>
                </v-flex>

                <v-flex xs12>
                    <v-card color="cyan darken-2" class="white--text">
                    <v-layout>
                        <v-flex xs5>
                        <v-img
                            src="https://cdn.vuetifyjs.com/images/cards/foster.jpg"
                            height="125px"
                            contain
                        ></v-img>
                        </v-flex>
                        <v-flex xs7>
                        <v-card-title primary-title>
                            <div>
                            <div class="headline">Supermodel</div>
                            <div>Foster the People</div>
                            <div>(2014)</div>
                            </div>
                        </v-card-title>
                        </v-flex>
                    </v-layout>
                    <v-divider light></v-divider>
                    <v-card-actions class="pa-3">
                        Rate this album
                        <v-spacer></v-spacer>
                        <v-icon>star_border</v-icon>
                        <v-icon>star_border</v-icon>
                        <v-icon>star_border</v-icon>
                        <v-icon>star_border</v-icon>
                        <v-icon>star_border</v-icon>
                    </v-card-actions>
                    </v-card>
                </v-flex>

                <v-flex xs12>
                    <v-card color="purple" class="white--text">
                    <v-layout row>
                        <v-flex xs7>
                        <v-card-title primary-title>
                            <div>
                            <div class="headline">Halycon Days</div>
                            <div>Ellie Goulding</div>
                            <div>(2013)</div>
                            </div>
                        </v-card-title>
                        </v-flex>
                        <v-flex xs5>
                        <v-img
                            src="https://cdn.vuetifyjs.com/images/cards/halcyon.png"
                            height="125px"
                            contain
                        ></v-img>
                        </v-flex>
                    </v-layout>
                    <v-divider light></v-divider>
                    <v-card-actions class="pa-3">
                        Rate this album
                        <v-spacer></v-spacer>
                        <v-icon>star_border</v-icon>
                        <v-icon>star_border</v-icon>
                        <v-icon>star_border</v-icon>
                        <v-icon>star_border</v-icon>
                        <v-icon>star_border</v-icon>
                    </v-card-actions>
                    </v-card>
                </v-flex>
                </v-layout>
            </v-container>
        </v-card>
    </div>
</template>

<script>
export default {
methods:{
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
          } else {
            alert(res.data.message);
          }
        })
        .catch(() => {
          this.loading = false;
        });
    },
}
}
</script>

<style>

</style>
