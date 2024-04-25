<script setup lang="ts">
    import Cart from '@/components/Cart.vue'
    import { ref } from 'vue'
    import { useRouter } from 'vue-router';
    import { getPurchasePayment, getPurchaseShipping, postPurchasePayment, postPurchaseShipping } from '@/commerble/api'
    import type { CommerblePurchasePayment, CommerblePurchaseShipping } from '../commerble/types';

    type Data =  CommerblePurchaseShipping | CommerblePurchasePayment;
    
    const router = useRouter()
    const login = () => router.push('/login');
    const next = () => router.push('/checkout');

    const cartId = 1;
    const data = ref<null | Data>(null);

    const step1 = async () => {
        const result = await getPurchaseShipping(cartId);

        if (result.type === 'next') {
            switch (result.next) {
                case 'site/login': login(); break;
                default: console.error(`'${result.next}' is not supported`);
            }
        }
        else {
            data.value = result;
        }
    }
    const goStep2 = async () => {
        if (data.value?.type !== 'purchase/shipping')
            return;

        if (data.value.orderCustomerOrderedAddress == null)
            return;

        if (data.value.deliveryOrderAddress == null)
            return;

        data.value.orderCustomerOrderedAddress.recipientfirstname = data.value.customer?.firstName;
        data.value.orderCustomerOrderedAddress.recipientfirstnamekana = data.value.customer?.firstNameKana;
        data.value.orderCustomerOrderedAddress.recipientlastname = data.value.customer?.lastName;
        data.value.orderCustomerOrderedAddress.recipientlastnamekana = data.value.customer?.lastNameKana;

        data.value.deliveryOrderAddress.zipCode = data.value.orderCustomerOrderedAddress?.zipCode;
        data.value.deliveryOrderAddress.pref = data.value.orderCustomerOrderedAddress?.pref;
        data.value.deliveryOrderAddress.city = data.value.orderCustomerOrderedAddress?.city;
        data.value.deliveryOrderAddress.street = data.value.orderCustomerOrderedAddress?.street;
        data.value.deliveryOrderAddress.building = data.value.orderCustomerOrderedAddress?.building;
        data.value.deliveryOrderAddress.tel = data.value.orderCustomerOrderedAddress?.tel;
        data.value.deliveryOrderAddress.recipientfirstname = data.value.customer?.firstName;
        data.value.deliveryOrderAddress.recipientfirstnamekana = data.value.customer?.firstNameKana;
        data.value.deliveryOrderAddress.recipientlastname = data.value.customer?.lastName;
        data.value.deliveryOrderAddress.recipientlastnamekana = data.value.customer?.lastNameKana;

        const result = await postPurchaseShipping(cartId, data.value);

        if (result.type === 'next') {
            switch (result.next) {
                case 'site/login': login(); break;
                case 'purchase/payment': step2(); break;
                default: console.error(`'${result.next}' is not supported`);
            }
        }
        else {
            data.value = result;
        }
    }
    const step2 = async () => {
        const result = await getPurchasePayment(cartId);

        if (result.type === 'next') {
            switch (result.next) {
                case 'site/login': login(); break;
                default: console.error(`'${result.next}' is not supported`);
            }
        }
        else {
            data.value = result;
        }
    }
    const goStep3 = async () => {
        if (data.value?.type !== 'purchase/payment')
            return;

        const result = await postPurchasePayment(cartId, data.value);

        if (result.type === 'next') {
            switch (result.next) {
                case 'site/login': login(); break;
                case 'purchase/confirm': next(); break;
                default: console.error(`'${result.next}' is not supported`);
            }
        }
        else {
            data.value = result;
        }
    }

    const deliveryableDetas = () => {
        const now = new Date().getTime();
        const unit = 1000 * 60 * 60 * 24;
        const formatter = new Intl.DateTimeFormat('ja', { year: 'numeric', month: 'long', day: 'numeric', weekday: 'short' });
        return [...(new Array(10) as any).keys()].map(i => new Date(now + i * unit)).map(d => ({
            value: d.toISOString().substring(0, 10).replace(/-/g, '/'),
            text: formatter.format(d)
        }));
    }

    const deliveryDateOptions = ref(deliveryableDetas());

    step1();
</script>

<template>
    <template v-if="data">
        <h2>Checkout</h2>

        <Cart :items="data.items" :state="data.state"></Cart>

        <section v-if="data.type == 'purchase/shipping'">
            <h3>STEP 1</h3>
            <fieldset>
                <h4>購入者情報</h4>
                <label>メールアドレス</label>
                <input type="email" v-model="data.customer.emailAddr" />

                <label>お名前（姓）</label>
                <input v-model="data.customer.lastName" />

                <label>お名前（名）</label>
                <input v-model="data.customer.firstName" />

                <label>フリガナ（セイ）</label>
                <input v-model="data.customer.lastNameKana" />

                <label>フリガナ（メイ）</label>
                <input v-model="data.customer.firstNameKana" />

                <label>郵便番号</label>
                <input type="tel" v-model="data.orderCustomerOrderedAddress.zipCode" />

                <label>都道府県</label>
                <input v-model="data.orderCustomerOrderedAddress.pref" />

                <label>市区町村</label>
                <input v-model="data.orderCustomerOrderedAddress.city" />

                <label>通り・丁目・番地・号</label>
                <input v-model="data.orderCustomerOrderedAddress.street" />

                <label>建物名・部屋番号</label>
                <input v-model="data.orderCustomerOrderedAddress.building" />

                <label>電話番号</label>
                <input type="tel" v-model="data.orderCustomerOrderedAddress.tel" />
            </fieldset>
            <button @click="goStep2">次へ</button>
        </section>
        <section v-if="data.type == 'purchase/payment'">
            <h3>STEP 2</h3>
            <fieldset>
                <h4>配送オプション</h4>
                <label>お届け日</label>
                <select v-model="data.deliveryOrder.deliveryDate">
                    <option value="">---</option>
                    <option v-for="d in deliveryDateOptions" :value="d.value">{{d.text}}</option>
                </select>

                <label>お届け時間帯</label>
                <select v-model="data.deliveryOrder.hourRange">
                    <option value="0000">指定なし</option>
                    <option value="0812">午前中</option>
                    <option value="1214">12:00-14:00</option>
                    <option value="1416">14:00-16:00</option>
                    <option value="1618">16:00-18:00</option>
                    <option value="1821">18:00-21:00</option>
                </select>

                <label>明細書への金額印字</label>
                <select v-model="data.deliveryOrder.wrappingType">
                    <option value="2">印字する</option>
                    <option value="1">印字しない</option>
                </select>
            </fieldset>
            <fieldset>
                <h4>お支払オプション</h4>
                <label>決済方法</label>
                <select v-model="data.paymentMethod">
                    <option value="">---</option>
                    <option value="CashOnDelivery">代引き</option>
                    <option value="Token">クレジットカード</option>
                </select>

                <label>ポイント使用</label>
                <input type="tel" v-model="data.inputUsagePoint" />

                <label>クーポンコード</label>
                <input v-model="data.serviceValues.campaignCode" />
            </fieldset>
            <button @click="goStep3">次へ</button>
        </section>
    </template>
</template>
