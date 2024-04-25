<script setup lang="ts">
    import Cart from '@/components/Cart.vue'
    import { ref } from 'vue'
    import { useRoute, useRouter } from 'vue-router';
    import { checkout, checkoutAsGuest, getPurchaseComplete } from '@/commerble/api'
    import type { CommerblePurchaseComplete } from '../commerble/types';

    type Props = {
        orderId: number
    }

    const props = defineProps<Props>();

    const router = useRouter();
    const login = () => router.push('/login');

    const cartId = 1;
    const data = ref<null | CommerblePurchaseComplete>(null);


    const result = await getPurchaseComplete(cartId, props.orderId);

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
        <h2>Complete :)</h2>
    </template>
</template>
