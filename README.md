# TurnosTutoriasWCF

Esta solución contiene dos proyectos principales:
- **Service**: Biblioteca WCF que expone los servicios necesarios para la gestión de turnos de tutorías.
- **Client**: Proyecto de consola (o aplicación de escritorio) que consume y prueba el servicio WCF.

## Estructura de carpetas

- **Service/Service.Contracts**  
  Contratos de servicio (interfaces, DataContracts, DTOs).

- **Service/Service.Implementations**  
  Clases que implementan los contratos WCF.

- **Service/Data**  
  Capa de acceso a datos (Entity Framework, modelos de datos, repositorios).

- **Service/ServiceHost**  
  Proyecto host (aplicación de consola) para levantar el servicio localmente.

- **Client**  
  Proyecto cliente (consola/WPF/WinForms) que consume el servicio.

## Instrucciones para compilar y ejecutar

1. Abrir la solución `TurnosTutoriasWCF.sln` en Visual Studio 2019 (o superior).  
2. Establecer como proyecto de inicio `ServiceHost` para arrancar el servicio WCF.  
3. Ejecutar el Cliente para probar los métodos expuestos.  
4. Configurar cadenas de conexión en `App.config` (carpeta Data) antes de ejecutar por primera vez.

## Rúbrica y criterios de evaluación

- Repositorio en GitHub con commits periódicos.  
- Cumplimiento de validaciones, manejo de errores y pruebas unitarias.  
- Documentación básica de cada servicio (XML comments).  
- Manejo de excepciones personalizadas y registros (logs).

*(Completar con más detalles a medida que avances en el proyecto.)*
