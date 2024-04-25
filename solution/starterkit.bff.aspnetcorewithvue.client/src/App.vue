<script setup lang="ts">
    import { RouterView } from 'vue-router'
    import { Suspense } from 'vue'
</script>

<template>
    <main>
        <h1>Rendered Vue SPA</h1>
        <Suspense>
            <RouterView />
            <template #fallback>
                Loading...
            </template>
        </Suspense>
        <a href="/">TOP</a>
    </main>
</template>

<style scoped>
</style>
