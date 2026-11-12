using BuisnessLayer.Interface;
using Microsoft.Extensions.Logging;
using ModelLayer;
using RepositoryLayer.Interface;
using StackExchange.Redis;

namespace BuisnessLayer.Service
{
    public class NotesBL : INotesBL
    {
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

        private readonly INotesRL _notesRL;
        private readonly IDatabase _redis;
        private readonly ILogger<NotesBL> _logger;

        public NotesBL(INotesRL notesRL, IConnectionMultiplexer redis, ILogger<NotesBL> logger)
        {
            _notesRL = notesRL;
            _redis = redis.GetDatabase();
            _logger = logger;
        }

        private static string NoteKey(int userId, int noteId) => $"fundoo:note:{userId}:{noteId}";


        public async Task<NoteModel> AddNote(int userID, NoteCreationModel note)
        {
            var created = await _notesRL.AddNote(userID, note);

            try
            {
                
                await _redis.HashSetAsync(NoteKey(userID, created.NoteId), ToHash(created));
                _logger.LogInformation("Note {NoteId} saved permanently in Redis", created.NoteId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis write failed for new note {NoteId}", created.NoteId);
            }

            return created;
        }

        
        public async Task<NoteModel> ViewNoteById(int noteID, int userID)
        {
            string key = NoteKey(userID, noteID);

            
            try
            {
                HashEntry[] entries = await _redis.HashGetAllAsync(key);
                if (entries.Length > 0)
                {
                    _logger.LogInformation("CACHE HIT {Key}", key);
                    return FromHash(entries);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis read failed for {Key}", key);
            }

            
            _logger.LogInformation("CACHE MISS {Key}", key);
            var note = await _notesRL.ViewNoteById(noteID, userID);

            
            if (note != null)
            {
                try
                {
                    await _redis.HashSetAsync(key, ToHash(note));
                    await _redis.KeyExpireAsync(key, CacheTtl);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Redis write failed for {Key}", key);
                }
            }
            return note;
        }

       
        public async Task<NoteModel> EditNotes(int userID, EditNotesModel model)
        {
            var note = await _notesRL.EditNotes(userID, model);
            await RemoveCacheAsync(userID, model.NoteId);
            return note;
        }

        public async Task<bool> DeleteNote(int noteID, int userID)
        {
            bool ok = await _notesRL.DeleteNote(noteID, userID);
            if (ok) await RemoveCacheAsync(userID, noteID);
            return ok;
        }

        public async Task<bool> ArchUnarchive(int noteID, int userID)
        {
            bool ok = await _notesRL.ArchUnarchive(noteID, userID);
            if (ok) await RemoveCacheAsync(userID, noteID);
            return ok;
        }

        public async Task<bool> TrashUntrash(int noteID, int userID)
        {
            bool ok = await _notesRL.TrashUntrash(noteID, userID);
            if (ok) await RemoveCacheAsync(userID, noteID);
            return ok;
        }

        public Task<List<NoteModel>> ViewNotes(int userId)
            => _notesRL.ViewNotes(userId);

       
        private async Task RemoveCacheAsync(int userId, int noteId)
        {
            try
            {
                await _redis.KeyDeleteAsync(NoteKey(userId, noteId));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis remove failed for note {NoteId}", noteId);
            }
        }

        private static HashEntry[] ToHash(NoteModel n) => new[]
        {
            new HashEntry("NoteId",      n.NoteId),
            new HashEntry("Title",       n.Title ?? ""),
            new HashEntry("Description", n.Description ?? ""),
            new HashEntry("Colour",      n.Colour ?? ""),
            new HashEntry("IsArchived",  n.IsArchived ? "1" : "0"),
            new HashEntry("IsDeleted",   n.IsDeleted ? "1" : "0")
        };

        private static NoteModel FromHash(HashEntry[] entries)
        {
            var d = entries.ToDictionary(e => e.Name.ToString(), e => e.Value.ToString());
            return new NoteModel
            {
                NoteId = int.Parse(d["NoteId"]),
                Title = d["Title"],
                Description = d["Description"],
                Colour = d["Colour"],
                IsArchived = d["IsArchived"] == "1",
                IsDeleted = d["IsDeleted"] == "1"
            };
        }
    }
}