using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Infrastructure.Settings
{
    public class AzureStorageSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerName { get; set; } = "movie-posters";
    }
}
