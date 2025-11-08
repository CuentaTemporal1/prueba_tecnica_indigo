# 🤖 AI-Generated Doc: Un Sistema de Inventario y Pedidos

Este repositorio contiene la solución completa para un sistema de gestión de inventario y pedidos diseñado con una arquitectura **Clean Architecture** y *deployable* en un único entorno de contenedores.

El foco de esta solución está en demostrar la **separación de responsabilidades**, la **seguridad del manejo de sesión** (JWT vía Cookies HttpOnly), y el uso de patrones de diseño modernos.

---

## 🏛️ Descripción General de la Arquitectura

La aplicación está montada como un sistema distribuido simple, orquestado por Docker Compose:

1.  **Backend (.NET 8):** Implementa la lógica de negocio y se conecta a la base de datos.
2.  **Frontend (Vue 3/Vite):** Aplicación de una sola página (SPA) que consume la API.
3.  **Base de Datos (SQL Server):** Almacena todos los datos de la aplicación.

La comunicación entre el Frontend y el Backend se realiza mediante cookies de sesión, asegurando que el *token* JWT nunca sea accesible desde JavaScript del lado del cliente (prevención de XSS).

---

## 💻 Requisitos Previos

Para ejecutar la aplicación localmente, solo necesita tener instalado lo siguiente:

1.  **Git**
2.  **Docker Desktop** (Debe estar iniciado y ejecutándose).
3.  **Node.js** (Para instalar dependencias del frontend, aunque Docker lo compilará).

---

## 🚀 Despliegue Rápido (Docker Compose)

El sistema completo se construye e inicia con un solo comando.

1.  **Clonar el Repositorio:**
    ```
    git clone [URL_DE_TU_REPOSITORIO]
    cd [CARPETA_RAIZ_DEL_PROYECTO] 
    ```

2.  **Configuración de Variables de Entorno:**
    * Cree un archivo llamado **.env** en la raíz del proyecto.
    * Este archivo debe contener las claves de conexión a Azure y la contraseña de la base de datos.

    > **Nota:** El archivo .env ya debe tener las variables DB_PASSWORD, BLOB_CONNECTION_STRING, JWT_KEY, etc.

3.  **Construir e Iniciar el Sistema:**
    Ejecute este comando desde la raíz del proyecto (donde se encuentran el docker-compose.yml y las carpetas backend/ y frontend/):
    ```
    docker-compose up --build
    ```
    *(Este proceso tardará unos minutos la primera vez, ya que construye el backend C# y el frontend Vue).*

4.  **Acceso a la Aplicación:**
    Una vez que los contenedores estén corriendo:
    * **Frontend (App de Usuario):** http://localhost (o http://localhost:80)
    * **Backend (Swagger/OpenAPI):** http://localhost:8080/swagger/index.html

---

## 🔑 Credenciales de Prueba (Seeded Data)

El sistema crea automáticamente un usuario administrador al iniciar la base de datos.

| Rol | Email | Contraseña |
| :--- | :--- | :--- |
| **Administrador** | admin@bohorquez.com | Admin123! |
| **Usuario** | (No se crea por defecto, pero se puede registrar mediante la API) | |

---

## 🛑 Detener y Limpiar

Para detener y eliminar los contenedores y los datos persistentes (bases de datos), use el siguiente comando:

docker-compose down -v

El flag -v es crucial para eliminar el volumen de la base de datos y asegurar una instalación limpia en el futuro.