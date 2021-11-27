using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace ShapefileToGeoJsonConverter.Blazor.Pages
{
    public partial class Index
    {
        [Inject] IJSRuntime _js { get; set; }
        const string wwwrootDirectory= "./wwwroot";
        GeoConverter GeoConverter = new GeoConverter();

        public async void ConvertToGeoJson(IBrowserFile file)
        {
            //save the .zip to a temp folder and extract it
            byte[] fileBytes=await ConvertFileToByteArray(file);
            string guid = Guid.NewGuid().ToString();
            string tmpDirectoryPath = Path.Combine(wwwrootDirectory, "tmp");
            string guidTempDirectoryPath = Path.Combine(tmpDirectoryPath, guid);
            Directory.CreateDirectory(guidTempDirectoryPath);
            string zipFilePath = Path.Combine(guidTempDirectoryPath, guid + ".zip");
            File.WriteAllBytes(zipFilePath, fileBytes);
            ZipFile.ExtractToDirectory(zipFilePath, guidTempDirectoryPath);
            //pass the folder to our python script
            string geoJson = GeoConverter.ConvertShapefileToGeoJson(guidTempDirectoryPath);
            //remove the temp files
            foreach (var item in new DirectoryInfo(tmpDirectoryPath).EnumerateDirectories())
            {
                item.Delete(true);
            }
            foreach (var item in new DirectoryInfo(tmpDirectoryPath).EnumerateFiles())
            {
                item.Delete();
            }
            Directory.Delete(tmpDirectoryPath);
            //do whatever you want with the GeoJSON. in our case we'll just try to save it on the client's PC.
            await SaveAs("output.json", Encoding.UTF8.GetBytes(geoJson));
        }


        public async Task<byte[]> ConvertFileToByteArray(IBrowserFile file)
        {
           
            using (MemoryStream ms = new MemoryStream())
            {
                Stream data = file.OpenReadStream(100*1024*1024);
                await data.CopyToAsync(ms);
                byte[] fileBytes = ms.ToArray();
                return fileBytes;
            }
           
        }

        public async Task SaveAs(string filename, byte[] data)
        {
            await _js.InvokeAsync<object>(
            "saveAsFile",
            filename,
            Convert.ToBase64String(data));

        }

    
    }
}
