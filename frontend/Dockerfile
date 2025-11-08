# ----------------------------------------------------------------------
# ETAPA 1: BUILDER (Construcción de la aplicación Vue)
# ----------------------------------------------------------------------
FROM node:20-alpine AS builder

# Establece el directorio de trabajo dentro del contenedor
WORKDIR /app

# Copia los archivos de configuración de dependencias (package.json, etc.)
COPY package*.json ./

# Instala las dependencias, incluyendo las de desarrollo
RUN npm install

# Copia el resto de los archivos de la aplicación
COPY . .

# Ejecuta el comando de construcción de Vue/Vite.
RUN npm run build

# ----------------------------------------------------------------------
# ETAPA 2: FINAL (Servidor de los archivos estáticos)
# ----------------------------------------------------------------------
FROM node:20-alpine AS final

# Establece el directorio de trabajo
WORKDIR /usr/src/app

# Copia el archivo static.js, que es tu servidor de Express
COPY static.js .

# Copia solo los archivos estáticos (el resultado de 'npm run build') desde la etapa 'builder'
COPY --from=builder /app/dist ./dist

# Instala solo las dependencias de producción para el servidor Express
# Copiamos package.json aquí para instalar solo prod deps
COPY package.json .
RUN npm install --only=production

# Expone el puerto 80 (para coincidir con el docker-compose)
EXPOSE 80

# Comando para iniciar el servidor usando static.js (CORREGIDO)
CMD ["node", "static.js"]