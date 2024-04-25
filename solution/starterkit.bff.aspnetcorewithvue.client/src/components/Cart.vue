<script setup lang="ts">
    import type { CommerbleCart } from '@/commerble/types'

    const props = defineProps<Pick<CommerbleCart, 'items'|'state'>>()
</script>

<template>
    <ul>
        <li v-for="err in props.state.errors" key="err">{{err}}</li>
    </ul>
    <table>
        <thead>
            <tr>
                <th>ID</th>
                <th>Name</th>
                <th>UnitPrice</th>
                <th>Quantity</th>
                <th>LinePrice</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="item in props.items" key="item.productId">
                <td>{{item.productId}}</td>
                <td>{{item.productName}}</td>
                <td>{{item.unitPriceWithTax}}</td>
                <td>{{item.requestAmount}}</td>
                <td>{{item.linePrice}}</td>
            </tr>
        </tbody>
    </table>
</template>

<style scoped>
</style>
