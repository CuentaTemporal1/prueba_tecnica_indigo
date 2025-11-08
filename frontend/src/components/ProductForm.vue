<template>
  <form @submit.prevent="onSubmit" class="space-y-4">
    <div>
      <label class="font-medium text-gray-200">Nombre <span class="text-red-400">*</span></label>
      <input v-model="name" type="text" class="w-full border border-gray-800 px-3 py-2 rounded bg-gray-800 text-white" :class="{'border-red-400': nombreError}"/>
      <div v-if="nombreError" class="text-red-400 text-xs mt-1">El nombre es obligatorio.</div>
    </div>
    <div>
      <label class="font-medium text-gray-200">Descripción <span class="text-red-400">*</span></label>
      <textarea v-model="description" class="w-full border border-gray-800 px-3 py-2 rounded bg-gray-800 text-white" :class="{'border-red-400': descripcionError}"></textarea>
      <div v-if="descripcionError" class="text-red-400 text-xs mt-1">La descripción es obligatoria.</div>
    </div>
    <div class="flex gap-3">
      <div class="flex-1">
        <label class="font-medium text-gray-200">Precio <span class="text-red-400">*</span></label>
        <input v-model.number="price" type="number" min="0" step="0.01" class="w-full border border-gray-800 px-3 py-2 rounded bg-gray-800 text-white" :class="{'border-red-400': precioError}" />
        <div v-if="precioError" class="text-red-400 text-xs mt-1">El precio es obligatorio y debe ser mayor a 0.</div>
      </div>
      <div class="flex-1">
        <label class="font-medium text-gray-200">Stock <span class="text-red-400">*</span></label>
        <input v-model.number="stock" type="number" min="0" step="1" class="w-full border border-gray-800 px-3 py-2 rounded bg-gray-800 text-white" :class="{'border-red-400': stockError}" />
        <div v-if="stockError" class="text-red-400 text-xs mt-1">El stock es obligatorio y debe ser mayor o igual a 0.</div>
      </div>
    </div>
    <div>
      <label class="font-medium text-gray-200">Imagen <span class="text-red-400">*</span></label>
      <input type="file" accept="image/*" @change="onFileChange" class="block text-gray-800 bg-white" />
      <div v-if="imgError" class="text-red-400 text-xs mt-1">La imagen es obligatoria.</div>
      <div v-if="imageUrlPreview" class="mt-2">
        <img :src="imageUrlPreview" class="h-24 rounded border border-gray-700 object-contain" />
      </div>
    </div>
    <div v-if="error" class="text-red-400">{{ error }}</div>
    <div v-if="erroresGenerales.length > 0" class="bg-red-900/30 border border-red-500 rounded p-3">
      <div class="text-red-400 font-semibold mb-2">Faltan los siguientes campos:</div>
      <ul class="list-disc list-inside text-red-300 text-sm space-y-1">
        <li v-for="err in erroresGenerales" :key="err">{{ err }}</li>
      </ul>
    </div>
    <div class="flex justify-end gap-2 pt-2">
      <button type="button" class="px-4 py-2 rounded bg-gray-200 text-gray-900 font-semibold" @click="$emit('close')">Volver</button>
      <button :disabled="loading || tieneErrores" type="submit" class="px-4 py-2 bg-gray-100 text-gray-900 font-bold rounded shadow disabled:opacity-70">
        <span v-if="loading">Guardando...</span>
        <span v-else>{{ productToEdit ? 'Actualizar' : 'Crear' }}</span>
      </button>
    </div>
  </form>
</template>
<script setup>
import { ref, watch, computed } from 'vue';
import api from '../services/api';
const props = defineProps({ productToEdit: { type: Object, default: null } });
const emit = defineEmits(['success','close']);

const name = ref('');
const description = ref('');
const price = ref('');
const stock = ref('');
const imageFile = ref(null);
const imageUrlPreview = ref('');
const error = ref(null);
const loading = ref(false);
const nombreError = ref(false);
const descripcionError = ref(false);
const precioError = ref(false);
const stockError = ref(false);
const imgError = ref(false);

