using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.Web.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ManageController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Areas/Manage/Views/Shared/_ManageLayout.cshtml");
        }
    }
}
