<template>
  <div class="p-4 bg-gray-950 min-h-screen">
    <div class="flex justify-between mb-4 items-center">
      <div class="flex gap-2">
        <button
          @click="router.push('/my-orders')"
          class="bg-gray-900 hover:bg-gray-700 text-white rounded px-4 py-2 shadow-md transition font-semibold"
        >
          Ver historial de órdenes
        </button>
        <button
          v-if="authStore.user && authStore.user.roles && Array.isArray(authStore.user.roles) && authStore.user.roles.some(r => r === 'Admin' || r === 'admin')"
          @click="router.push('/admin/products')"
          class="bg-gray-800 hover:bg-blue-800 text-white rounded px-4 py-2 shadow-md transition font-semibold"
        >
          Dashboard productos
        </button>
      </div>
      <button
        @click="cerrarSesion"
        class="bg-gray-800 hover:bg-red-600 text-white rounded px-4 py-2 shadow transition font-semibold"
      >
        Cerrar sesión
      </button>
    </div>
    <h1 class="text-3xl font-bold mb-6 text-white">Tienda</h1>
    <div v-if="error" class="mb-4 text-red-400">{{ error }}</div>
    <div v-if="loading" class="text-gray-400">Cargando productos...</div>
    <div v-else class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-8">
      <div
        v-for="producto in productos"
        :key="producto.id"
        class="bg-gray-900 bg-opacity-90 rounded-2xl shadow-xl border border-gray-700 p-5 flex flex-col cursor-pointer transition hover:scale-105 hover:shadow-blue-900 min-h-[420px] relative group"
        @click="abrirProducto(producto)"
      >
        <img v-if="producto.imageUrl" :src="producto.imageUrl" class="object-cover h-40 w-full mb-3 rounded-lg border shadow group-hover:shadow-lg group-hover:border-blue-700" />
        <img v-else src="/public/favicon.ico" alt="Sin imagen" class="object-cover h-40 w-full mb-3 rounded-lg border shadow opacity-30" />
        <div class="font-bold text-lg mb-2 text-white drop-shadow">{{ producto.name }}</div>
        <div class="mb-2 text-gray-300 italic opacity-90 line-clamp-3">{{ producto.description }}</div>
        <div class="mb-2 flex items-center">
          <span :class="{'animate-pulse text-yellow-400 font-black': stockAnimId === producto.id, 'text-gray-100': stockAnimId !== producto.id}" class="transition">Stock: {{ producto.stock }}</span>
        </div>
        <div class="mb-4 text-gray-100"></div>
        <button
          :disabled="producto.stock <= 0 || comprandoId === producto.id"
          @click.stop="abrirProducto(producto)"
          class="mt-auto bg-gray-100 text-gray-900 w-full py-2 px-4 rounded-lg hover:bg-white shadow disabled:opacity-60 transition font-semibold border border-gray-300"
        >
          Ver y Comprar
        </button>
        <div class="absolute top-2 right-2 bg-black/60 text-white text-xs px-3 py-1 rounded-lg shadow" v-if="producto.stock === 0">Agotado</div>
      </div>
    </div>

    <div v-if="showModal">
      <div class="fixed inset-0 flex items-center justify-center bg-black/80 z-30">
        <div class="bg-gray-900 bg-opacity-95 rounded-2xl shadow-2xl p-7 w-full max-w-md relative border-2 border-gray-700">
          <button @click="cerrarModal" class="absolute top-2 right-3 text-2xl text-gray-500 hover:text-red-400">&times;</button>
          <div class="mb-4">
            <img v-if="productoActivo?.imageUrl" :src="productoActivo.imageUrl" class="object-contain max-h-56 mx-auto rounded-lg mb-3 shadow border border-gray-700" />
            <img v-else src="/public/favicon.ico" alt="Sin imagen" class="object-contain max-h-56 mx-auto rounded-lg mb-3 opacity-30" />
            <div class="text-2xl font-bold mb-1 text-white">{{ productoActivo?.name }}</div>
            <div class="mb-2 text-gray-300 opacity-90">{{ productoActivo?.description }}</div>
            <div class="mb-2 text-gray-100">Precio: <span class="font-semibold">${{ productoActivo?.price }}</span></div>
            <div class="mb-2 text-gray-200">Stock: {{ productoActivo?.stock }}</div>
          </div>
          <form @submit.prevent="comprarProducto">
            <div class="mb-4">
              <label class="font-medium text-gray-200">Cantidad</label>
              <input type="number" min="1" :max="productoActivo?.stock ?? 1" v-model.number="cantidad" class="w-full border px-3 py-2 rounded bg-gray-800 text-white border-gray-700 focus:ring focus:ring-blue-900" required />
            </div>
            <div class="mb-4 text-red-400" v-if="modalError">{{ modalError }}</div>
            <button :disabled="comprandoModal" type="submit" class="w-full bg-gray-100 text-gray-900 font-bold py-2 px-4 rounded-lg shadow hover:bg-white transition border border-gray-300">
              <span v-if="comprandoModal">Comprando...</span>
              <span v-else>Comprar ahora</span>
            </button>
          </form>
          <button type="button" @click="cerrarModal" class="w-full mt-4 py-2 rounded bg-gray-200 text-gray-900 font-semibold">Volver</button>
          <div class="mt-3 text-green-400 text-center" v-if="modalSuccess">¡Compra exitosa!</div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted, watch } from 'vue';
import api from '../services/api';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';

const productos = ref([]);
const loading = ref(true);
const error = ref(null);
const router = useRouter();
const comprandoId = ref(null);
const authStore = useAuthStore();

const showModal = ref(false);
const productoActivo = ref(null);
const cantidad = ref(1);
const comprandoModal = ref(false);
const modalError = ref('');
const modalSuccess = ref(false);
const stockAnimId = ref(null);
// NADA de userIsAdmin helper, TODO directo.

async function syncUser() {
  await authStore.fetchUser();
}

onMounted(async () => {
  await syncUser();
  if (!authStore.isLoggedIn) {
    router.replace('/login');
    return;
  }
  await cargarProductos();
  console.log(authStore.user);
});

watch(() => router.currentRoute.value.fullPath, async (nuevo) => {
  if (nuevo === '/' || nuevo === '/admin/products' || nuevo === '/my-orders') {
    await syncUser();
  }
});

async function cargarProductos() {
  loading.value = true;
  try {
    const { data } = await api.get('/Products?pageNumber=1&pageSize=12');
    productos.value = data.items;
    error.value = null;
  } catch {
    error.value = 'No se pudieron cargar los productos.';
  } finally {
    loading.value = false;
  }
}

function abrirProducto(item) {
  productoActivo.value = item;
  cantidad.value = 1;
  modalError.value = '';
  modalSuccess.value = false;
  showModal.value = true;
}
function cerrarModal() {
  showModal.value = false;
  modalError.value = '';
  modalSuccess.value = false;
}

async function comprarProducto() {
  if (!productoActivo.value) return;
  comprandoModal.value = true;
  modalError.value = '';
  modalSuccess.value = false;
  try {
    await api.post('/Orders', {
      items: [{
        productId: productoActivo.value.id,
        quantity: cantidad.value,
      }]
    });
    modalSuccess.value = true;
    await cargarProductos();
    stockAnimId.value = productoActivo.value.id;
    setTimeout(() => {
      stockAnimId.value = null;
      cerrarModal();
    }, 1100);
  } catch {
    modalError.value = 'No se pudo realizar la compra.';
  } finally {
    comprandoModal.value = false;
  }
}

function cerrarSesion() {
  authStore.logout();
  router.replace('/login');
}
</script>
