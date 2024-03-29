﻿using BibliotecaESFE.EN;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaESFE.DAL
{
    public class LibrosDAL
    {
        public static async Task<int> CreateAsync(Libros libros)
        {
            int result = 0;
            using (var bdContexto = new ContextoBD())
            {
                bdContexto.Libros.Add(libros);
                result = await bdContexto.SaveChangesAsync();
            }
            return result;
        }


        public static async Task<int> UpdateAsync(Libros libros)
        {
            int result = 0;
            using (var bdContexto = new ContextoBD())
            {
                var librosDB = await bdContexto.Libros.FirstOrDefaultAsync(l => l.Id == libros.Id);
                if (librosDB != null)
                {
                   
                    librosDB.Titulo = libros.Titulo;
                    librosDB.AutorId = libros.AutorId;
                    librosDB.EditorialId = libros.EditorialId;
                    librosDB.CategoriaId = libros.CategoriaId;
                    librosDB.Disponibilidad = libros.Disponibilidad;
                    bdContexto.Update(librosDB);
                    result = await bdContexto.SaveChangesAsync();

                }
                return result;
            }
        }
        public static async Task<int> DeleteAsync(Libros libros)
        {
            int result = 0;
            using (var bdContexto = new ContextoBD())
            {
                var librosDb = await bdContexto.Libros.FirstOrDefaultAsync(l => l.Id == libros.Id);
                if (librosDb != null)
                {
                    bdContexto.Libros.Remove(librosDb);
                    result = await bdContexto.SaveChangesAsync();
                }
                return result;
            }

        }
        public static async Task<Libros> GetByIdAsync(Libros libros)
        {
            var librosDb = new Libros();
            using (var bdContexto = new ContextoBD())
            {
                librosDb = await bdContexto.Libros.FirstOrDefaultAsync(l => l.Id == libros.Id);
            }
            return librosDb!;
        }
        public static async Task<List<Libros>> GetAllAsync()
        {
            var libroes = new List<Libros>();
            using (var bdContexto = new ContextoBD())
            {
                libroes = await bdContexto.Libros.ToListAsync();
            }
            return libroes;
        }
        internal static IQueryable<Libros> QuerySelect(IQueryable<Libros> query, Libros libros)
        {
            if (libros.Id > 0)
            {
                query = query.Where(l => l.Id == libros.Id);
            }
            if (!string.IsNullOrEmpty(libros.Titulo))
            {
                query = query.Where(l => l.Titulo.Contains(libros.Titulo));
            }

            if (libros.AutorId > 0)
                query = query.Where(a => a.AutorId == libros.AutorId);

            if (libros.EditorialId > 0)
                query = query.Where(e => e.EditorialId == libros.EditorialId);

            if (libros.CategoriaId > 0)
                query = query.Where(c => c.CategoriaId == libros.CategoriaId);

            query = query.OrderByDescending(l => l.Id);
            if (libros.Top_Aux > 0)
            {
                query = query.Take(libros.Top_Aux).AsQueryable();
            }
            return query;
        }
        public static async Task<List<Libros>> SearchAsync(Libros libros)
        {
            var libroes = new List<Libros>();
            using (var bdContexto = new ContextoBD())
            {
                var select = bdContexto.Libros.AsQueryable();
                select = QuerySelect(select, libros);
                libroes = await select.ToListAsync();
            }
            return libroes;
        }
        public static async Task<List<Libros>> SearchIncludeCludeLibrosAsync(Libros libros)
        {
            var libro = new List<Libros>();
            using (var bdContxto = new ContextoBD())
            {
                var selec = bdContxto.Libros.AsQueryable();
                selec = QuerySelect(selec, libros).Include(l => l.Autores).Include(e => e.Editoriales).Include(c => c.Categorias).AsQueryable();
                libro = await selec.ToListAsync();
            }
            return libro;
        }
    }
}
