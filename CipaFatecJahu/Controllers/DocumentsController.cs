using CipaFatecJahu.Models;
using CipaFatecJahu.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CipaFatecJahu.Controllers
{
    [Authorize(Roles = "Administrador,Secretário")]
    public class DocumentsController : Controller
    {
        ContextMongodb _context = new ContextMongodb();
        private UserManager<ApplicationUser> _userManager;
        public DocumentsController(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }
        [Route("History/Documents")]
        public async Task<IActionResult> History(string? material, string? userId, string? date)
        {
            var pipeline = new[]
            {
               new BsonDocument("$lookup", new BsonDocument
               {
                   { "from", "Materials" },
                   { "localField", "MaterialId" },
                   { "foreignField", "_id" },
                   { "as", "Material" }
               }),
               new BsonDocument("$unwind", "$Material"),
               new BsonDocument("$lookup", new BsonDocument
               {
                   { "from", "Mandates" },
                   { "localField", "MandateId" },
                   { "foreignField", "_id" },
                   { "as", "Mandate" }
               }),
               new BsonDocument("$unwind", new BsonDocument
               {
                   { "path", "$Mandate" },
                   { "preserveNullAndEmptyArrays", true }
               }),
               new BsonDocument("$addFields", new BsonDocument
               {
                   { "MandateStartYear", new BsonDocument("$year", new BsonDocument("$toDate", new BsonDocument("$dateFromString", new BsonDocument
                       {
                           { "dateString", new BsonDocument("$concat", new BsonArray {
                               new BsonDocument("$toString", "$Mandate.StartYear.Year"),
                               "-01-01"
                           })}
                       })))
                   },
                   { "MandateTerminationYear", new BsonDocument("$year", new BsonDocument("$toDate", new BsonDocument("$dateFromString", new BsonDocument
                       {
                           { "dateString", new BsonDocument("$concat", new BsonArray {
                               new BsonDocument("$toString", "$Mandate.TerminationYear.Year"),
                               "-01-01"
                           })}
                       })))
                   }
               }),
               new BsonDocument("$project", new BsonDocument
               {
                   { "_id", "$_id" },
                   { "Name", "$Name" },
                   { "DocumentCreationDate", "$DocumentCreationDate" },
                   { "Attachment", "$Attachment" },
                   { "Status", "$Status" },
                   { "Mandate", new BsonDocument("$concat", new BsonArray {
                       new BsonDocument("$toString", "$MandateStartYear"),
                       "/",
                       new BsonDocument("$toString", "$MandateTerminationYear")
                   })},
                   { "Material", "$Material.Description" },
                   { "UserId", "$UserId" },
                   { "ScheduledDate", "$ScheduledDate" },
                   { "LawPublication", "$LawPublication" }
               }),
               new BsonDocument("$sort", new BsonDocument("DocumentCreationDate", -1))
           };

            var result = await _context.Documents.Aggregate<DocumentWithUserMandateMaterialViewModel>(pipeline).ToListAsync();

            if (material != null)
            {
                result = result.Where(u => u.Material == material).ToList();
                ViewBag.MaterialSelected = material;
            }

            if (date == "last_month" || date == null)
            {
                var dateNow = DateTime.Now;
                result = result.Where(u => u.DocumentCreationDate.HasValue && (dateNow - u.DocumentCreationDate.Value).Days < 30).ToList();
                ViewBag.DateSelected = "last_month";
            }


            if (date == "last_6_months")
            {
                var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                result = result.Where(u => u.DocumentCreationDate.HasValue && u.DocumentCreationDate.Value >= sixMonthsAgo).ToList();
                ViewBag.DateSelected = date;
            }


            if (date == "last_year")
            {
                var dateNow = DateTime.Now;
                result = result.Where(u => u.DocumentCreationDate.HasValue && (dateNow - u.DocumentCreationDate.Value).Days < 365).ToList();
                ViewBag.DateSelected = date;
            }




            foreach (var item in result)
            {
                var user = _userManager.Users.FirstOrDefault(u => u.Id == item.UserId);
                if (user != null)
                {
                    item.UserName = user.Name;
                }
            }

            if (userId != null && Guid.TryParse(userId, out Guid parsedUserId))
            {
                result = result.Where(u => u.UserId == parsedUserId).ToList();
                ViewBag.UserNameSelected = userId;
            }

            var materials = (await _context.Materials.Find(_ => true).ToListAsync()).OrderBy(u => u.Description).ToList();
            ViewBag.Materials = new SelectList(materials, "Description", "Description");

            var users = _userManager.Users.ToList();
            ViewBag.UserName = new SelectList(users, "Id", "Name");

            return View(result);
        }

        public async Task<IActionResult> Search(Guid MaterialId, Guid MandateId)
        {
            var pipeline = new[]
            {
               new BsonDocument("$lookup", new BsonDocument
               {
                   { "from", "Materials" },
                   { "localField", "MaterialId" },
                   { "foreignField", "_id" },
                   { "as", "Material" }
               }),
               new BsonDocument("$unwind", "$Material"),
               new BsonDocument("$match", new BsonDocument{
                   { "Material._id", new BsonBinaryData(MaterialId, GuidRepresentation.Standard) }
               }),
               new BsonDocument("$lookup", new BsonDocument
               {
                   { "from", "Mandates" },
                   { "localField", "MandateId" },
                   { "foreignField", "_id" },
                   { "as", "Mandate" }
               }),
               new BsonDocument("$unwind", new BsonDocument
               {
                   { "path", "$Mandate" },
                   { "preserveNullAndEmptyArrays", true }
               }),
               new BsonDocument("$match", new BsonDocument{
                  { "Mandate._id", new BsonBinaryData(MandateId, GuidRepresentation.Standard) }
               }),
               new BsonDocument("$addFields", new BsonDocument
               {
                   { "MandateStartYear", new BsonDocument("$year", new BsonDocument("$toDate", new BsonDocument("$dateFromString", new BsonDocument
                       {
                           { "dateString", new BsonDocument("$concat", new BsonArray {
                               new BsonDocument("$toString", "$Mandate.StartYear.Year"),
                               "-01-01"
                           })}
                       })))
                   },
                   { "MandateTerminationYear", new BsonDocument("$year", new BsonDocument("$toDate", new BsonDocument("$dateFromString", new BsonDocument
                       {
                           { "dateString", new BsonDocument("$concat", new BsonArray {
                               new BsonDocument("$toString", "$Mandate.TerminationYear.Year"),
                               "-01-01"
                           })}
                       })))
                   }
               }),
               new BsonDocument("$project", new BsonDocument
               {
                   { "_id", "$_id" },
                   { "Name", "$Name" },
                   { "DocumentCreationDate", "$DocumentCreationDate" },
                   { "Attachment", "$Attachment" },
                   { "Position", "$Position" },
                   { "Status", "$Status" },
                   { "Mandate", new BsonDocument("$concat", new BsonArray {
                       new BsonDocument("$toString", "$MandateStartYear"),
                       "/",
                       new BsonDocument("$toString", "$MandateTerminationYear")
                   })},
                   { "Material", "$Material.Description" },
                   { "UserId", "$UserId" },
                   { "ScheduledDate", "$ScheduledDate" },
                   { "LawPublication", "$LawPublication" }
               }),
               new BsonDocument("$sort", new BsonDocument("DocumentCreationDate", -1))
            };

            var result = await _context.Documents.Aggregate<DocumentWithUserMandateMaterialViewModel>(pipeline).ToListAsync();

            foreach (var item in result)
            {

                if (!string.IsNullOrEmpty(item.Attachment))
                {
                    var filePath = Path.Combine("~/", item.Attachment);
                    if (!System.IO.File.Exists(filePath))
                    {
                        item.Attachment = "img/icon-photo.png";
                    }
                }
                else
                {
                    item.Attachment = "img/icon-photo.png";
                }
            }

            return View(result);
        }


        public IActionResult Material()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Studies()
        {
            return View((await _context.Documents.Find(u => u.MaterialId == new Guid("2a7d3f2e-9b4e-4b8d-8b7e-2c3d3a5f6d9c") && u.Status == "Ativo").ToListAsync()).OrderByDescending(u => u.DocumentCreationDate));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Legislation()
        {
            return View((await _context.Documents.Find(u => u.MaterialId == new Guid("1a6d3f2e-8b4e-4b8d-8b7e-2c3d3a5f6d9c") && u.Status == "Ativo").ToListAsync()).OrderByDescending(u => u.LawPublication));
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pipeline = new[]
        {              new BsonDocument("$match", new BsonDocument("_id", id)),
               new BsonDocument("$lookup", new BsonDocument
               {
                   { "from", "Materials" },
                   { "localField", "MaterialId" },
                   { "foreignField", "_id" },
                   { "as", "Material" }
               }),
               new BsonDocument("$unwind", "$Material"),
               new BsonDocument("$lookup", new BsonDocument
               {
                   { "from", "Mandates" },
                   { "localField", "MandateId" },
                   { "foreignField", "_id" },
                   { "as", "Mandate" }
               }),
               new BsonDocument("$unwind", new BsonDocument
               {
                   { "path", "$Mandate" },
                   { "preserveNullAndEmptyArrays", true }
               }),
               new BsonDocument("$addFields", new BsonDocument
               {
                   { "MandateStartYear", new BsonDocument("$year", new BsonDocument("$toDate", new BsonDocument("$dateFromString", new BsonDocument
                       {
                           { "dateString", new BsonDocument("$concat", new BsonArray {
                               new BsonDocument("$toString", "$Mandate.StartYear.Year"),
                               "-01-01"
                           })}
                       })))
                   },
                   { "MandateTerminationYear", new BsonDocument("$year", new BsonDocument("$toDate", new BsonDocument("$dateFromString", new BsonDocument
                       {
                           { "dateString", new BsonDocument("$concat", new BsonArray {
                               new BsonDocument("$toString", "$Mandate.TerminationYear.Year"),
                               "-01-01"
                           })}
                       })))
                   }
               }),
               new BsonDocument("$project", new BsonDocument
               {
                   { "_id", "$_id" },
                   { "Name", "$Name" },
                   { "DocumentCreationDate", "$DocumentCreationDate" },
                   { "Position", "$Position" },
                   { "Attachment", "$Attachment" },
                   { "Status", "$Status" },
                   { "Mandate", new BsonDocument("$concat", new BsonArray {
                       new BsonDocument("$toString", "$MandateStartYear"),
                       "/",
                       new BsonDocument("$toString", "$MandateTerminationYear")
                   })},
                   { "Material", "$Material.Description" },
                   { "UserId", "$UserId" },
                   { "ScheduledDate", "$ScheduledDate" },
                   { "LawPublication", "$LawPublication" }
               }),
               new BsonDocument("$sort", new BsonDocument("DocumentCreationDate", -1))
            };

            var result = await _context.Documents.Aggregate<DocumentWithUserMandateMaterialViewModel>(pipeline).ToListAsync();

            if (result == null)
            {
                return NotFound();
            }

            foreach (var item in result)
            {
                var user = _userManager.Users.FirstOrDefault(u => u.Id == item.UserId);
                if (user != null)
                {
                    item.UserName = user.Name;
                }
            }

            return View(result.FirstOrDefault());
        }

        public IActionResult Create()
        {
            return RedirectToAction("Material");
        }

        [Route("Documents/ATA/Create")]
        public IActionResult AtaCreate()
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/ATA/Create")]
        public async Task<IActionResult> AtaCreate([Bind("Id,Name,DocumentCreationDate,ScheduledDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (document.ScheduledDate == null || document.ScheduledDate == DateOnly.MinValue)
                {
                    ModelState.AddModelError("ScheduledDate", "O campo Data da Reunião é obrigatório");
                    return View(document);
                }
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo Mandato é obrigatório!");
                    return View(document);
                }
                if (document.ScheduledDate > DateOnly.FromDateTime(DateTime.Now.AddHours(-3)))
                {
                    ModelState.AddModelError("ScheduledDate", "A data da reunião não pode ser futura.");
                    return View(document);
                }
                var mandate = _context.Mandates.Find(m => m.Id == document.MandateId.Value).FirstOrDefault();
                if (mandate != null)
                {
                    if (document.ScheduledDate < mandate.StartYear || document.ScheduledDate > mandate.TerminationYear)
                    {
                        ModelState.AddModelError("ScheduledDate", $"A data da reunião deve estar dentro do período do mandato selecionado. {mandate.StartYear} - {mandate.TerminationYear}");
                        return View(document);
                    }
                }
                else
                {
                    ModelState.AddModelError("MandateId", "Mandato não encontrado ou inválido.");
                    return View(document);
                }
                if (attachment == null || attachment.Length == 0)
                {
                    ModelState.AddModelError("Attachment", "O campo Anexar é obrigatório!");
                    return View(document);
                }
                var extension = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
                    return View(document);
                }
                document.Id = Guid.NewGuid();
                document.MaterialId = new Guid("9b927360-b531-4bb9-9e09-1a3093f8507a");
                document.DocumentCreationDate = DateTime.Now.AddHours(-3);
                document.Status = "Ativo";
                document.UserId = new Guid(_userManager.GetUserId(User));

                string randomFileName;
                string filePath;
                do
                {
                    randomFileName = $"ATA-{Guid.NewGuid()}.pdf";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", randomFileName);
                } while (System.IO.File.Exists(filePath));

                document.Attachment = Path.Combine("docs/", randomFileName);
                await _context.Documents.InsertOneAsync(document); //Insert

                var docsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs");
                if (!Directory.Exists(docsDirectory))
                {
                    Directory.CreateDirectory(docsDirectory);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(History));
            }
            return View(document);
        }

        [Route("Documents/Course/Create")]
        public IActionResult CourseCreate()
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/Course/Create")]
        public async Task<IActionResult> CourseCreate([Bind("Id,Name,DocumentCreationDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo Mandato é obrigatório!");
                    return View(document);
                }
                if (attachment == null || attachment.Length == 0)
                {
                    ModelState.AddModelError("Attachment", "O campo Anexar é obrigatório!");
                    return View(document);
                }
                var extension = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
                    return View(document);
                }
                document.Id = Guid.NewGuid();
                document.MaterialId = new Guid("d2f6b9f0-3b1a-4e4e-9b8e-1c3d2a4f7c8b");
                document.DocumentCreationDate = DateTime.Now.AddHours(-3);
                document.Status = "Ativo";
                document.UserId = new Guid(_userManager.GetUserId(User));

                string randomFileName;
                string filePath;
                do
                {
                    randomFileName = $"COURSE-{Guid.NewGuid()}.pdf";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", randomFileName);
                } while (System.IO.File.Exists(filePath));

                document.Attachment = Path.Combine("docs/", randomFileName);

                await _context.Documents.InsertOneAsync(document); //Insert

                var docsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs");
                if (!Directory.Exists(docsDirectory))
                {
                    Directory.CreateDirectory(docsDirectory);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(History));
            }
            return View(document);
        }

        [Route("Documents/Election/Create")]
        public IActionResult ElectionCreate()
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/Election/Create")]
        public async Task<IActionResult> ElectionCreate([Bind("Id,Name,DocumentCreationDate,ScheduledDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (document.ScheduledDate == null || document.ScheduledDate == DateOnly.MinValue)
                {
                    ModelState.AddModelError("ScheduledDate", "O campo Data da Reunião é obrigatório");
                    return View(document);
                }
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo Mandato é obrigatório!");
                    return View(document);
                }
                if (document.ScheduledDate > DateOnly.FromDateTime(DateTime.Now.AddHours(-3)))
                {
                    ModelState.AddModelError("ScheduledDate", "A data da reunião não pode ser futura.");
                    return View(document);
                }
                var mandate = _context.Mandates.Find(m => m.Id == document.MandateId.Value).FirstOrDefault();
                if (mandate != null)
                {
                    if (document.ScheduledDate < mandate.StartYear || document.ScheduledDate > mandate.TerminationYear)
                    {
                        ModelState.AddModelError("ScheduledDate", $"A data da reunião deve estar dentro do período do mandato selecionado. {mandate.StartYear} - {mandate.TerminationYear}");
                        return View(document);
                    }
                }
                else
                {
                    ModelState.AddModelError("MandateId", "Mandato não encontrado ou inválido.");
                    return View(document);
                }
                if (attachment == null || attachment.Length == 0)
                {
                    ModelState.AddModelError("Attachment", "O campo Anexar é obrigatório!");
                    return View(document);
                }
                var extension = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
                    return View(document);
                }
                document.Id = Guid.NewGuid();
                document.MaterialId = new Guid("3b8d3f2e-1a1c-4e4e-9b8e-1c3d2a4f7c8b");
                document.DocumentCreationDate = DateTime.Now.AddHours(-3);
                document.Status = "Ativo";
                document.UserId = new Guid(_userManager.GetUserId(User));

                string randomFileName;
                string filePath;
                do
                {
                    randomFileName = $"ELECTION-{Guid.NewGuid()}.pdf";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", randomFileName);
                } while (System.IO.File.Exists(filePath));

                document.Attachment = Path.Combine("docs/", randomFileName);
                await _context.Documents.InsertOneAsync(document); //Insert

                var docsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs");
                if (!Directory.Exists(docsDirectory))
                {
                    Directory.CreateDirectory(docsDirectory);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(History));
            }
            return View(document);
        }

        [Route("Documents/Studies/Create")]
        public IActionResult StudiesCreate()
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/Studies/Create")]
        public async Task<IActionResult> StudiesCreate([Bind("Id,Name,DocumentCreationDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (attachment == null || attachment.Length == 0)
                {
                    ModelState.AddModelError("Attachment", "O campo Anexar é obrigatório!");
                    return View(document);
                }
                var extension = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
                    return View(document);
                }

                document.Id = Guid.NewGuid();
                document.MaterialId = new Guid("2a7d3f2e-9b4e-4b8d-8b7e-2c3d3a5f6d9c");
                document.DocumentCreationDate = DateTime.Now.AddHours(-3);
                document.Status = "Ativo";
                document.UserId = new Guid(_userManager.GetUserId(User));

                string randomFileName;
                string filePath;
                do
                {
                    randomFileName = $"STUDIES-{Guid.NewGuid()}.pdf";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", randomFileName);
                } while (System.IO.File.Exists(filePath));

                document.Attachment = Path.Combine("docs/", randomFileName);
                await _context.Documents.InsertOneAsync(document); //Insert

                var docsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs");
                if (!Directory.Exists(docsDirectory))
                {
                    Directory.CreateDirectory(docsDirectory);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(History));
            }
            return View(document);
        }

        [Route("Documents/Legislation/Create")]
        public IActionResult LegislationCreate()
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/Legislation/Create")]
        public async Task<IActionResult> LegislationCreate([Bind("Id,Name,DocumentCreationDate,LawPublication,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (document.LawPublication == null || document.LawPublication == DateOnly.MinValue)
                {
                    ModelState.AddModelError("LawPublication", "O campo Data da publicação é obrigatório");
                    return View(document);
                }
                if (document.LawPublication > DateOnly.FromDateTime(DateTime.Now.AddHours(-3)))
                {
                    ModelState.AddModelError("LawPublication", "A data da publicação não pode ser futura.");
                    return View(document);
                }
                if (attachment == null || attachment.Length == 0)
                {
                    ModelState.AddModelError("Attachment", "O campo Anexar é obrigatório!");
                    return View(document);
                }
                var extension = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
                    return View(document);
                }
                document.Id = Guid.NewGuid();
                document.MaterialId = new Guid("1a6d3f2e-8b4e-4b8d-8b7e-2c3d3a5f6d9c");
                document.DocumentCreationDate = DateTime.Now.AddHours(-3);
                document.Status = "Ativo";
                document.UserId = new Guid(_userManager.GetUserId(User));

                string randomFileName;
                string filePath;
                do
                {
                    randomFileName = $"LEGISLATION-{Guid.NewGuid()}.pdf";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", randomFileName);
                } while (System.IO.File.Exists(filePath));

                document.Attachment = Path.Combine("docs/", randomFileName);
                await _context.Documents.InsertOneAsync(document); //Insert

                var docsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs");
                if (!Directory.Exists(docsDirectory))
                {
                    Directory.CreateDirectory(docsDirectory);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(History));
            }
            return View(document);
        }

        [Route("Documents/Map/Create")]
        public IActionResult MapCreate()
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/Map/Create")]
        public async Task<IActionResult> MapCreate([Bind("Id,Name,DocumentCreationDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo Mandato é obrigatório!");
                    return View(document);
                }
                if (attachment == null || attachment.Length == 0)
                {
                    ModelState.AddModelError("Attachment", "O campo Anexar é obrigatório!");
                    return View(document);
                }
                var extension = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
                    return View(document);
                }
                document.Id = Guid.NewGuid();
                document.MaterialId = new Guid("0a5d3f2e-7b4e-4b8d-8b7e-2c3d3a5f6d9c");
                document.DocumentCreationDate = DateTime.Now.AddHours(-3);
                document.Status = "Ativo";
                document.UserId = new Guid(_userManager.GetUserId(User));

                string randomFileName;
                string filePath;
                do
                {
                    randomFileName = $"MAP-{Guid.NewGuid()}.pdf";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", randomFileName);
                } while (System.IO.File.Exists(filePath));

                document.Attachment = Path.Combine("docs/", randomFileName);

                await _context.Documents.InsertOneAsync(document); //Insert

                var docsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs");
                if (!Directory.Exists(docsDirectory))
                {
                    Directory.CreateDirectory(docsDirectory);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(History));
            }
            return View(document);
        }

        [Route("Documents/Other/Create")]
        public IActionResult OtherCreate()
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/Other/Create")]
        public async Task<IActionResult> OtherCreate([Bind("Id,Name,DocumentCreationDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo Mandato é obrigatório!");
                    return View(document);
                }
                if (attachment == null || attachment.Length == 0)
                {
                    ModelState.AddModelError("Attachment", "O campo Anexar é obrigatório!");
                    return View(document);
                }
                var extension = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
                    return View(document);
                }
                document.Id = Guid.NewGuid();
                document.MaterialId = new Guid("7e4c8f2d-9b4e-4b8d-8b7e-2c3d3a5f6d9c");
                document.DocumentCreationDate = DateTime.Now.AddHours(-3);
                document.Status = "Ativo";
                document.UserId = new Guid(_userManager.GetUserId(User));

                string randomFileName;
                string filePath;
                do
                {
                    randomFileName = $"OTHER-{Guid.NewGuid()}.pdf";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", randomFileName);
                } while (System.IO.File.Exists(filePath));

                document.Attachment = Path.Combine("docs/", randomFileName);

                await _context.Documents.InsertOneAsync(document); //Insert

                var docsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs");
                if (!Directory.Exists(docsDirectory))
                {
                    Directory.CreateDirectory(docsDirectory);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(History));
            }
            return View(document);
        }

        [Route("Documents/SIPAT/Create")]
        public IActionResult SipatCreate()
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/SIPAT/Create")]
        public async Task<IActionResult> SipatCreate([Bind("Id,Name,DocumentCreationDate,ScheduledDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (document.ScheduledDate == null || document.ScheduledDate == DateOnly.MinValue)
                {
                    ModelState.AddModelError("ScheduledDate", "O campo data do evento é obrigatório");
                    return View(document);
                }
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo mandato é obrigatório!");
                    return View(document);
                }
                if (document.ScheduledDate > DateOnly.FromDateTime(DateTime.Now.AddHours(-3)))
                {
                    ModelState.AddModelError("ScheduledDate", "A data do evento não pode ser futura.");
                    return View(document);
                }
                var mandate = _context.Mandates.Find(m => m.Id == document.MandateId.Value).FirstOrDefault();
                if (mandate != null)
                {
                    if (document.ScheduledDate < mandate.StartYear || document.ScheduledDate > mandate.TerminationYear)
                    {
                        ModelState.AddModelError("ScheduledDate", $"A data do evento deve estar dentro do período do mandato selecionado. {mandate.StartYear} - {mandate.TerminationYear}");
                        return View(document);
                    }
                }
                else
                {
                    ModelState.AddModelError("MandateId", "Mandato não encontrado ou inválido.");
                    return View(document);
                }
                if (attachment == null || attachment.Length == 0)
                {
                    ModelState.AddModelError("Attachment", "O campo Anexar é obrigatório!");
                    return View(document);
                }
                var extension = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
                    return View(document);
                }
                document.Id = Guid.NewGuid();
                document.MaterialId = new Guid("9a4d3f2e-6b4e-4b8d-8b7e-2c3d3a5f6d9c");
                document.DocumentCreationDate = DateTime.Now.AddHours(-3);
                document.Status = "Ativo";
                document.UserId = new Guid(_userManager.GetUserId(User));

                string randomFileName;
                string filePath;
                do
                {
                    randomFileName = $"SIPAT-{Guid.NewGuid()}.pdf";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", randomFileName);
                } while (System.IO.File.Exists(filePath));

                document.Attachment = Path.Combine("docs/", randomFileName);
                await _context.Documents.InsertOneAsync(document); //Insert

                var docsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs");
                if (!Directory.Exists(docsDirectory))
                {
                    Directory.CreateDirectory(docsDirectory);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(History));
            }
            return View(document);
        }

        [Route("Members/Create")]
        public IActionResult MembersCreate()
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");

            return View();
        }
        [HttpPost]
        [Route("Members/Create")]
        public async Task<IActionResult> MembersCreate([Bind("Id,Name,Position,DocumentCreationDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (document.Position == null)
                {
                    ModelState.AddModelError("Position", "O campo cargo é obrigatório");
                    return View(document);
                }
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo mandato é obrigatório!");
                    return View(document);
                }
                var mandate = _context.Mandates.Find(m => m.Id == document.MandateId.Value).FirstOrDefault();
                if (mandate == null)
                {
                    ModelState.AddModelError("MandateId", "Mandato não encontrado ou inválido.");
                    return View(document);
                }
                if (attachment == null || attachment.Length == 0)
                {
                    ModelState.AddModelError("Attachment", "O campo Anexar é obrigatório!");
                    return View(document);
                }
                var extension = Path.GetExtension(attachment.FileName);
                var allowedImageExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };
                if (string.IsNullOrEmpty(extension) || !allowedImageExtensions.Contains(extension.ToLower()))
                {
                    ModelState.AddModelError("Attachment", "Somente arquivos de imagem (.png, .jpg, .jpeg, .gif, .bmp) são permitidos.");
                    return View(document);
                }
                document.Id = Guid.NewGuid();
                document.MaterialId = new Guid("8c5d3f2e-5b4e-4b8d-8b7e-2c3d3a5f6d9c");
                document.DocumentCreationDate = DateTime.Now.AddHours(-3);
                document.Status = "Ativo";
                document.UserId = new Guid(_userManager.GetUserId(User));

                string randomFileName;
                string filePath;
                do
                {
                    randomFileName = $"MEMBERS-{Guid.NewGuid()}{Path.GetExtension(attachment.FileName)}";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", randomFileName);
                } while (System.IO.File.Exists(filePath));

                document.Attachment = Path.Combine("docs/", randomFileName);
                await _context.Documents.InsertOneAsync(document); //Insert

                var docsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs");
                if (!Directory.Exists(docsDirectory))
                {
                    Directory.CreateDirectory(docsDirectory);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(History));
            }
            return View(document);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ChangeStatus(Guid id, string status)
        {
            if (id == Guid.Empty || status == "" || status == null)
            {
                return NotFound();
            }
            var document = await _context.Documents.Find(d => d.Id == id).FirstOrDefaultAsync();
            if (document == null)
            {
                return NotFound();
            }
            document.Status = document.Status == "Ativo" ? "Inativo" : "Ativo";
            try
            {
                await _context.Documents.ReplaceOneAsync(m => m.Id == document.Id, document);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(document.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        private bool DocumentExists(Guid id)
        {
            return _context.Documents.Find(e => e.Id == id).Any();
        }

        private List<dynamic> SearchMandates()
        {
            return _context.Mandates.AsQueryable().OrderByDescending(m => m.StartYear).ToList().Select(m => new { m.Id, mandate = $"{m.StartYear.Year}/{m.TerminationYear.Year}" }).ToList<dynamic>();
        }

    }
}
