<script setup lang="ts">
    import Cart from '@/components/Cart.vue'
    import { ref } from 'vue'
    import { useRouter } from 'vue-router';
    import { getPurchaseConfirm, purchase } from '@/commerble/api'
    import type { CommerblePurchaseConfirm, CommerblePurchaseError } from '../commerble/types';

    type Data = CommerblePurchaseError | CommerblePurchaseConfirm;

    const router = useRouter()
    const login = () => router.push('/login');
    const next = (orderId: string) => router.push(`/complete/${orderId}`);

    const cartId = 1;
    const data = ref<null | Data>(null);

    const commit = async () => {
        if (!data.value)
            return;

        const result = await purchase(cartId, data.value.token);

        if (result.type === 'next') {
            switch (result.next) {
                case 'site/login': login(); break;
                case 'purchase/complete':
                    const id = result.raw.match(/\/(\d+)$/)?.[1];
                    if (id) {
                        next(id);
                    }
                    break;
                case 'purchase/external':
                default: console.error(`'${result.next}' is not supported`);
            }
        }
        else {
            data.value = result;
        }
    }

    const result = await getPurchaseConfirm(cartId);

    if (result.type === 'next') {
        switch (result.next) {
            case 'site/login': login(); break;
            default: console.error(`'${result.next}' is not supported`);
        }
    }
    else {
        data.value = result;
    }
</script>

<template>
    <template v-if="data">
        <h2>Checkout</h2>

        <Cart :items="data.items" :state="data.state"></Cart>

        <table>
            <caption>購入者情報</caption>
            <tbody>
                <tr>
                    <th>メールアドレス</th>
                    <td>{{data.orderCustomer.mailAddr}}</td>
                </tr>
                <tr>
                    <th>お名前（姓）</th>
                    <td>{{data.orderCustomer.lastName}}</td>
                </tr>
                <tr>
                    <th>お名前（名）</th>
                    <td>{{data.orderCustomer.firstName}}</td>
                </tr>
                <tr>
                    <th>フリガナ（セイ）</th>
                    <td>{{data.orderCustomer.lastNameKana}}</td>
                </tr>
                <tr>
                    <th>フリガナ（メイ）</th>
                    <td>{{data.orderCustomer.firstNameKana}}</td>
                </tr>
                <tr>
                    <th>住所</th>
                    <td>
                        {{data.orderCustomerOrderedAddress.zipCode}}<br />
                        {{data.orderCustomerOrderedAddress.pref}} {{data.orderCustomerOrderedAddress.city}} {{data.orderCustomerOrderedAddress.street}}<br />
                        {{data.orderCustomerOrderedAddress.building}}<br />
                    </td>
                </tr>
                <tr>
                    <th>電話番号</th>
                    <td>{{data.orderCustomerOrderedAddress.tel}}</td>
                </tr>
            </tbody>
        </table>

        <table>
            <caption>お届け先</caption>
            <tbody>
                <tr>
                    <th>お名前（姓）</th>
                    <td>{{data.deliveryOrderAddress.recipientlastname}}</td>
                </tr>
                <tr>
                    <th>お名前（名）</th>
                    <td>{{data.deliveryOrderAddress.recipientfirstname}}</td>
                </tr>
                <tr>
                    <th>フリガナ（セイ）</th>
                    <td>{{data.deliveryOrderAddress.recipientlastnameKana}}</td>
                </tr>
                <tr>
                    <th>フリガナ（メイ）</th>
                    <td>{{data.deliveryOrderAddress.recipientfirstnameKana}}</td>
                </tr>
                <tr>
                    <th>住所</th>
                    <td>
                        {{data.deliveryOrderAddress.zipCode}}<br />
                        {{data.deliveryOrderAddress.pref}} {{data.deliveryOrderAddress.city}} {{data.deliveryOrderAddress.street}}<br />
                        {{data.deliveryOrderAddress.building}}<br />
                    </td>
                </tr>
                <tr>
                    <th>電話番号</th>
                    <td>{{data.deliveryOrderAddress.tel}}</td>
                </tr>
            </tbody>
        </table>

        <table>
            <caption>配送オプション</caption>
            <tbody>
                <tr>
                    <th>お届け日</th>
                    <td>{{data.deliveryOrder.deliveryDate}}</td>
                </tr>
                <tr>
                    <th>お届け時間</th>
                    <td>{{data.deliveryOrder.hourRange}}</td>
                </tr>
                <tr>
                    <th>明細書への金額印字</th>
                    <td>{{data.deliveryOrder.wrappingType}}</td>
                </tr>
            </tbody>
        </table>

        <table>
            <caption>お支払オプション</caption>
            <tbody>
                <tr>
                    <th>決済方法</th>
                    <td>{{data.paymentMethod}}</td>
                </tr>
                <tr>
                    <th>ポイント使用</th>
                    <td>{{data.inputUsagePoint}}</td>
                </tr>
                <tr>
                    <th>クーポンコード</th>
                    <td>{{data.serviceValues.campaignCode}}</td>
                </tr>
            </tbody>
        </table>

        <button @click="commit">Purchase</button>
    </template>
</template>

