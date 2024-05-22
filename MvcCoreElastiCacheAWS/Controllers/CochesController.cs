using Microsoft.AspNetCore.Mvc;
using MvcCoreElastiCacheAWS.Models;
using MvcCoreElastiCacheAWS.Repositories;
using MvcCoreElastiCacheAWS.Services;

namespace MvcCoreElastiCacheAWS.Controllers
{
    public class CochesController : Controller
    {
        private RepositoryCoches repo;
        private ServiceAWSCache service;

        public CochesController(RepositoryCoches repo, ServiceAWSCache service)
        {
            this.repo = repo;
            this.service = service;
        }

        public async Task<IActionResult> SeleccionarFavorito(int idcoche)
        {
            //BUSCAMOS EL COCHE DEL DOCUMENTO XML (repo)
            Coche car = this.repo.FindCoche(idcoche);
            await this.service.AddCocheFavoritoAsync(car);
            return RedirectToAction("Favoritos");
        }

        public async Task<IActionResult> Favoritos()
        {
            List<Coche> cars = await this.service.GetCochesFavoritosAsync();
            return View(cars);
        }

        public async Task<IActionResult> DeleteFavorito(int idcoche)
        {
            await this.service.DeleteCocheFavoritoAsync(idcoche);
            return RedirectToAction("Favoritos");
        }
        
        public IActionResult Index()
        {
            List<Coche> coches = this.repo.GetCoches();
            return View(coches);
        }

        public IActionResult Details(int id)
        {
            Coche coche = this.repo.FindCoche(id);
            return View(coche);
        }
    }
}
