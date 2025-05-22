# 🖼 AutoThumbnail MP4 - Miniaturas Automáticas para Videos

Este programa de consola en **C#** permite modificar la miniatura de un archivo de video en **MP4** de manera automática. Utiliza **FFmpeg** para extraer imágenes en los tiempos **10%, 20% y 70%** de la duración total del video y permite al usuario elegir una miniatura para incrustarla.

## 🚀 Características
- 📸 **Extracción automática de miniaturas** en momentos clave del video.
- 🔎 **Selección de miniatura por parte del usuario**.
- 🎭 **Incorporación correcta de la miniatura** en el archivo MP4 con `attached_pic`.
- 🗑 **Eliminación de archivos temporales**, manteniendo solo el video final.
- ✂️ **Reemplazo automático del archivo original** con el video modificado.
- 🔄 **Ejecución continua**, sin necesidad de confirmación, hasta que se cierre la ventana.

## 📋 Requisitos
- Tener **FFmpeg** instalado y agregado al **PATH** del sistema.

## 🛠 Instalación
1. **Clona el repositorio**
   ```sh
   git clone https://github.com/TU-USUARIO/AutoThumbnail-MP4.git
   cd AutoThumbnail-MP4
