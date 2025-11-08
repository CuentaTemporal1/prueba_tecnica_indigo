import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/LoginView.vue'),
  },
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/HomeView.vue'),
  },
  {
    path: '/my-orders',
    name: 'MyOrders',
    component: () => import('../views/MyOrdersView.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/admin/products',
    name: 'AdminProducts',
    component: () => import('../views/AdminProductsView.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  
  // SIEMPRE cargar usuario si no está logueado (excepto en login)
  if (to.path !== '/login' && !authStore.isLoggedIn) {
    await authStore.fetchUser();
  }

  if (to.meta.requiresAuth && !authStore.isLoggedIn) {
    return next('/login');
  }
  
  // Verificación robusta de admin
  if (to.meta.requiresAdmin) {
    // Si no hay usuario, redirigir
    if (!authStore.user) {
      return next('/');
    }
    // Si no hay roles o no es array, redirigir
    if (!authStore.user.roles || !Array.isArray(authStore.user.roles)) {
      return next('/');
    }
    // Verificar si tiene rol Admin (case-insensitive)
    const hasAdminRole = authStore.user.roles.some(role => 
      role === 'Admin' || role === 'admin' || role.toLowerCase() === 'admin'
    );
    if (!hasAdminRole) {
      return next('/');
    }
  }
  
  return next();
});

export default router;
