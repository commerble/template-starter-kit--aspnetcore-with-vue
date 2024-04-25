<script setup lang="ts">
    import Cart from '@/components/Cart.vue'
    import { ref } from 'vue'
    import { useRoute, useRouter } from 'vue-router';
    import { checkout, checkoutAsGuest } from '@/commerble/api'
    import type { CommerblePurchaseError } from '../commerble/types';

    const route = useRoute();
    const router = useRouter();
    const login = () => router.push('/login');
    const next = () => router.push('/checkout/form');

    const cartId = 1;
    const asGuest = !!route.query.guest;
    const data = ref<null | CommerblePurchaseError>(null);


    const result = asGuest ? await checkoutAsGuest(cartId) : await checkout(cartId);

    if (result.type === 'next') {
        switch (result.next) {
            case 'site/login': login(); break;
            case 'purchase/shipping': next(); break;
            case 'purchase/confirm': next(); break;
            default: console.error(`'${result.next}' is not supported`);
        }
    }
    else {
        data.value = result;
    }
</script>

<template>
    <template v-if="data">
        <h2>Error</h2>
        <ul>
            <li v-for="err in data.errors" key="err.type">{{err.type}}</li>
        </ul>
    </template>
</template>