const erroresGenerales = computed(() => {
  const errores = [];
  if (nombreError.value) errores.push('Nombre');
  if (descripcionError.value) errores.push('Descripción');
  if (precioError.value) errores.push('Precio');
  if (stockError.value) errores.push('Stock');
  if (imgError.value) errores.push('Imagen');
  return errores;
});

const tieneErrores = computed(() => {
  return nombreError.value || descripcionError.value || precioError.value || stockError.value || imgError.value;
});

watch(() => props.productToEdit, (nuevo) => {
  if (nuevo) {
    name.value = nuevo.name || '';
    description.value = nuevo.description || '';
    price.value = nuevo.price || 0;
    stock.value = nuevo.stock || 0;
    imageUrlPreview.value = nuevo.imageUrl || '';
    imageFile.value = null;
  } else {
    name.value = '';
    description.value = '';
    price.value = 0;
    stock.value = 0;
    imageFile.value = null;
    imageUrlPreview.value = '';
  }
  nombreError.value = false;
  descripcionError.value = false;
  precioError.value = false;
  stockError.value = false;
  imgError.value = false;
}, { immediate: true });

function onFileChange(e) {
  const file = e.target.files[0];
  imageFile.value = file;
  if (file) {
    const reader = new FileReader();
    reader.onload = e => imageUrlPreview.value = e.target.result;
    reader.readAsDataURL(file);
    imgError.value = false;
  } else {
    // Si está editando, respetar imagen existente si la hay
    if (props.productToEdit?.imageUrl) {
      imageUrlPreview.value = props.productToEdit.imageUrl;
      imgError.value = false;
    } else {
      imageUrlPreview.value = '';
      imgError.value = true;
    }
  }
}

async function onSubmit() {
  // Validar todos los campos
  nombreError.value = !name.value || !name.value.trim();
  descripcionError.value = !description.value || !description.value.trim();
  precioError.value = !price.value || price.value <= 0 || isNaN(price.value);
  stockError.value = stock.value === null || stock.value === undefined || stock.value < 0 || isNaN(stock.value);
  imgError.value = !imageFile.value && !(props.productToEdit && props.productToEdit.imageUrl);
  
  if (tieneErrores.value) {
    const mensajes = [];
    if (nombreError.value) mensajes.push('El nombre es obligatorio');
    if (descripcionError.value) mensajes.push('La descripción es obligatoria');
    if (precioError.value) mensajes.push('El precio es obligatorio y debe ser mayor a 0');
    if (stockError.value) mensajes.push('El stock es obligatorio y debe ser mayor o igual a 0');
    if (imgError.value) mensajes.push('La imagen es obligatoria. Debes subir una imagen para guardar el producto');
    
    alert('⚠️ Faltan campos obligatorios:\n\n' + mensajes.join('\n'));
    return;
  }
  
  error.value = null;
  loading.value = true;
  const formData = new FormData();
  formData.append('Name', name.value.trim());
  formData.append('Description', description.value.trim());
  formData.append('Price', price.value);
  formData.append('Stock', stock.value);
  if (imageFile.value) {
    formData.append('Image', imageFile.value);
  }
  try {
    if (props.productToEdit) {
      await api.put(`/Products/${props.productToEdit.id}`, formData);
    } else {
      await api.post('/Products', formData);
    }
    emit('success');
  } catch (e) {
    // Mensajes de error más específicos
    let mensajeError = 'No se guardó el producto.';
    if (e.response) {
      if (e.response.status === 400) {
        mensajeError = 'Error: Los datos enviados no son válidos. Verifica que todos los campos estén correctos.';
      } else if (e.response.status === 401) {
        mensajeError = 'Error: No tienes autorización para realizar esta acción.';
      } else if (e.response.data && e.response.data.message) {
        mensajeError = `Error: ${e.response.data.message}`;
      }
    }
    error.value = mensajeError;
  } finally {
    loading.value = false;
  }
}
</script>
