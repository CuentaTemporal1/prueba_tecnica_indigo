<template>
  <div class="p-4 bg-gray-950 min-h-screen">
    <div class="flex justify-between mb-4 items-center">
      <button @click="router.push('/')" class="bg-gray-900 hover:bg-gray-700 text-white rounded px-4 py-2 shadow-md font-semibold transition">Volver</button>
      <div></div>
    </div>
    <h1 class="text-2xl font-bold mb-6 text-white">Administrar Productos</h1>
    <div class="mb-4 flex justify-end">
      <button @click="abrirNuevo" class="bg-gray-800 hover:bg-blue-700 text-white px-4 py-2 rounded shadow">Crear Nuevo Producto</button>
    </div>
    <div v-if="error" class="text-red-400 mb-4">{{ error }}</div>
    <div v-if="loading" class="text-gray-400">Cargando...</div>
    <table v-else class="min-w-full border text-center bg-gray-900 border-gray-700 rounded-2xl overflow-hidden shadow-xl">
      <thead>
        <tr class="bg-gray-800 text-white">
          <th class="py-2 px-3">ID</th>
          <th class="py-2 px-3">Nombre</th>
          <th class="py-2 px-3">Precio</th>
          <th class="py-2 px-3">Stock</th>
          <th class="py-2 px-3">Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="producto in productos" :key="producto.id" class="border-t border-gray-800 hover:bg-gray-800 transition">
          <td class="text-gray-100">{{ producto.id }}</td>
          <td class="text-white">{{ producto.name }}</td>
          <td class="text-gray-200">${{ producto.price }}</td>
          <td class="text-gray-300">{{ producto.stock }}</td>
          <td>
            <button @click="editar(producto)" class="bg-yellow-500 hover:bg-yellow-400 text-gray-900 text-sm px-2 py-1 rounded mr-2 font-bold">Editar</button>
            <button @click="eliminar(producto.id)" class="bg-red-600 hover:bg-red-500 text-white text-sm px-2 py-1 rounded font-bold">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>
    <div v-if="showModal">
      <div class="fixed inset-0 flex items-center justify-center bg-black/80 z-40">
        <div class="bg-gray-950 rounded-2xl shadow-2xl p-7 min-w-[340px] border-2 border-gray-700">
          <ProductForm :productToEdit="productoEditando" @success="actualizarYCerrar" @close="showModal = false" />
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue';
import api from '../services/api';
import ProductForm from '../components/ProductForm.vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';
const productos = ref([]);
const loading = ref(true);
const error = ref(null);
const showModal = ref(false);
const productoEditando = ref(null);
const router = useRouter();
const authStore = useAuthStore();

onMounted(async () => {
  if (!authStore.isLoggedIn) {
    await authStore.fetchUser();
    if (!authStore.isLoggedIn) {
      router.replace('/login');
      return;
    }
  }
  await cargarProductos();
});

async function cargarProductos() {
  loading.value = true;
  try {
    const { data } = await api.get('/Products?pageNumber=1&pageSize=30');
    productos.value = data.items;
    error.value = null;
  } catch {
    error.value = 'Error cargando productos';
  } finally {
    loading.value = false;
  }
}

function abrirNuevo() {
  productoEditando.value = null;
  showModal.value = true;
}

function editar(prod) {
  productoEditando.value = { ...prod };
  showModal.value = true;
}

async function eliminar(id) {
  if (confirm('¿Seguro de eliminar este producto?')) {
    try {
      await api.delete(`/Products/${id}`);
      await cargarProductos();
    } catch {
      alert('No se pudo eliminar.');
    }
  }
}

function actualizarYCerrar() {
  showModal.value = false;
  cargarProductos();
}
</script>
