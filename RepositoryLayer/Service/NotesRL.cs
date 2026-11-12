using Microsoft.EntityFrameworkCore;
using ModelLayer;
using RepositoryLayer.context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Service
{
    public class NotesRL : INotesRL
    {
        private readonly Fundoo2Context _context;

        public NotesRL(Fundoo2Context context)
        {
            _context = context;
        }
        private static NoteModel ToModel(NoteEntity n)
        {
            NoteModel m = new NoteModel();
            m.NoteId = n.NoteId;
            m.Title = n.title;
            m.Description = n.description;
            m.Colour = n.colour;
            m.IsArchived = n.isArchived;
            m.IsDeleted = n.isDeleted;
            return m;
        }

        public async Task<NoteModel> AddNote(int userID, NoteCreationModel model)
        {
            NoteEntity note = new NoteEntity();
            note.title = model.Title;
            note.description = model.Description;
            note.colour = model.Colour;
            note.UserId = userID;

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return ToModel(note);

        }

        public async Task<bool> ArchUnarchive(int noteID, int userID)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.UserId == userID && n.NoteId == noteID);
            if (note == null) return false;
            note.isArchived = !note.isArchived;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNote(int noteID, int userID)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.UserId == userID && n.NoteId==noteID);
            if (note == null) return false;
             _context.Remove(note);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<NoteModel> EditNotes(int userID, EditNotesModel model)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.UserId == userID && n.NoteId==model.NoteId);
            if (note == null) return null;
            note.title = model.Title;
            note.description = model.Description;
            note.colour = model.Colour;
            await _context.SaveChangesAsync();
            return ToModel(note);


        }

        public async Task<bool> TrashUntrash(int noteID, int userID)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.UserId == userID && n.NoteId == noteID);
            if (note == null) return false;
            note.isDeleted = !note.isDeleted;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<NoteModel> ViewNoteById(int noteID, int userId)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.NoteId == noteID && n.UserId == userId);
            return note == null ? null : ToModel(note);
                                            
        }

        public async Task<List<NoteModel>> ViewNotes(int userId)
        {
            var notes =await _context.Notes.Select(e => e)
                                        .Where(x => x.UserId == userId)
                                        .ToListAsync();
            List<NoteModel> list = new List<NoteModel>();
            foreach(NoteEntity n in notes)
            {
                NoteModel note = ToModel(n);
                list.Add(note);
            }
            return list;

        }

    }
}
