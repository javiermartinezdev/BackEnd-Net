Este repositorio contiene una serie de prácticas para aprender y desarrollar APIs REST con .Net. Se incluyen ejercicios de configuración del entorno, creación de modelos, autenticación, manejo de vistas, serialización de datos y pruebas. Ideal para quienes desean profundizar en el desarrollo backend con .Net.

### Tecnologías utilizadas
- **.NET**
- **C#**
- **Postgrest**

### Requisitos Previos:
Antes de ejecutar el proyecto, es necesario instalar **.NET**. Puedes descargarlo e instalarlo desde el [sitio web oficial](https://dotnet.microsoft.com/es-es/download). Ademas de instalar Postgrest en tu computadora.

### Pasos para Ejecutar el Proyecto:
1. **Abrir la Terminal**:
   - Abre una terminal o línea de comandos en tu computadora.
  
2. **Navegar al Directorio del Proyecto**:
   - Utiliza el comando `cd` para cambiar al directorio donde se encuentra tu proyecto. Debes estar en el mismo directorio que el archivo `Program.cs`.
   - Ejemplo: Si tu proyecto está en el escritorio, podrías escribir:
     ```bash
     cd Desktop/BackEnd-Net
     ```
3. **Ejecutar la migracion de BD**:
    - Utiliza el comando `dotnet ef migrations add PrimeraMigracionUsuarios --context DataContex` para preparar la primera migracion. 

    - Utiliza el comando `dotnet ef migrations add PrimeraMigracionProductos --context DataContextProduct` para preparar la segunda migracion. 
    
    - Despues ejecuta las migraciones con los comandos
      `dotnet ef database update --context DataContext` para crear la tabla Users.
      `dotnet ef database update --context DataContextProduct` para crear la tabla. Products.

4. **Ejecutar Proyecto**:
    - Finalmente ejecuta el programa `Program.cs` `con el siguiente comando:
        ```bash
        dotnet run
        ```
    - Este comando iniciará el proyecto
  
5. **Disfruta**.

## Contacto para el proyecto
Si estás interesado en cómo se realizó este proyecto paso a paso por favor contacta a:

**Dr. José Luis Quiroz Fabian**
- **Número de cubículo**: T-169
- **Ubicación**: Edificio T de CBI, Universidad Autónoma Metropolitana Unidad Iztapalapa
- **Telefono**: 58 04 46 00 ext. 1169


Realizado por:
José Luis Quiroz Fabian. 
Javier Clara Martinez.
Fernando Octavio Arroyo Velasco.
