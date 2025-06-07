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

        public async Task<IActionResult> History()
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
                   { "MeetingDate", "$MeetingDate" },
                   { "LawPublication", "$LawPublication" }
               }),
               new BsonDocument("$sort", new BsonDocument("DocumentCreationDate", -1))
            };

            var result = await _context.Documents.Aggregate<DocumentWithUserMandateMaterialViewModel>(pipeline).ToListAsync();

            foreach (var item in result)
            {
                var user = _userManager.Users.FirstOrDefault(u => u.Id == item.UserId);
                if (user != null)
                {
                    item.UserName = user.Name;
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
                   { "Attachment", "$Attachment" },
                   { "Status", "$Status" },
                   { "Mandate", new BsonDocument("$concat", new BsonArray {
                       new BsonDocument("$toString", "$MandateStartYear"),
                       "/",
                       new BsonDocument("$toString", "$MandateTerminationYear")
                   })},
                   { "Material", "$Material.Description" },
                   { "UserId", "$UserId" },
                   { "MeetingDate", "$MeetingDate" },
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
        public async Task<IActionResult> AtaCreate([Bind("Id,Name,DocumentCreationDate,MeetingDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (document.MeetingDate == null || document.MeetingDate == DateOnly.MinValue)
                {
                    ModelState.AddModelError("MeetingDate", "O campo Data da Reunião é obrigatório");
                    return View(document);
                }
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo Mandato é obrigatório!");
                    return View(document);
                }
                if (document.MeetingDate > DateOnly.FromDateTime(DateTime.Now.AddHours(-3)))
                {
                    ModelState.AddModelError("MeetingDate", "A data da reunião não pode ser futura.");
                    return View(document);
                }
                var mandate = _context.Mandates.Find(m => m.Id == document.MandateId.Value).FirstOrDefault();
                if (mandate != null)
                {
                    if (document.MeetingDate < mandate.StartYear || document.MeetingDate > mandate.TerminationYear)
                    {
                        ModelState.AddModelError("MeetingDate", $"A data da reunião deve estar dentro do período do mandato selecionado. {mandate.StartYear} - {mandate.TerminationYear}");
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
                    ModelState.AddModelError("Attachment", "O campo Anexo é obrigatório!");
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

        //[Route("Documents/ATA/Edit")]
        //public async Task<IActionResult> AtaEdit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var document = await _context.Documents.Find(m => m.Id == id).FirstOrDefaultAsync();
        //    if (document == null)
        //    {
        //        return NotFound();
        //    }
        //    var mandates = SearchMandates();
        //    ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
        //    return View(document);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Route("Documents/ATA/Edit")]
        //public async Task<IActionResult> AtaEdit(Guid id, [Bind("Id,Name,DocumentCreationDate,MeetingDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment, bool changeAttachment)
        //{
        //    if (id != document.Id)
        //    {
        //        return NotFound();
        //    }
        //    var mandates = SearchMandates();
        //    ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
        //    if (ModelState.IsValid)
        //    {

        //        if (document.MeetingDate == null || document.MeetingDate == DateOnly.MinValue)
        //        {
        //            ModelState.AddModelError("MeetingDate", "O campo Data da Reunião é obrigatório");
        //            return View(document);
        //        }
        //        if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
        //        {
        //            ModelState.AddModelError("MandateId", "O campo Mandato é obrigatório!");
        //            return View(document);
        //        }
        //        if (changeAttachment == true)
        //        {
        //            if (attachment == null || attachment.Length == 0)
        //            {
        //                ModelState.AddModelError("Attachment", "O campo Anexo é obrigatório!");
        //                return View(document);
        //            }
        //            var extension = Path.GetExtension(attachment.FileName);
        //            if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        //            {
        //                ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
        //                return View(document);
        //            }

        //        }
        //        try
        //        {
        //            await _context.Documents.ReplaceOneAsync(m => m.Id == document.Id, document);
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!DocumentExists(document.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        if (changeAttachment == true)
        //        {
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(),
        //                  "wwwroot", "docs", attachment.FileName);
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await attachment.CopyToAsync(stream);
        //            }
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(document);
        //}

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
                    ModelState.AddModelError("Attachment", "O campo Anexo é obrigatório!");
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
        public async Task<IActionResult> ElectionCreate([Bind("Id,Name,DocumentCreationDate,MeetingDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (document.MeetingDate == null || document.MeetingDate == DateOnly.MinValue)
                {
                    ModelState.AddModelError("MeetingDate", "O campo Data da Reunião é obrigatório");
                    return View(document);
                }
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo Mandato é obrigatório!");
                    return View(document);
                }
                if (document.MeetingDate > DateOnly.FromDateTime(DateTime.Now.AddHours(-3)))
                {
                    ModelState.AddModelError("MeetingDate", "A data da reunião não pode ser futura.");
                    return View(document);
                }
                var mandate = _context.Mandates.Find(m => m.Id == document.MandateId.Value).FirstOrDefault();
                if (mandate != null)
                {
                    if (document.MeetingDate < mandate.StartYear || document.MeetingDate > mandate.TerminationYear)
                    {
                        ModelState.AddModelError("MeetingDate", $"A data da reunião deve estar dentro do período do mandato selecionado. {mandate.StartYear} - {mandate.TerminationYear}");
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
                    ModelState.AddModelError("Attachment", "O campo Anexo é obrigatório!");
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
                    ModelState.AddModelError("Attachment", "O campo Anexo é obrigatório!");
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
                    ModelState.AddModelError("Attachment", "O campo Anexo é obrigatório!");
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
                    ModelState.AddModelError("Attachment", "O campo Anexo é obrigatório!");
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
            return RedirectToAction("History");
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
