using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapefileToGeoJsonConverter
{
    public class GeoConverter
    {
        public string ConvertShapefileToGeoJson(string folderPath) {
            string pyScript =
@"import geopandas as gpd
map_df = gpd.read_file(folderPath)
map_df = map_df.to_crs(crs = ""urn:ogc:def:crs:OGC:1.3:CRS84"")
geojson = map_df.to_json()";
            string geoJson=(string) PythonInterop.RunPythonCodeAndReturn(pyScript, folderPath, "folderPath", "geojson");
            return geoJson;
        }
    }
}
