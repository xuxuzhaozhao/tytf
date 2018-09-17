using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using TYTFBackTermimalBack.DataHandle.Config;
using TYTFBackTermimalBack.Models;

namespace TYTFBackTermimalBack.Api
{
    [Produces("application/json")]
    [Consumes("application/json", "multipart/form-data")]//此处为新增
    [Route("api/Upload")]
    public class UploadController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public UploadController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("menu/{id}")]
        public object Post(int id, IFormCollection files)
        {
            var file = files.Files[0];
            var urlPath = $"/menuupload/{file.FileName}";
            var fileName = _hostingEnvironment.WebRootPath + urlPath;

            //if (System.IO.File.Exists(fileName))
            //{
            //    System.IO.File.Delete(fileName);
            //}

            if (!Directory.Exists(_hostingEnvironment.WebRootPath + "/menuupload"))
            {
                Directory.CreateDirectory(_hostingEnvironment.WebRootPath + "/menuupload");
            }

            using (FileStream fs = System.IO.File.Create(fileName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            using (Image<Rgba32> image = Image.Load(fileName))
            {
                var originName = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                urlPath = $"/menuupload/{DateTime.Now:yyyyMMddHHmmss}_{originName}_icon228{ext}";

                var newFileName = $"{_hostingEnvironment.WebRootPath}{urlPath}";

                //if (System.IO.File.Exists(newFileName))
                //{
                //    System.IO.File.Delete(newFileName);
                //}

                image.Mutate(x => x.Resize(228, 228));
                image.Save($"{_hostingEnvironment.WebRootPath}{urlPath}");
            }

            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            //删除老的
            string oriFileName = null;
            using (var con = new MySqlConnection(XConfig.MySqlConnectionString))
            {
                con.Open();
                using(var tran = con.BeginTransaction())
                {
                    try
                    {
                        string query = $@"SELECT Icon FROM Menu WHERE Id = @Id";
                        string oriUrlPath = con.ExecuteScalar<string>(query, new { Id = id }, tran);
                        oriFileName = $"{_hostingEnvironment.WebRootPath}{oriUrlPath}";

                        string sql = $@"UPDATE Menu SET 
                                        RootUrl='https://tytf.xuxuzhaozhao.top',
                                        Icon=@Icon
                                    WHERE Id = @Id";
                        con.Execute(sql, new { Id = id, Icon = urlPath }, tran);

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }

            if (!string.IsNullOrEmpty(oriFileName) && System.IO.File.Exists(oriFileName))
            {
                System.IO.File.Delete(oriFileName);
            }
            return new XResult();
        }
    }
}