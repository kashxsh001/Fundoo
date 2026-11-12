using BuisnessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System.Security.Claims;

namespace Fundoo2.Controllers
{
    [ApiController]
    [Route("api/notes")]
    [Authorize]
    public class NoteController:ControllerBase
    {
        private const string UserIdClaim = "user_id";
        private INotesBL _notesBL;
        public NoteController(INotesBL notesBL)
        {
            _notesBL = notesBL;
        }

        private bool TryGetUserId(out int UserId)
        {
            return int.TryParse(User.FindFirstValue(UserIdClaim),out UserId);
        }
        [HttpPost("add")]
        public async Task<ActionResult<ResponseModel<NoteModel>>> AddNote(NoteCreationModel model)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var note = await _notesBL.AddNote(userId, model);
            return Created(
                string.Empty,
                new ResponseModel<NoteModel>
                {
                    success = true,
                    message = "Note Created",
                    data = note
                });

        }
        [HttpGet("notes")]
        public async Task<ActionResult<ResponseModel<List<NoteModel>>>> ViewNotes()
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var notes = await _notesBL.ViewNotes(userId);
            return Ok(
                new ResponseModel<List<NoteModel>>
                {
                    success = true,
                    message = "Get all notes",
                    data = notes
                });
        }

        [HttpGet("note")]
        public async Task<ActionResult<ResponseModel<NoteModel>>> ViewNoteById(int noteId)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var note = await _notesBL.ViewNoteById(noteId,userId);
            if (note is null)
            {
                return NotFound(new ResponseModel<NoteModel>
                {
                    success = false,
                    message = "Note not found",
                    data = null
                });
            }
            return Ok(
                new ResponseModel<NoteModel>
                {
                    success = true,
                    message = "Note Reterived",
                    data = note
                });
        }
        [HttpPut("edit")]

        public async Task<ActionResult<ResponseModel<NoteModel>>> EditNote(EditNotesModel model)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var note = await _notesBL.EditNotes(userId, model);
            if (note is null)
            {
                return NotFound(new ResponseModel<NoteModel>
                {
                    success = false,
                    message = "Note not found",
                    data = null
                });
            }
            return Ok(new ResponseModel<NoteModel>
            {
                success = true,
                message = "Note Updated",
                data = note
            });
        }
        [HttpDelete("delete")]

        public async Task<IActionResult> DeleteNote([FromQuery]int noteID)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            return await _notesBL.DeleteNote(noteID, userId)? NoContent(): NotFound(new ResponseModel<bool>
            {
                success = false,
                message = "Note not found",
                data = false
            });

        }
        [HttpPatch("archieving")]
        public async Task<IActionResult> ArchUnarchive(int noteID)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            return await _notesBL.ArchUnarchive(noteID,userId) ? Ok(): NotFound(new ResponseModel<bool>
            {
                success = false,
                message = "Note not found",
                data = false
            });

        }

        [HttpPatch("trashing")]
        public async Task<IActionResult> TrashUntrash(int noteID)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            return await _notesBL.TrashUntrash(noteID, userId) ? Ok() : NotFound(new ResponseModel<bool>
            {
                success = false,
                message = "Note not found",
                data = false
            });

        }

    }
}
