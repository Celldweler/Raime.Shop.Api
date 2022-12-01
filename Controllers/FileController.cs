using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.DAL;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers
{
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private ApplicationDbContext _ctx;
        private string _dir;
        private IWebHostEnvironment _env;

        public FileController(IWebHostEnvironment env, ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _dir = Path.Combine(env.ContentRootPath, @"wwwroot\images\product_images");
            _env = env;
        }
        [HttpGet("{imgPath}")]
        public IActionResult GetProductImages(string imgPath)
        {
            var mime = "image/png";

            var streamResult = new FileStream(Path.Combine(_dir, imgPath), FileMode.Open, FileAccess.Read);

            return new FileStreamResult(streamResult, mime);
        }

        [HttpPost]
        public IActionResult DownloadProductImage(ProductImage productImage)
        {
            string mime = "." + productImage.FormFile.FileName.Split('.')[1];

            string imageName = productImage.FileName + mime;

            using (var fileStream = new FileStream(Path.Combine(_dir, imageName),
               FileMode.Create, FileAccess.Write))
            {
                productImage.FormFile.CopyTo(fileStream);
            }
            return Ok(imageName);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductImage(ProductImageUpdate productImageUpdate)
        {
            var p = _ctx.Products.Where(p => p.ProductId == productImageUpdate.ProductId).FirstOrDefault();
            var oldFileName = p.ImagePath;

            System.IO.File.Delete(Path.Combine(_dir, oldFileName));

            string mime = "." + productImageUpdate.FormFile.FileName.Split('.')[1];

            var newImageName = p.ImagePath = productImageUpdate.NewFileName + mime;

            using (var fileStream = new FileStream(Path.Combine(_dir, newImageName),
              FileMode.Create, FileAccess.Write))
            {
                productImageUpdate.FormFile.CopyTo(fileStream);
            }

            await _ctx.SaveChangesAsync();

            return Ok(newImageName);
        }
        public class ProductImageUpdate
        {
            public int ProductId { get; set; }
            public IFormFile FormFile { get; set; }
            public string NewFileName { get; set; }
        }
        public class ProductImage
        {
            public IFormFile FormFile { get; set; }
            public string FileName { get; set; }
        }
    }
}
