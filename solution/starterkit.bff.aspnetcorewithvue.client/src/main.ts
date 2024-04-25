import './assets/main.css'

import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'

import App from '@/App.vue'

import CheckoutInitView from '@/views/CheckoutInitView.vue'
import CheckoutFormView from '@/views/CheckoutFormView.vue'
import CheckoutView from '@/views/CheckoutView.vue'
import CompleteView from '@/views/CompleteView.vue'
import LoginView from '@/views/LoginView.vue'

const routes = [
  { path: '/checkout/init', component: CheckoutInitView },
  { path: '/checkout/form', component: CheckoutFormView },
  { path: '/checkout', component: CheckoutView },
  { path: '/complete/:orderId', component: CompleteView, props: true },
  { path: '/login', component: LoginView },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

createApp(App).use(router).mount('#app')
