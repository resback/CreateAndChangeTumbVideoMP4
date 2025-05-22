using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main()
    {
        while (true) // Mantener el programa en bucle infinito hasta que el usuario cierre la ventana
        {
            Console.Write("Ingrese la ruta del video en MP4: ");
            string videoPath = Console.ReadLine();

            // Limpiar la ruta de comillas dobles si es necesario
            videoPath = CleanPath(videoPath);

            if (!File.Exists(videoPath))
            {
                Console.WriteLine("El archivo no existe.");
                continue;
            }

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(videoPath);
            string outputDir = Path.GetDirectoryName(videoPath);
            int[] percentages = { 10, 20, 70 };
            string[] imagePaths = new string[percentages.Length];

            // Obtener la duración del video
            double duration = GetVideoDuration(videoPath);
            if (duration <= 0)
            {
                Console.WriteLine("No se pudo obtener la duración del video.");
                continue;
            }

            // Extraer imágenes según los tiempos calculados
            for (int i = 0; i < percentages.Length; i++)
            {
                double timestamp = (duration * percentages[i]) / 100;
                string imagePath = Path.Combine(outputDir, $"{fileNameWithoutExt}_{i + 1}.jpg");
                CaptureThumbnail(videoPath, imagePath, timestamp);
                imagePaths[i] = imagePath;
            }

            // Permitir al usuario elegir una imagen
            Console.WriteLine("Seleccione la miniatura a establecer (1, 2 o 3): ");
            foreach (var img in imagePaths) Console.WriteLine(img);
            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > imagePaths.Length)
            {
                Console.WriteLine("Opción inválida.");
                continue;
            }

            string outputVideoPath = SetThumbnail(videoPath, imagePaths[choice - 1]);

            // Eliminar todas las miniaturas generadas
            DeleteTemporaryImages(imagePaths);

            // Borrar el archivo original y renombrar el nuevo con el nombre original
            ReplaceOriginalVideo(videoPath, outputVideoPath);

            Console.WriteLine($"Proceso completado. Se ha reemplazado el video original con la nueva versión.");
        }
    }

    static string CleanPath(string path)
    {
        if (!string.IsNullOrEmpty(path) && path.Length > 1 && path.StartsWith("\"") && path.EndsWith("\""))
        {
            path = path.Substring(1, path.Length - 2);
        }
        return path;
    }

    static double GetVideoDuration(string videoPath)
    {
        ProcessStartInfo psi = new ProcessStartInfo("ffmpeg", $"-i \"{videoPath}\"")
        {
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(psi))
        {
            using (StreamReader reader = process.StandardError)
            {
                string output = reader.ReadToEnd();
                process.WaitForExit();

                var match = System.Text.RegularExpressions.Regex.Match(output, @"Duration: (\d+):(\d+):(\d+\.\d+)");
                if (match.Success)
                {
                    int hours = int.Parse(match.Groups[1].Value);
                    int minutes = int.Parse(match.Groups[2].Value);
                    double seconds = double.Parse(match.Groups[3].Value);
                    return hours * 3600 + minutes * 60 + seconds;
                }
                return -1;
            }
        }
    }

    static void CaptureThumbnail(string videoPath, string imagePath, double timestamp)
    {
        ProcessStartInfo psi = new ProcessStartInfo("ffmpeg", $"-ss {timestamp} -i \"{videoPath}\" -frames:v 1 \"{imagePath}\"")
        {
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process.Start(psi).WaitForExit();
    }

    static string SetThumbnail(string videoPath, string imagePath)
    {
        string outputVideoPath = Path.Combine(Path.GetDirectoryName(videoPath), $"{Path.GetFileNameWithoutExtension(videoPath)}_thumb.mp4");
        ProcessStartInfo psi = new ProcessStartInfo("ffmpeg", $"-i \"{videoPath}\" -i \"{imagePath}\" -map 0 -map 1 -c copy -disposition:v:1 attached_pic \"{outputVideoPath}\"")
        {
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process.Start(psi).WaitForExit();
        return outputVideoPath;
    }

    static void DeleteTemporaryImages(string[] imagePaths)
    {
        foreach (string imagePath in imagePaths)
        {
            try
            {
                File.Delete(imagePath);
                Console.WriteLine($"Imagen eliminada: {imagePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar {imagePath}: {ex.Message}");
            }
        }
    }

    static void ReplaceOriginalVideo(string originalPath, string newPath)
    {
        try
        {
            File.Delete(originalPath);
            File.Move(newPath, originalPath);
            Console.WriteLine($"El archivo original ha sido reemplazado exitosamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al reemplazar el archivo original: {ex.Message}");
        }
    }
}