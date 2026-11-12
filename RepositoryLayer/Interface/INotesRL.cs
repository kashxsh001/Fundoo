using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface INotesRL
    {
        public Task<NoteModel> AddNote(int userID, NoteCreationModel note);
        public Task<List<NoteModel>> ViewNotes(int userId);
        public Task<NoteModel> ViewNoteById(int noteID,int userID);
        public Task<NoteModel> EditNotes(int userID,EditNotesModel model );
        public Task<bool> DeleteNote(int noteID, int userID);
        public Task<bool> ArchUnarchive(int noteID, int userID);
        public Task<bool> TrashUntrash(int noteID, int userID);
    }

    
}
