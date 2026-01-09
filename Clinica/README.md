# Sistema de Clínica - Frontend

Frontend desarrollado con Vue 3, Vite, Vue Router y Axios.

## Características

- ✅ CRUD completo de Pacientes
- ✅ Creación de Citas con cascada (Servicio → Médico → Fecha → Hora)
- ✅ Listado de Citas con filtros
- ✅ Gestión de estados de citas (Programada, Atendida, Cancelada)
- ✅ Visualización de cupos disponibles por hora
- ✅ Búsqueda y paginación

## Instalación

```bash
npm install
```

## Desarrollo

```bash
npm run dev
```

La aplicación se abrirá en `http://localhost:5173`

## Configuración

Por defecto, la API se conecta a `https://localhost:44367/api`. 

Para cambiar la URL de la API, crea un archivo `.env`:

```
VITE_API_URL=https://localhost:44367/api
```

**Nota:** Si encuentras errores de certificado SSL en desarrollo, puedes aceptar el certificado en el navegador o usar HTTP en su lugar.

## Estructura

```
src/
├── api/           # Servicios API
├── components/    # Componentes reutilizables
├── views/         # Vistas/páginas
├── router/        # Configuración de rutas
├── App.vue        # Componente principal
└── style.css      # Estilos globales
```

## Rutas

- `/pacientes` - Listado de pacientes
- `/pacientes/nuevo` - Crear paciente
- `/pacientes/editar/:id` - Editar paciente
- `/citas` - Listado de citas
- `/citas/nueva` - Crear cita

