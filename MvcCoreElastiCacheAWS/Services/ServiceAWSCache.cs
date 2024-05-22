using Microsoft.Extensions.Caching.Distributed;
using MvcCoreElastiCacheAWS.Models;
using Newtonsoft.Json;

namespace MvcCoreElastiCacheAWS.Services
{
    public class ServiceAWSCache
    {
        private IDistributedCache cache;

        public ServiceAWSCache(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<List<Coche>> GetCochesFavoritosAsync()
        {
            //ALMACENAREMOS UNA COLECCION DE COCHES EN FORMATO JSON
            //LAS KEYS DEBEN SER UNICAS PARA CADA USER
            string jsonCoches = await this.cache.GetStringAsync("cochesfavoritos");
            if(jsonCoches == null)
            {
                return null;
            }
            else
            {
                List<Coche> coches = JsonConvert.DeserializeObject<List<Coche>>(jsonCoches);
                return coches;
            }
        }

        public async Task AddCocheFavoritoAsync(Coche car)
        {
            List<Coche> cars = await this.GetCochesFavoritosAsync();
            //SI NO EXISTE COCHES FAVORITOS TODAVIA, CREAMOS LA COLECCION
            if(cars == null)
            {
                cars = new List<Coche>();
            }
            //AÑADIMOS EL NUEVO COCHE A LA COLECCION
            cars.Add(car);
            //SERIALIZAMOS A JSON LA COLECCION
            string jsonCoches = JsonConvert.SerializeObject(cars);
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
            //ALMACENAMOS LA COLECCION DENTRO DE CACHE REDIS
            //INDICAREMOS QUE LOS DATOS DURARAN 30 MINS
            await this.cache.SetStringAsync("cochesfavoritos", jsonCoches, options);
        }

        public async Task DeleteCocheFavoritoAsync(int idcoche)
        {
            List<Coche> cars = await this.GetCochesFavoritosAsync();
            if(cars != null)
            {
                Coche carDelete = cars.FirstOrDefault(x => x.IdCoche == idcoche);
                cars.Remove(carDelete);
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                };
                //COMPROBAMOS SI LA COLECCION TIENE COCHES FAVORITOS TODAVIA O NO TIENE
                //SI NO TIENE COCHES, ELIMINAMOS LA JEY DE CACHE REDIS
                if (cars.Count == 0)
                {
                    await this.cache.RemoveAsync("cochesfavoritos");
                }
                else
                {
                    //ALMACENAMOS DE NUEVO LOS COCHES SIN EL CAR A ELIMINAR
                    string jsonCoches = JsonConvert.SerializeObject(cars);
                    //ACTUALIZAMOS EL CACHE REDIS
                    await this.cache.SetStringAsync("cochesfavoritos", jsonCoches, options);
                }
            }
        }
    }
}
