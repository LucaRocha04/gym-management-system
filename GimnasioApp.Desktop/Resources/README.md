# Instrucciones para agregar la imagen de fondo del gimnasio

Para agregar la imagen de fondo del gimnasio:

1. Guarda la imagen del gimnasio como "gym_background.jpg" en la carpeta:
   GimnasioApp.Desktop\bin\Debug\net8.0-windows\Resources\

2. La imagen se aplicará automáticamente como fondo en todas las pantallas principales de los usuarios (Admin, Profesor, Recepcionista).

3. Si la imagen no se encuentra, el sistema usará el color de fondo por defecto.

Características de la implementación:
- Imagen de fondo con layout "Stretch" para cubrir toda la pantalla
- Panel principal semi-transparente para que se vea el fondo
- Funciona en las 3 pantallas principales de cada rol de usuario
- Manejo de errores incluido