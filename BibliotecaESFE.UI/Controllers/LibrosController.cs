﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BibliotecaESFE.EN;
using BibliotecaESFE.BL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using BibliotecaESFE.DAL;


namespace BibliotecaESFE.UI.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LibrosController : Controller
    {
        AutoresBL autoresBL = new AutoresBL();
        EditorialesBL editorialesBL = new EditorialesBL();
        CategoriasBL categoriasBL = new CategoriasBL();
        LibrosBL librosBL = new LibrosBL();
        CalificacionesReseniasBL calificacionesReseniasBL = new CalificacionesReseniasBL();
        UsuarioBL usuarioBL = new UsuarioBL();
        // GET: LibrosController

        // GET: LibrosController
        public async Task<IActionResult> Index(string searchTitle = "", int top = 10)
        {
            // Si searchTitle no es nulo o vacío, filtra los libros por título.
            var librosFiltrados = await librosBL.SearchIncludeLibrosAsync(new Libros { Titulo = searchTitle, Top_Aux = top });

            // Cargar las listas relacionadas como antes.
            var autores = await autoresBL.GetAllAsync();
            var editoriales = await editorialesBL.GetAllAsync();
            var categorias = await categoriasBL.GetAllAsync();
            var calificacionesResenias = await calificacionesReseniasBL.GetAllAsync();
            var usuarios = await usuarioBL.GetAllAsync();

            // Pasar todo a ViewBag como antes.
            ViewBag.Top = top;
            ViewBag.Autores = autores;
            ViewBag.Editoriales = editoriales;
            ViewBag.Categorias = categorias;
            ViewBag.CalificacionesResenias = calificacionesResenias;
            ViewBag.Usuarios = usuarios;

            return View(librosFiltrados);
        }

        public async Task<IActionResult> IndexUs()
        {
            var libros = new Libros(); // Si necesitas inicializar con valores específicos
            var libro = await librosBL.SearchIncludeLibrosAsync(libros);

            var calificacionesResenias = await calificacionesReseniasBL.GetAllAsync(); // Importante para mostrar calificaciones

            ViewBag.CalificacionesResenias = calificacionesResenias; // Pasar las calificaciones a la vista

            return View(libro);
        }

       
        // GET: LibrosController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var libros = await librosBL.GetByIdAsync(new Libros { Id = id });
            libros.Autores = await autoresBL.GetByIdAsync(new Autores { Id = libros.AutorId });
            libros.Editoriales = await editorialesBL.GetByIdAsync(new Editoriales { Id = libros.EditorialId });
            libros.Categorias = await categoriasBL.GetByIdAsync(new Categorias { Id = libros.CategoriaId });
            return View(libros);
        }
        public async Task<IActionResult> Calificar(int id)
        {
            var libros = await librosBL.GetByIdAsync(new Libros { Id = id });
            var usuario = await usuarioBL.SearchAsync(new Usuarios { Login = User.Identity.Name, Top_Aux = 1 });
            var actualUser = usuario.FirstOrDefault();
            ViewBag.Usuario = actualUser;
            ViewBag.Libro = libros;
            ViewBag.Error = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calificar(CalificacionesResenias calificacionesResenias)
        {
            try
            {
                calificacionesResenias.Id = 0;
                calificacionesResenias.FechaCalificacion = DateTime.Now;
                int result = await calificacionesReseniasBL.CreateAsync(calificacionesResenias);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                ViewBag.CalificacionesResenias = await calificacionesReseniasBL.GetAllAsync();
                return View(calificacionesResenias);
            }
        }
       

        // GET: LibrosController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Autores = await autoresBL.GetAllAsync();
            ViewBag.Editoriales = await editorialesBL.GetAllAsync();
            ViewBag.Categorias = await categoriasBL.GetAllAsync();
            ViewBag.Error = "";
            return View();
        }



        // POST: LibrosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Libros libros, IFormFile imagen)
        {
            try
            {

                int result = await librosBL.CreateAsync(libros);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                ViewBag.Autores = await autoresBL.GetAllAsync();
                ViewBag.Editoriales = await editorialesBL.GetAllAsync();
                ViewBag.Categorias = await categoriasBL.GetAllAsync();
                return View(libros);
            }
        }

        // GET: LibrosController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var libros = await librosBL.GetByIdAsync(new Libros { Id = id });
            ViewBag.Autores = await autoresBL.GetAllAsync();
            ViewBag.Editoriales = await editorialesBL.GetAllAsync();
            ViewBag.Categorias = await categoriasBL.GetAllAsync();
            return View(libros);
        }

        // POST: LibrosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Libros libros)
        {
            try
            {
                int result = await librosBL.UpdateAsync(libros);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Autores = await autoresBL.GetAllAsync();
                ViewBag.Editoriales = await editorialesBL.GetAllAsync();
                ViewBag.Categorias = await categoriasBL.GetAllAsync();
                return View(libros);
            }
        }

        // GET: LibrosController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var libros = await librosBL.GetByIdAsync(new Libros { Id = id });
            libros.Autores = await autoresBL.GetByIdAsync(new Autores { Id = libros.AutorId });
            libros.Editoriales = await editorialesBL.GetByIdAsync(new Editoriales { Id = libros.EditorialId });
            libros.Categorias = await categoriasBL.GetByIdAsync(new Categorias { Id = libros.CategoriaId });
            return View(libros);
        }

        // POST: LibrosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Libros libros)
        {
            try
            {
                int result = await librosBL.DeleteAsync(libros);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var librosDb = await librosBL.GetByIdAsync(libros);
                if (librosDb == null)
                    librosDb = new Libros();
                if (librosDb.Id > 0)
                    librosDb.Autores = await autoresBL.GetByIdAsync(new Autores { Id = librosDb.AutorId });
                if (librosDb.Id > 0)
                    librosDb.Editoriales = await editorialesBL.GetByIdAsync(new Editoriales { Id = librosDb.EditorialId });
                if (librosDb.Id > 0)
                    librosDb.Categorias = await categoriasBL.GetByIdAsync(new Categorias { Id = librosDb.CategoriaId });

                return View(librosDb);
            }
        }
    }
}