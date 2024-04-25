<script setup lang="ts">
    import { useRouter } from "vue-router"

    const router = useRouter();
    const loginAsGuest = () => {
        router.push('/checkout/init?guest=true');
    }
</script>

<template>
    <h2>Login</h2>
    <button @click="loginAsGuest">Buy as Guest</button>
</template>

<style scoped>
</style>
