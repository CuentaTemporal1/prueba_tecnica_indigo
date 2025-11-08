<template>
  <div class="p-4 bg-gray-950 min-h-screen">
    <div class="flex justify-between mb-6 items-center">
      <h1 class="text-2xl font-bold text-gray-100">Mis Pedidos</h1>
      <button
        @click="cerrarSesion"
        class="bg-gray-800 hover:bg-red-600 text-white rounded px-4 py-2 shadow transition font-semibold"
      >
        Cerrar sesión
      </button>
    </div>
    <div v-if="error" class="text-red-400 mb-4">{{ error }}</div>
    <div v-if="loading" class="text-gray-400">Cargando pedidos...</div>
    <div v-else>
      <div v-if="orders.length === 0" class="text-gray-500">No tienes pedidos.</div>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div v-for="order in orders" :key="order?.id" class="bg-gray-900 bg-opacity-90 rounded-xl shadow-lg border border-gray-700 p-6 mb-2">
          <div class="mb-2 font-semibold text-white">Pedido #{{ order.id }}</div>
          <div class="mb-2 text-gray-400 text-xs">({{ order.orderItems.length }} producto{{ order.orderItems.length > 1 ? 's' : '' }})</div>
          <ul>
            <li v-for="item in order.orderItems" :key="item.productId" class="flex items-center gap-2 mb-2 bg-gray-800 rounded p-2 shadow-sm">
              <div class="flex-1">
                <span class="block font-medium text-white">Producto: {{ item.productName || item.productId }}</span>
                <span class="block text-sm text-gray-300">Cantidad: {{ item.quantity }}</span>
              </div>
              <button 
                @click="verProducto(item.productId)" 
                class="bg-white text-dark  hover:bg-gray-200 text-sm rounded px-3 py-1 transition font-semibold"
              >
                Ver producto
              </button>
            </li>
          </ul>
        </div>
      </div>
    </div>

    <div v-if="showModal">
      <div class="fixed inset-0 flex items-center justify-center bg-black/80 z-40">
        <div class="bg-gray-900 bg-opacity-95 rounded-xl shadow-2xl p-6 max-w-md w-full relative border-2 border-blue-900">
          <button @click="cerrarModal" class="absolute top-2 right-3 text-2xl text-gray-500 hover:text-red-400">&times;</button>
          <template v-if="productoModal">
            <img v-if="productoModal.imageUrl" :src="productoModal.imageUrl" class="mb-4 rounded-lg mx-auto object-contain max-h-56 shadow border border-gray-700" />
            <div class="text-xl font-bold mb-2 text-white">{{ productoModal.name }}</div>
            <div class="mb-1 text-gray-200">{{ productoModal.description }}</div>
            <div class="mb-1 text-gray-100">Precio: <span class="font-semibold">${{ productoModal.price }}</span></div>
            <div class="mb-1 text-gray-200">Stock: {{ productoModal.stock }}</div>
          </template>
          <div v-else class="text-gray-500">Cargando producto...</div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue';
import api from '../services/api';
import { useAuthStore } from '../stores/auth';
import { useRouter } from 'vue-router';
const orders = ref([]);
const loading = ref(true);
const error = ref(null);
const authStore = useAuthStore();
const router = useRouter();

const showModal = ref(false);
const productoModal = ref(null);

onMounted(async () => {
  await cargarPedidos();
});

async function cargarPedidos() {
  loading.value = true;
  try {
    const { data } = await api.get('/Orders/my-history');
    if (Array.isArray(data)) {
      orders.value = data;
      loading.value = false;
    } else {
      orders.value = [];
    }
    error.value = null;
  } catch (err) {
    if (err && err.response && err.response.status === 401) {
      error.value = 'No autorizado. Inicie sesión nuevamente.';
      authStore.logout();
      router.replace('/login');
    } else {
      error.value = 'No se pudieron cargar los pedidos.';
    }
    orders.value = [];
  } finally {
    loading.value = false;
  }
}

async function verProducto(id) {
  showModal.value = true;
  productoModal.value = null;
  try {
    const { data } = await api.get(`/Products/${id}`);
    productoModal.value = data;
  } catch {
    productoModal.value = null;
  }
}
function cerrarModal() {
  showModal.value = false;
  productoModal.value = null;
}
function cerrarSesion() {
  authStore.logout();
  router.replace('/login');
}
</script>
