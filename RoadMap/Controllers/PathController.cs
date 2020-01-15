using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoadMap.Controllers
{
    public class PathController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Map(string imageName)
        {
            int width = 0;
            int height = 0;
        
            var directory = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
            var myFile = directory.GetFiles(imageName).FirstOrDefault();

            if (myFile != null && myFile.Extension != ".svg")
            {
                using (var image = Image.FromStream(myFile.OpenRead()))
                {
                    width = image.Width/10 ;
                    height = image.Height/10;
                }
            }

            ViewBag.width = width;
            ViewBag.height = height;
            ViewBag.path = "/images/" + myFile.Name;

            return View();
        }

        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file.Length > 0)
            {

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", file.FileName);

                //if (System.IO.File.Exists(path))
                //{
                //    System.IO.File.Delete(path);
                //}

                int count = 1;

                string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
                string extension = Path.GetExtension(fullPath);
                string path = Path.GetDirectoryName(fullPath);
                string newFullPath = fullPath;

                while (System.IO.File.Exists(newFullPath))
                {
                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                    newFullPath = Path.Combine(path, tempFileName + extension);
                }

                //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", file.FileName);

                using (var stream = new FileStream(newFullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return RedirectToAction("Map", "Path", new { imageName = file.FileName });

        }
    }
}