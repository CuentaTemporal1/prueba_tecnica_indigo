<template>
  <div v-if="!authStore.isLoggedIn" class="min-h-screen flex items-center justify-center bg-gray-900">
    <form @submit.prevent="onSubmit" class="bg-gray-950 shadow-lg p-8 rounded-xl w-full max-w-sm border border-gray-800">
      <h2 class="text-2xl font-bold mb-6 text-center text-white">Iniciar Sesión</h2>
      <div v-if="error" class="mb-4 text-red-400 text-sm">{{ error }}</div>
      <div class="mb-4">
        <label class="block mb-2 text-gray-200">Email</label>
        <input v-model="email" type="email" class="w-full border border-gray-700 bg-gray-800 text-white px-3 py-2 rounded focus:outline-none focus:ring" required />
      </div>
      <div class="mb-6">
        <label class="block mb-2 text-gray-200">Contraseña</label>
        <input v-model="password" type="password" class="w-full border border-gray-700 bg-gray-800 text-white px-3 py-2 rounded focus:outline-none focus:ring" required />
      </div>
      <button :disabled="loading" type="submit" class="w-full bg-gray-800 hover:bg-gray-700 text-white font-bold py-2 px-4 rounded transition disabled:opacity-60">
        <span v-if="loading">Entrando...</span>
        <span v-else>Entrar</span>
      </button>
    </form>
  </div>
</template>

<script setup>
import { ref, watchEffect } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';

const email = ref('');
const password = ref('');
const error = ref(null);
const loading = ref(false);
const router = useRouter();
const authStore = useAuthStore();

watchEffect(() => {
  if (authStore.isLoggedIn) {
    router.replace('/');
  }
});

async function onSubmit() {
  error.value = null;
  loading.value = true;
  const [ok, role] = await authStore.login({ email: email.value, password: password.value });
  loading.value = false;

  if (ok) {
    router.push('/');
  } else {
    error.value = 'Credenciales inválidas o error del servidor.';
  }
}
</script>
